﻿using LMS.UI.MVC.Utilities;
using System.Web.Mvc;
using System.Web.Routing;

namespace LMS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.Add("CourseDetails", new SeoRoute("Course/Details/{id}",
            new RouteValueDictionary(new { controller = "Course", action = "Details" }),
            new MvcRouteHandler()));

            routes.Add("LessonDetails", new SeoRoute("Lesson/Details/{id}",
            new RouteValueDictionary(new { controller = "Lesson", action = "Details" }),
            new MvcRouteHandler()));

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}