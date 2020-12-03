using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LMS.DATA.EF;
using LMS.UI.MVC.Utilities;
using Microsoft.AspNet.Identity;

namespace LMS.UI.MVC.Controllers
{
    public class UserController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: User
        [Authorize]
        public ActionResult Index()
        {
            //Get current logged in user info
            string userId = User.Identity.GetUserId();
            UserDetail userDetail = db.UserDetails.Find(userId);
            if (userDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.Email = User.Identity.GetUserName();
            ViewBag.Name = $"{userDetail.FirstName} {userDetail.LastName}";
            if (userDetail.ReportsTo != null)
            {
                ViewBag.ManagerName = $"{userDetail.UserDetail1.FirstName} {userDetail.UserDetail1.LastName}";
            }

            return View(userDetail);
        }

        // GET: User/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserDetail userDetail = db.UserDetails.Find(id);
            if (userDetail == null)
            {
                return HttpNotFound();
            }
            return View(userDetail);
        }

        // GET: User/Create
        public ActionResult Create()
        {
            ViewBag.ReportsTo = new SelectList(db.UserDetails, "UserId", "FirstName");
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,FirstName,LastName,ReportsTo,UserPhoto")] UserDetail userDetail)
        {
            if (ModelState.IsValid)
            {
                db.UserDetails.Add(userDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ReportsTo = new SelectList(db.UserDetails, "UserId", "FirstName", userDetail.ReportsTo);
            return View(userDetail);
        }

        // GET: User/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserDetail userDetail = db.UserDetails.Find(id);
            if (userDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.Email = db.AspNetUsers.Find(userDetail.UserId).Email;
            ViewBag.Role = db.AspNetUsers.Find(userDetail.UserId).AspNetRoles;
            ViewBag.ReportsTo = new SelectList(db.UserDetails, "UserId", "FullName", userDetail.ReportsTo);
            return View(userDetail);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,FirstName,LastName,ReportsTo,UserPhoto")] UserDetail userDetail, HttpPostedFileBase userImg)
        {
            if (ModelState.IsValid)
            {
                #region File Upload - Using the Image Service
                //no default image to be concerned with all records in the database should have a vlaid file name
                //AND all files in the database should be represented in the Website Content folder.
                //if there is NO FILE in the input, maintain the existing image (Front End using the HiddenFor() field)

                //if the input is NOT Null, process image with the other udpates AND remove the OLD image
                if (userImg != null)
                {
                    //retrieve the fileName and assign it to a variable
                    string imgName = userImg.FileName;

                    //declare and assign the extension
                    string ext = imgName.Substring(imgName.LastIndexOf('.'));

                    //declare a good list of file extensions 
                    string[] goodExts = { ".jpeg", ".jpg", ".gif", ".png" };

                    //check the variable (ToLower()) against the list and verify the content lenght is less than 4MB
                    if (goodExts.Contains(ext.ToLower()) && (userImg.ContentLength <= 4194304))
                    {

                        //rename the file using a guid (see create POST for other unique naming options) - use the Convention in BOTH places
                        imgName = Guid.NewGuid() + ext.ToLower();//ToLower() is optional
                        //ResizeImage Values
                        //path
                        string savePath = Server.MapPath("~/Content/images/user/");

                        //actual image (converted image)
                        Image convertedImage = Image.FromStream(userImg.InputStream);

                        //maxImageSize
                        int maxImgSize = 500;

                        //maxThumbSize
                        int maxThumbSize = 150;

                        //Call the ImageService.ResizeImage()
                        ImageService.ResizeImage(savePath, imgName, convertedImage, maxImgSize, maxThumbSize);

                        //DELETE from the Image Service and delete the old image
                        //--Image Service Makes sure the fiel is NOT noImage.png && that it exists on the server BEFORE deleting
                        //we dont need to do taht check 
                        if (userDetail.UserPhoto != null)
                        {
                            ImageService.Delete(savePath, userDetail.UserPhoto);
                        }

                        //save the object ONLY if all other conditions are met
                        userDetail.UserPhoto = imgName;

                    }//and extgood if

                }//end if !=null
                 //HiddenFor() is used here (if the file information is not valid) OR if it fails the ext & size check
                #endregion

                db.Entry(userDetail).State = EntityState.Modified;
                db.SaveChanges();

                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("../Reports/Admin");
                }
                return RedirectToAction("Index");
            }
            ViewBag.ReportsTo = new SelectList(db.UserDetails, "UserId", "FirstName", userDetail.ReportsTo);
            return View(userDetail);
        }

        // GET: User/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserDetail userDetail = db.UserDetails.Find(id);
            if (userDetail == null)
            {
                return HttpNotFound();
            }
            return View(userDetail);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            UserDetail userDetail = db.UserDetails.Find(id);
            db.UserDetails.Remove(userDetail);
            db.SaveChanges();
            return RedirectToAction("Index");
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
