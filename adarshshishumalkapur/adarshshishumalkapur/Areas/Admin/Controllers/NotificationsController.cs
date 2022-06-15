using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Website.Dal;

namespace Website.Areas.Admin.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private DalContext db = new DalContext();
        private int _MonthsToExpirte = 12;
        // GET: Admin/Notifications
        public ActionResult Index()
        {
            return View(db.Notification.ToList());
        }

        // GET: Admin/Notifications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notification.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // GET: Admin/Notifications/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Notifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Notification notification, HttpPostedFileBase filedata)
        {
            if (ModelState.IsValid)
            {
                notification.StartDate = DateTime.UtcNow;
                notification.EndDate = DateTime.UtcNow.AddMonths(_MonthsToExpirte);
                string filename = null;
                //string path = Path.Combine(Server.MapPath("~/Uploads"), filename);
                if(Request.Files.Count > 0)
                {
                    var postedFile = Request.Files[0];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        string imagesPath = HttpContext.Server.MapPath("~/Uploads"); // Or file save folder, etc.
                        //string extension = Path.GetExtension(postedFile.FileName);
                        filename =   DateTime.Now.Ticks + "_" + Path.GetFileName(postedFile.FileName);
                        string saveToPath = Path.Combine(imagesPath, filename);
                        postedFile.SaveAs(saveToPath);
                        notification.Link = @"/uploads/" + filename;
                    }
                }
                
                //file.SaveAs(path);
                db.Notification.Add(notification);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(notification);
        }

        // GET: Admin/Notifications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notification.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // POST: Admin/Notifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,DisplayText,Link,StartDate,EndDate,Active")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                notification.StartDate = DateTime.UtcNow;
                notification.EndDate = DateTime.UtcNow.AddMonths(_MonthsToExpirte);
                db.Entry(notification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(notification);
        }

        // GET: Admin/Notifications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notification.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // POST: Admin/Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Notification notification = db.Notification.Find(id);
            db.Notification.Remove(notification);
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
