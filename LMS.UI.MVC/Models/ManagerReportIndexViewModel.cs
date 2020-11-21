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
        public int numCoursesCompletedYTD { get; set; }
        public int numLessonsCompletedYTD { get; set; }
    }
}