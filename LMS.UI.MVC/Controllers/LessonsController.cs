using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using LMS.DATA.EF;
using Microsoft.AspNet.Identity;

namespace LMS.UI.MVC.Controllers
{
    [HandleError]
    public class LessonsController : Controller
    {
        private LMSEntities db = new LMSEntities();
        private string[] allowedExts = new string[] { ".pdf" };

        // GET: Lessons
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var lessons = db.Lessons.Include(l => l.Course);
            return View(lessons.ToList());
        }

        // GET: Lessons/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }

            if (lesson.VideoURL != null)
            {
                var v = lesson.VideoURL.IndexOf("v=");
                var amp = lesson.VideoURL.IndexOf("&", v);
                string vid;
                // if the video id is the last value in the url
                if (amp == -1)
                {
                    vid = lesson.VideoURL.Substring(v + 2);
                    // if there are other parameters after the video id in the url
                }
                else
                {
                    vid = lesson.VideoURL.Substring(v + 2, amp - (v + 2));
                }
                ViewBag.VideoID = vid;
            }

            ViewBag.CurrentUserId = User.Identity.GetUserId();
            ViewBag.AlreadyCompletedMessage = TempData["AlreadyCompletedMessage"];

            return View(lesson);
        }

        // POST: Lessons/Completed/5
        [HttpPost]
        [Authorize(Roles = "Employee")]
        public ActionResult Completed(int? id)
        {
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            string userId = User.Identity.GetUserId();
            if (lesson.LessonViews.Where(x => x.LessonId == lesson.LessonId && userId == x.UserId).Count() > 0)
            {
                TempData["AlreadyCompletedMessage"] = "This lesson has already been completed, please do not attempt to resubmit!";
                return RedirectToAction("Details", new { id });
            }

                       
            LessonView lessonView = new LessonView
            {
                UserId = User.Identity.GetUserId(),
                LessonId = lesson.LessonId,
                DateViewed = DateTime.Now
            };

            db.LessonViews.Add(lessonView);
            db.SaveChanges();

            var howManyLessonsinCourse = (from howManyLesson in db.Lessons
                                          where howManyLesson.CourseId == lesson.CourseId
                                          select howManyLesson).ToList();

            var howManyLessonsinCourseCount = howManyLessonsinCourse.Count();

            var howManyCompletedLessonsInCourseForUser = (from completed in db.LessonViews
                                                          where completed.LessonId == completed.Lesson.LessonId && completed.Lesson.CourseId == lesson.CourseId && userId == completed.UserId
                                                          select completed).ToList();

            var howManyCompletedLessonsInCourseForUserCount = howManyCompletedLessonsInCourseForUser.Count();


            if (howManyLessonsinCourseCount == howManyCompletedLessonsInCourseForUserCount)
            {
                CourseCompletion courseCompletion = new CourseCompletion
                {
                    UserId = userId,
                    CourseId = lesson.CourseId,
                    DateCompleted = DateTime.Now
                };

                db.CourseCompletions.Add(courseCompletion);
                db.SaveChanges();

                UserDetail employeeThatCompleted = db.UserDetails.Find(userId);

                if (employeeThatCompleted.ReportsTo != null)
                {

                    var manager = db.AspNetUsers.Find(employeeThatCompleted.ReportsTo);

                    //Message to send to me
                    string message = $"<strong>{employeeThatCompleted.FirstName} {employeeThatCompleted.LastName}</strong> has completed <strong>{lesson.Course.CourseName}</strong> on <strong>{courseCompletion.DateCompleted:f}</strong>";


                    //Try to send email
                    try
                    {
                        client.Send(mm);
                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("Index", "Courses");
                    }
                }

                return RedirectToAction("Index", "Courses");//TODO: Maybe make a congrats course finished page?
            }

            List<int> allLessonsInCourseId = new List<int>();
            List<int> numCompletedforUserId = new List<int>();

            foreach (var allLessonsInCourse in howManyLessonsinCourse)
            {
                allLessonsInCourseId.Add(allLessonsInCourse.LessonId);
            }
            foreach (var numCompletedforUser in howManyCompletedLessonsInCourseForUser)
            {
                numCompletedforUserId.Add(numCompletedforUser.LessonId);
            }

            var nextLessonId = allLessonsInCourseId.Where(x => numCompletedforUserId.All(y => x != y));

            return RedirectToAction("Details", new { id = nextLessonId.FirstOrDefault() });
        }

        // GET: Lessons/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName");
            return View();
        }

        // TODO: add more lessons for the other courses
        // POST: Lessons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "LessonId,LessonTitle,CourseId,Introduction,VideoURL,PdfFilename,IsActive")] Lesson lesson, HttpPostedFileBase pdfFile)
        {
            if (ModelState.IsValid)
            {
                #region file upload
                string fileName = null;
                if (pdfFile != null)
                {
                    fileName = pdfFile.FileName;

                    string fileExtension = fileName.Substring(fileName.LastIndexOf(".")).ToLower();
                    if (allowedExts.Contains(fileExtension.ToLower()) && (pdfFile.ContentLength <= 4194304))
                    {
                        fileName = Guid.NewGuid() + fileExtension;
                        pdfFile.SaveAs(Server.MapPath("~/Content/lessonPdf/" + fileName));
                    }
                    else
                    {
                        fileName = string.Empty;
                    }
                }

                lesson.PdfFilename = fileName;
                #endregion

                db.Lessons.Add(lesson);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", lesson.CourseId);
            return View(lesson);
        }

        // GET: Lessons/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", lesson.CourseId);
            return View(lesson);
        }

        // POST: Lessons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "LessonId,LessonTitle,CourseId,Introduction,VideoURL,PdfFilename,IsActive")] Lesson lesson, HttpPostedFileBase pdfFile)
        {
            if (ModelState.IsValid)
            {
                #region File Upload
                if (pdfFile != null)
                {
                    string fileName = pdfFile.FileName;

                    string fileExtension = fileName.Substring(fileName.LastIndexOf(".")).ToLower();
                    if (allowedExts.Contains(fileExtension.ToLower()) && (pdfFile.ContentLength <= 4194304))
                    {
                        fileName = Guid.NewGuid() + fileExtension;
                        pdfFile.SaveAs(Server.MapPath("~/Content/lessonPdf/" + fileName));

                        if (lesson.PdfFilename != null)
                        {
                            System.IO.File.Delete(Server.MapPath("~/Content/lessonPdf/" + lesson.PdfFilename));
                        }

                        lesson.PdfFilename = fileName;
                    }
                }
                #endregion

                db.Entry(lesson).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", lesson.CourseId);
            return View(lesson);
        }

        // GET: Lessons/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            return View(lesson);
        }

        // POST: Lessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Lesson lesson = db.Lessons.Find(id);

            if (lesson.PdfFilename != null)
            {
                System.IO.File.Delete(Server.MapPath("~/Content/lessonPdf/" + lesson.PdfFilename));
            }

            db.Lessons.Remove(lesson);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
