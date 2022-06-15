using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Website.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult PageNotFound()
        {
            return View();
        }
        public ActionResult Unhandled()
        {
            return View();
        }
        public ActionResult AccessDenied()
        {
            return View();
        }

        //public ActionResult UnspecifiedError()
        public ActionResult UnspecifiedError()
        {
            return RedirectToAction("PageNotFound");
        }


    }
}