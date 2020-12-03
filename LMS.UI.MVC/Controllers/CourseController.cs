using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LMS.DATA.EF;
using LMS.UI.MVC.Utilities;
using Microsoft.AspNet.Identity;

namespace LMS.UI.MVC.Controllers
{
    [HandleError]
    public class CourseController : Controller
    {
        private LMSEntities db = new LMSEntities();
        private string[] goodExts = { ".jpeg", ".jpg", ".gif", ".png" };

        // GET: Courses
        public ActionResult Index()
        {
            ViewBag.CurrentUser = User.Identity.GetUserId();

            return View(db.Courses.Where(x => x.IsActive == true).ToList());
        }

        // GET: Courses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            if (course.IsActive == false && !User.IsInRole("Admin"))
            {
                return RedirectToAction("../Home/Index");
            }

            ViewBag.CurrentUserId = User.Identity.GetUserId();

            return View(course);
        }

        // GET: Courses/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CourseId,CourseName,CourseDescription,IsActive,Category,CourseImg")] Course course, HttpPostedFileBase courseImg)
        {
            if (ModelState.IsValid)
            {
                #region File Uploade - Using the Image Service

                string imgName = "noImg.png";

                if (courseImg != null)
                {

                    imgName = courseImg.FileName;

                    string ext = imgName.Substring(imgName.LastIndexOf('.'));

                    if (goodExts.Contains(ext.ToLower()) && (courseImg.ContentLength <= 4194304))
                    {
                        imgName = Guid.NewGuid() + ext.ToLower();

                        string savePath = Server.MapPath("~/Content/images/courses/");

                        Image convertedImage = Image.FromStream(courseImg.InputStream);

                        int maxImageSize = 500;

                        int maxThumbSize = 100;

                        ImageService.ResizeImage(savePath, imgName, convertedImage, maxImageSize, maxThumbSize);
                    }
                    else
                    {
                        imgName = "noImage.png";
                    }

                }
                course.CourseImg = imgName;
                #endregion


                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("../Reports/Admin");
            }

            return View(course);
        }

        // GET: Courses/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "CourseId,CourseName,CourseDescription,IsActive,Category,CourseImg")] Course course, HttpPostedFileBase courseImg)
        {
            if (ModelState.IsValid)
            {
                #region File Upload - Using the Image Service
                if (courseImg != null)
                {
                    string imgName = courseImg.FileName;

                    string ext = imgName.Substring(imgName.LastIndexOf('.'));

                    if (goodExts.Contains(ext.ToLower()) && (courseImg.ContentLength <= 4194304))
                    {

                        imgName = Guid.NewGuid() + ext.ToLower();

                        string savePath = Server.MapPath("~/Content/images/courses/");

                        Image convertedImage = Image.FromStream(courseImg.InputStream);

                        int maxImgSize = 500;

                        int maxThumbSize = 100;

                        ImageService.ResizeImage(savePath, imgName, convertedImage, maxImgSize, maxThumbSize);

                        ImageService.Delete(savePath, course.CourseImg);

                        course.CourseImg = imgName;

                    }

                }
                #endregion


                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("../Reports/Admin");
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            if (course.Lessons.Count < 1)
            {
                db.Courses.Remove(course);

                ImageService.Delete(Server.MapPath("~/Content/images/courses/"), course.CourseImg);

                db.SaveChanges();
            }

            return RedirectToAction("../Reports/Admin");
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
