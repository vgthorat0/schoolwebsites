using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Dal;

namespace Website.Controllers
{
    public class DepartmentsController : Controller
    {
        DalContext dal = new DalContext();

            // GET: Departments
            public ActionResult Arts()
        {
           ViewBag.Notifications = dal.Notification.Where(x => x.Active).ToList();  return View(); 
        }
        public ActionResult Science()
        {
           ViewBag.Notifications = dal.Notification.Where(x => x.Active).ToList();  return View(); 
        }

        public ActionResult Commerce()
        {
           ViewBag.Notifications = dal.Notification.Where(x => x.Active).ToList();  return View(); 
        }

        public ActionResult Cultural()
        {
           ViewBag.Notifications = dal.Notification.Where(x => x.Active).ToList();  return View(); 
        }

        public ActionResult Exam()
        {
           ViewBag.Notifications = dal.Notification.Where(x => x.Active).ToList();  return View(); 
        }

        public ActionResult Sports()
        {
           ViewBag.Notifications = dal.Notification.Where(x => x.Active).ToList();  return View(); 
        }

        public ActionResult SchoolTrip()
        {
           ViewBag.Notifications = dal.Notification.Where(x => x.Active).ToList();  return View(); 
        }

        public ActionResult StudentHelth()
        {
           ViewBag.Notifications = dal.Notification.Where(x => x.Active).ToList();  return View(); 
        }
    }
}