using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.UI.MVC.Models
{
    public class CompanyAdminViewModel
    {
        public string ManagerId { get; set; }
        public string ManagerFirstName { get; set; }
        public string ManagerLastName { get; set; }
        public int NumOfDirectReports { get; set; }

        public string EmployeeId { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
    }

}