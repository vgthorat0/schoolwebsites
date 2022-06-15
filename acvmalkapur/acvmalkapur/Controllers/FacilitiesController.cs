using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Dal;

namespace Website.Controllers
{
    public class FacilitiesController : Controller
    {
        DalContext dal = new DalContext();
        
        // GET: Facilities
        public ActionResult Library()
        {
            ViewBag.Notifications = dal.Notification.Where(x => x.Active).ToList();
            return View();
        }
        public ActionResult Laboratory()
        {
            ViewBag.Notifications = dal.Notification.Where(x => x.Active).ToList();
            return View();
        }
        public ActionResult PlayGround()
        {
            ViewBag.Notifications = dal.Notification.Where(x => x.Active).ToList();
            return View();
        }
        public ActionResult ArtGallery()
        {
            ViewBag.Notifications = dal.Notification.Where(x => x.Active).ToList();
            return View();
        }
        public ActionResult CustomerStore()
        {
            ViewBag.Notifications = dal.Notification.Where(x => x.Active).ToList();
            return View();
        }
        
    }
}