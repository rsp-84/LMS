using LMS.DATA.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.UI.MVC.Models
{
    public class ManagerReportIndexViewModel
    {
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public int NumCoursesCompletedYTD { get; set; }
        public int NumLessonsCompletedYTD { get; set; }
        public List<Course> CoursesCompletedYTD { get; set; }
    }
}