using LMS.DATA.EF;
using LMS.UI.MVC.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LMS.UI.MVC.Controllers
{
    public class ReportsController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        // GET: Employee
        [Authorize(Roles = "Employee")]
        public ActionResult Employee()
        {
            //Get current logged in user info
            string userId = User.Identity.GetUserId();
            UserDetail userDetail = db.UserDetails.Find(userId);
            if (userDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeeName = $"{userDetail.FirstName} {userDetail.LastName}";
            ViewBag.ManagerName = $"{userDetail.UserDetail1.FirstName} {userDetail.UserDetail1.FirstName}";

            //List for all view info----------
            List<EmployeeReportsViewModel> employeeReportsVM = new List<EmployeeReportsViewModel>();

            //Get the data for the VM----------
            var userLessonData = (from completedLesson in db.LessonViews
                                  where completedLesson.LessonId == completedLesson.Lesson.LessonId && userId == completedLesson.UserId
                                  select new { completedLesson.Lesson.LessonTitle, completedLesson.DateViewed }).ToList();

            var userCourseData = (from completedCourse in db.CourseCompletions
                                  where completedCourse.CourseId == completedCourse.Course.CourseId && userId == completedCourse.UserId
                                  select new { completedCourse.Course.CourseName, completedCourse.Course.CourseImg, completedCourse.Course.Category, completedCourse.DateCompleted }).ToList();

            //Build ViewModel----------
            foreach (var item in userLessonData)
            {
                EmployeeReportsViewModel objErVM = new EmployeeReportsViewModel();

                objErVM.LessonTitle = item.LessonTitle;
                objErVM.LessonCompletedDate = item.DateViewed;

                employeeReportsVM.Add(objErVM);
            }
            foreach (var item in userCourseData)
            {
                EmployeeReportsViewModel objErVM = new EmployeeReportsViewModel();

                objErVM.CourseName = item.CourseName;
                objErVM.CourseImg = item.CourseImg;
                objErVM.CouseCat = item.Category;
                objErVM.CourseCompletedDate = item.DateCompleted;

                employeeReportsVM.Add(objErVM);
            }


            //Return info to view----------
            return View(employeeReportsVM);
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