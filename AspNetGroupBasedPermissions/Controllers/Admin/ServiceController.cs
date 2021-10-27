using AspNetGroupBasedPermissions.Filters;
using AspNetGroupBasedPermissions.Helpers;
using Core.Entities.Data;
using Core.Entities.Data.Admin;
using Infrastructures.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AspNetGroupBasedPermissions.Controllers.Admin
{
    public class ServiceController : Controller
    {
        private Infrastructures.Data.ApplicationDbContext db = new Infrastructures.Data.ApplicationDbContext();

  //   [Authorize(Roles = "Admin, CanEditGroup, CanEditUser")]
   //  [CustomAuthorize(Roles = Roles.Admin)]
       [EncryptedActionParameter]
        public ActionResult IndexService()
        {

            //var DetailCD = db.Services.Where(h => h.IsParent == true);
            //ViewBag.ParentDesc = DetailCD.Select(x => x.NameOption).Distinct();
            return View();
        }

        [HttpPost]
        public ActionResult GetService(string search, int pageSize, int pageIndex)
        {
            try
            {
                var POBs = (from a in db.Services
                            select new
                            {
                                ID = a.Id,
                                ServiceName = a.NameOption,
                                ParentName = "",
                                Controller = a.Controller,
                                ImageClass = a.ImageClass,
                                SequenceNo = a.Sequence,
                                Status = a.Status,
                                IsParent = a.IsParent,
                                IsActive = a.IsActive,
                            }).OrderByDescending(c => c.ServiceName);

                int totalRow = 0;
                int totalPage = 0;
                int skip = pageIndex * pageSize;

                var models = POBs.Where(x => (x.ServiceName.Contains(search)))
                            .Skip(skip)
                            .Take(pageSize)
                            .ToList();
                totalRow = POBs.Where(x => (x.ServiceName.Contains(search))).Count();
                totalPage = (totalRow + pageSize - 1) / pageSize;
                totalPage = totalPage == 0 ? 1 : totalPage;

                var hocs = models == null ? null :
                            models.Select(a =>
                             new
                             {
                                 a.ID,
                                 a.ServiceName,
                                 a.ParentName,
                                 a.Controller,
                                 a.ImageClass,
                                 a.SequenceNo,
                                 a. Status,
                                 a.IsParent,
                                 a.IsActive,
                                 ShowButton = true,
                                 EditURL = EncyptURLHelper.EncodedActionUrl("Edit", "Service", new { id = a.ID, area = " " }),
                                 DetailURL = EncyptURLHelper.EncodedActionUrl("Details", "Service", new { id = a.ID, area = " " }),
                                 DeleteURL = EncyptURLHelper.EncodedActionUrl("Delete", "Service", new { id = a.ID }),

                             });
                return Json(new { hocs = hocs, totalPage = totalPage, totalRow = totalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Console.WriteLine("Page Error. Contact System Administrator");
                throw;
            }
        }

        public ActionResult IndexMobile()
        {
            return View(db.Services.ToList());
        }

        [EncryptedActionParameter]
        public ActionResult DetailService(int? ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service service = db.Services.Find(ID);
            if (service == null)
            {
                return HttpNotFound();
            }
            var services = new List<Service>();
            var newService = new Service() { Id = 0, NameOption = "-- No Parent --" };
            services.Add(newService);
            services.AddRange(db.Services.ToList());
            ViewBag.ParentID = new SelectList(services, "Id", "NameOption", service.ParentId);
            return View(service);
        }

        //  [Authorize(Roles = "Admin, CanEditGroup")]
        [EncryptedActionParameter]
        public ActionResult CreateService()
        {
            var services = new List<Service>();
            var newService = new Service() { Id = 0, NameOption = "-- No Parent --" };
            services.Add(newService);
            services.AddRange(db.Services.ToList());
            ViewBag.ParentID = new SelectList(services, "Id", "NameOption");
            return View();
        }

    //  [Authorize(Roles = "Admin, CanEditGroup")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateService([Bind(Include="Id, NameOption, Controller, Action, Area, ImageClass, Status, Description ,IsActive, ParentId, IsParent, HasChild, DateCreated, DateModified, UserCreated, UserModified")] Service service)
        {
            service.IsActive = true;
            service.Status = "Draft";

            if (ModelState.IsValid)
            {
                if (service.ParentId == 0)
                {
                    service.IsParent = true;
                    service.HasChild = true;
                }
                else
                {
                    service.IsParent = false;
                    service.HasChild = false;
                }

                db.Services.Add(service);
                db.SaveChanges();
                return RedirectToAction("IndexService");
            }

            return View(service);
        }

   //     [Authorize(Roles = "Admin, CanEditGroup")]
        [EncryptedActionParameter]
        public ActionResult EditService(int? ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service service = db.Services.Find(ID);
            if (service == null)
            {
                return HttpNotFound();
            }
            var services = new List<Service>();
            var newService = new Service() { Id = 0, NameOption = "-- No Parent --" };
            services.Add(newService);
            services.AddRange(db.Services.ToList());
            ViewBag.ParentID = new SelectList(services, "Id", "NameOption", service.ParentId);
            return View(service);
        }

    //  [Authorize(Roles = "Admin, CanEditGroup")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult EditService([Bind(Include="Id, NameOption, Controller, Action, Area, ImageClass, IsActive, Status, Description, ParentId, IsParent, HasChild, DateCreated, DateModified, UserCreated, UserModified")] Service service)
        {
            if (ModelState.IsValid)
            {
                if (service.ParentId == 0)
                {
                    service.IsParent = true;
                    service.HasChild = true;
                }
                else
                {
                    service.IsParent = false;
                    service.HasChild = false;
                }
                db.Entry(service).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexService");
            }
            return View(service);
        }

      //[Authorize(Roles = "Admin, CanEditGroup")]
        [EncryptedActionParameter]
        public ActionResult DeleteService(int? ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service service = db.Services.Find(ID);
            if (service == null)
            {
                return HttpNotFound();
            }
            var services = new List<Service>();
            var newService = new Service() { Id = 0, NameOption = "-- No Parent --" };
            services.Add(newService);
            services.AddRange(db.Services.ToList());
            ViewBag.ParentID = new SelectList(services, "Id", "NameOption", service.ParentId);
            if (db.ApplicationUserService.Where(x => x.ServiceId == ID).Count() > 0)
            {
                ViewBag.CanDelete = "No";
                return View(service);
            }
            return View(service);
        }

        // [Authorize(Roles = "Admin, CanEditGroup")]
        [HttpPost, ActionName("DeleteService")]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult DeleteServiceConfirmed(int ID)
        {
            Service service = db.Services.Find(ID);
            if(db.ApplicationUserService.Where(x=>x.ServiceId == ID).Count()>0)
            {
                ModelState.AddModelError("Used", "The service is currently being used by user(s)");
                return View(service);
            }

            var idManager = new IdentityManager();
            idManager.DeleteService(ID);
            return RedirectToAction("IndexService");
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

