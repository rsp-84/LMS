using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.UI.MVC.Models
{
    public class ManagerReportIndexViewModel
    {
        public string EmployeeId { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public int NumCoursesCompletedYTD { get; set; }
        public int NumLessonsCompletedYTD { get; set; }
        public List<CoursesCompleted> CoursesCompletedYTD { get; set; }
        public List<LessonsCompleted> LessonsCompletedYTD { get; set; }
    }

    public class CoursesCompleted
    {
        public string CourseName { get; set; }
        public string CourseCat { get; set; }
        public DateTime CourseCompletedDate { get; set; }
    }

    public class LessonsCompleted
    {
        public string LessonName { get; set; }
        public DateTime LessonCompletedDate { get; set; }
    }
}