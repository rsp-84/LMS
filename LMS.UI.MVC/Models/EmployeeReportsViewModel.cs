using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.UI.MVC.Models
{
    public class EmployeeReportsViewModel
    {
        public string LessonTitle { get; set; }
        public DateTime LessonCompletedDate { get; set; }

        public string CourseName { get; set; }
        public string CourseImg { get; set; }
        public string CouseCat { get; set; }
        public DateTime CourseCompletedDate { get; set; }
    }
}