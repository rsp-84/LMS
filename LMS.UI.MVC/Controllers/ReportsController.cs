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
            if (userDetail.UserPhoto != null)
            {
                ViewBag.EmployeeImg = userDetail.UserPhoto;
            }
            else
            {
                ViewBag.EmployeeImg = null;
            }

            if (userDetail.ReportsTo != null)
            {
                ViewBag.ManagerName = $"{userDetail.UserDetail1.FirstName} {userDetail.UserDetail1.LastName}";
            }


            //List for all view info----------
            List<EmployeeReportsViewModel> employeeReportsVM = new List<EmployeeReportsViewModel>();

            //Get the data for the VM----------
            var userLessonData = (from completedLesson in db.LessonViews
                                  where completedLesson.LessonId == completedLesson.Lesson.LessonId && userId == completedLesson.UserId
                                  select new { completedLesson.Lesson.LessonTitle, completedLesson.DateViewed }).ToList();

            ViewBag.LessonCompleteCount = userLessonData.Where(x => x.DateViewed.Year == DateTime.Now.Year).Count();

            var userCourseData = (from completedCourse in db.CourseCompletions
                                  where completedCourse.CourseId == completedCourse.Course.CourseId && userId == completedCourse.UserId
                                  select new { completedCourse.Course.CourseName, completedCourse.Course.CourseImg, completedCourse.Course.Category, completedCourse.DateCompleted }).ToList();

            ViewBag.CourseCompleteCount = userCourseData.Where(x => x.DateCompleted.Year == DateTime.Now.Year).Count();

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

        // GET: Manager
        [Authorize(Roles = "Manager")]
        public ActionResult Manager()
        {
            //Logged in manager
            var loggedInManager = User.Identity.GetUserId();

            //get data
            List<ManagerReportIndexViewModel> managerReportIndexVM = new List<ManagerReportIndexViewModel>();

            var allAssignedEmployees = (from employees in db.UserDetails
                                       where employees.ReportsTo == loggedInManager
                                       select new { employees.UserId, employees.FirstName, employees.LastName }).ToList();
            
            //Build VM
            foreach (var item in allAssignedEmployees)
            {
                ManagerReportIndexViewModel objMrVM = new ManagerReportIndexViewModel();
                var numCourses = db.CourseCompletions.Where(x => x.UserId == item.UserId).Select(x => x.DateCompleted.Year);
                var numLessons = db.LessonViews.Where(x => x.UserId == item.UserId).Select(x => x.DateViewed.Year);

                int courseCount = 0;
                int lessonCount = 0;

                foreach (var courseYear in numCourses)
                {
                    if (courseYear == DateTime.Now.Year)
                    {
                        courseCount++;
                    }
                }
                foreach (var lessonYear in numLessons)
                {
                    if (lessonYear == DateTime.Now.Year)
                    {
                        lessonCount++;
                    }
                }


                objMrVM.EmployeeFirstName = item.FirstName;
                objMrVM.EmployeeLastName = item.LastName;
                objMrVM.numCoursesCompletedYTD = courseCount;
                objMrVM.numLessonsCompletedYTD = lessonCount;

                managerReportIndexVM.Add(objMrVM);
            }

            ViewBag.ManagerImg = db.UserDetails.Find(loggedInManager).UserPhoto;
            ViewBag.ManagerName = $"{db.UserDetails.Find(loggedInManager).FirstName} {db.UserDetails.Find(loggedInManager).LastName}";

            return View(managerReportIndexVM);
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