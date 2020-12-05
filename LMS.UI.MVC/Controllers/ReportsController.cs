using LMS.DATA.EF;
using LMS.UI.MVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
                                  where completedLesson.LessonId == completedLesson.Lesson.LessonId && userId == completedLesson.UserId && completedLesson.DateViewed.Year == DateTime.Now.Year
                                  select new { completedLesson.LessonId, completedLesson.Lesson.LessonTitle, completedLesson.DateViewed }).ToList();

            ViewBag.LessonCompleteCount = userLessonData.Where(x => x.DateViewed.Year == DateTime.Now.Year).Count();

            var userCourseData = (from completedCourse in db.CourseCompletions
                                  where completedCourse.CourseId == completedCourse.Course.CourseId && userId == completedCourse.UserId && completedCourse.DateCompleted.Year == DateTime.Now.Year
                                  select new { completedCourse.CourseId, completedCourse.Course.CourseName, completedCourse.Course.CourseImg, completedCourse.Course.Category, completedCourse.DateCompleted }).ToList();

            ViewBag.CourseCompleteCount = userCourseData.Where(x => x.DateCompleted.Year == DateTime.Now.Year).Count();

            //Build ViewModel----------
            foreach (var item in userLessonData)
            {
                EmployeeReportsViewModel objErVM = new EmployeeReportsViewModel();

                objErVM.LessonId = item.LessonId;
                objErVM.LessonTitle = item.LessonTitle;
                objErVM.LessonCompletedDate = item.DateViewed;

                employeeReportsVM.Add(objErVM);
            }
            foreach (var item in userCourseData)
            {
                EmployeeReportsViewModel objErVM = new EmployeeReportsViewModel();

                objErVM.CourseId = item.CourseId;
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
                var courseList = db.CourseCompletions.Where(x => x.UserId == item.UserId && x.DateCompleted.Year == DateTime.Now.Year).Select(x => new CoursesCompleted { CourseId = x.CourseId ,CourseName = x.Course.CourseName, CourseCat = x.Course.Category, CourseCompletedDate = x.DateCompleted }).ToList();
                var lessonList = db.LessonViews.Where(x => x.UserId == item.UserId && x.DateViewed.Year == DateTime.Now.Year).Select(x => new LessonsCompleted { LessonId = x.LessonId, LessonName = x.Lesson.LessonTitle, LessonCompletedDate = x.DateViewed }).ToList();
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

                objMrVM.EmployeeId = item.UserId;
                objMrVM.EmployeeFirstName = item.FirstName;
                objMrVM.EmployeeLastName = item.LastName;
                objMrVM.NumCoursesCompletedYTD = courseCount;
                objMrVM.NumLessonsCompletedYTD = lessonCount;
                objMrVM.CoursesCompletedYTD = courseList;
                objMrVM.LessonsCompletedYTD = lessonList;

                managerReportIndexVM.Add(objMrVM);
            }

            ViewBag.ManagerImg = db.UserDetails.Find(loggedInManager).UserPhoto;
            ViewBag.ManagerName = $"{db.UserDetails.Find(loggedInManager).FirstName} {db.UserDetails.Find(loggedInManager).LastName}";

            return View(managerReportIndexVM);
        }

        // GET: Admin
        [Authorize(Roles = "Admin")]
        public ActionResult Admin()
        {
            //Logged in admin
            var loggedInAdmin = User.Identity.GetUserId();

            //get data
            List<CompanyAdminViewModel> companyAdminVM = new List<CompanyAdminViewModel>();

            var managers = db.AspNetUsers
                  .Where(u => u.AspNetRoles.Any(r => r.Name == "Manager"))
                  .ToList();

            var employees = db.AspNetUsers
                  .Where(u => u.AspNetRoles.Any(r => r.Name == "Employee"))
                  .ToList();

            //Build VM
            foreach (var item in managers)
            {
                CompanyAdminViewModel objAdmVM = new CompanyAdminViewModel();

                objAdmVM.ManagerId = item.Id;
                objAdmVM.ManagerFirstName = db.UserDetails.Find(item.Id).FirstName;
                objAdmVM.ManagerLastName = db.UserDetails.Find(item.Id).LastName;
                objAdmVM.NumOfDirectReports = db.UserDetails.Where(x => x.ReportsTo == item.Id).Count();

                companyAdminVM.Add(objAdmVM);
            }
            foreach (var item in employees)
            {
                CompanyAdminViewModel objAdmVM = new CompanyAdminViewModel();

                objAdmVM.EmployeeId = item.Id;
                objAdmVM.EmployeeFirstName = db.UserDetails.Find(item.Id).FirstName;
                objAdmVM.EmployeeLastName = db.UserDetails.Find(item.Id).LastName;
                string managerId = db.UserDetails.Find(item.Id).ReportsTo;
                objAdmVM.EmployeeManagerName = managerId != null ? $"{db.UserDetails.Find(managerId).FirstName} {db.UserDetails.Find(managerId).LastName}" : "";

                companyAdminVM.Add(objAdmVM);
            }

            ViewBag.NumOfManagers = managers.Count;
            ViewBag.NumOfEmployees = employees.Count;
            ViewBag.AdminImg = db.UserDetails.Find(loggedInAdmin).UserPhoto;
            ViewBag.AdminName = $"{db.UserDetails.Find(loggedInAdmin).FirstName} {db.UserDetails.Find(loggedInAdmin).LastName}";

            //----- Bring Courses and Lessons into View
            ViewBag.ActiveCourses = db.Courses.Where(x => x.IsActive == true);
            ViewBag.InactiveCourses = db.Courses.Where(x => x.IsActive == false);

            ViewBag.ActiveLessons = db.Lessons.Where(x => x.IsActive == true);
            ViewBag.InactiveLessons = db.Lessons.Where(x => x.IsActive == false);

            return View(companyAdminVM);
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