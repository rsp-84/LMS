﻿//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using LMS.DATA.EF;

//namespace LMS.UI.MVC.Controllers
//{
//    public class UserDetailsController : Controller
//    {
//        private LMSEntities db = new LMSEntities();

//        // GET: UserDetails/Edit/5
//        public ActionResult Edit(string id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            UserDetail userDetail = db.UserDetails.Find(id);
//            if (userDetail == null)
//            {
//                return HttpNotFound();
//            }
//            ViewBag.ReportsTo = new SelectList(db.UserDetails, "UserId", "FullName", userDetail.ReportsTo);
//            return View(userDetail);
//        }

//        // POST: UserDetails/Edit/5
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit([Bind(Include = "UserId,FirstName,LastName,ReportsTo,UserPhoto")] UserDetail userDetail)
//        {
//            if (ModelState.IsValid)
//            {
//                db.Entry(userDetail).State = EntityState.Modified;
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }
//            ViewBag.ReportsTo = new SelectList(db.UserDetails, "UserId", "FirstName", userDetail.ReportsTo);
//            return View(userDetail);
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//    }
//}
