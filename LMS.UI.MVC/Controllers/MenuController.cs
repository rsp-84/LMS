using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LMS.DATA.EF;
using Microsoft.AspNet.Identity;

namespace LMS.UI.MVC.Controllers
{
    public class MenuController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: CourseComplete
        public ActionResult CourseComplete()
        {
            var userDetails = db.UserDetails.Find(User.Identity.GetUserId());

            if (userDetails.CourseCompletions.Count > 0)
            {
                ViewBag.CompletedNum = (userDetails.CourseCompletions.Where(x => x.DateCompleted.Year == DateTime.Now.Year)).Count();
            }
            else
            {
                ViewBag.CompletedNum = 0;
            }

            return PartialView(userDetails);
        }
    }
}
