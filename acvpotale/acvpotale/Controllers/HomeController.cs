using Application.Utilities;
using Microsoft.AspNet.Identity;
using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Website.Dal;
using Website.Models;

namespace Website.Controllers
{
    public class HomeController : Controller
    {
        DalContext dal = new DalContext();
        public ActionResult Index()
        {
          
            ViewBag.Notifications = dal.Notification.Where(x => x.Active).ToList();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            //DalContext db = new DalContext();
           // db.Event.Add(new Event { photo = "sdfsdf.jpg", Name ="test"});
           // db.SaveChanges();
            return View();
        }

        public async Task<ActionResult> Contact()
        {
                //  ViewBag.Message = "Your contact page.";
                //IdentityMessage message = new IdentityMessage();
                //message.Destination = "vijaygthorat@gmail.com";
                //message.Subject = "Test";
                //message.Body = "test body";
                //var mail = new EmailService();
                //await mail.SendAsync(message);
                return View();

        }

        [HttpPost]
        public ActionResult Contact(Contact model)
        {
            return View();
        }

        public ActionResult Gallery()
        {
            ViewBag.Events = dal.Event.ToList().OrderByDescending(x => x.Priority);
            return View();
        }

        public ActionResult SuccessStories()
        {
            return View();
        }

        public ActionResult Events()
        {
            return View();
        }

        public ActionResult Schools()
        {
            return View();
        }

        public ActionResult AssociateOrganization()
        {
            return View();
        }
        
            
    }


        
        
    
}