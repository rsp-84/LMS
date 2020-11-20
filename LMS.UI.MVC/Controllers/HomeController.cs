using LMS.DATA.EF;
using LMS.UI.MVC.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace LMS.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        private LMSEntities db = new LMSEntities();

        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.CurrentUser = User.Identity.GetUserId();

            return View(db.Courses.ToList());
        }

        [HttpGet]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Contact(ContactViewModel cvm)
        {
            //Checks model state, if bad send back to form.
            if (!ModelState.IsValid)
            {
                return View(cvm);
            }

            //Message to send to me
            string message = $"You have received an email from <strong>{cvm.Name}</strong> with a subject <strong>{cvm.Subject}</strong>. Please respond to <strong>{cvm.Email}</strong> with your response to the following message:<br />{cvm.Message}";


            //Try to send email
            try
            {
                client.Send(mm);
            }
            catch (Exception ex)
            {
                ViewBag.CustomerMessage = $"We're sorry your request could not be completed at this time. Please try again later.<br />Error Message:<br />{ex.StackTrace}";
                return View(cvm);
            }

            //--success
            return View("EmailConfirmation", cvm);
        }

        [HttpGet]
        public ActionResult Courses()
        {

            return View();
        }
    }
}
