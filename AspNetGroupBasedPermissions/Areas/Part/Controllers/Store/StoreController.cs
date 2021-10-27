using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Linq.Dynamic;
using Microsoft.AspNet.Identity;
using AspNetGroupBasedPermissions.Helpers;
using System.Web.UI.WebControls;
using AspNetGroupBasedPermissions.Filters;
using System.Net;
using System.Threading.Tasks;
using Core.Entities.Data.PartData;

namespace AspNetGroupBasedPermissions.Areas.Part.Controllers
{
    public class StoreController : Controller
    {
        private Infrastructures.Data.ApplicationDbContext db = new Infrastructures.Data.ApplicationDbContext();

        [EncryptedActionParameter]

        public ActionResult IndexStore()
        {
            var DetailCD = db.Stores.Include(x => x.RefSectionID);
            ViewBag.Section = DetailCD.Select(x => x.RefSectionID.SectionName).Distinct();
            ViewBag.Status = DetailCD.Select(x => x.Status).Distinct();
            return View();
        }


        [HttpPost]
        public ActionResult GetStore(string search, int pageSize, int pageIndex, 
            string section, string status)
        {
            try
            {
                var detailrec = (from a in db.Stores
                                 select new
                                 {
                                     StoreID = a.WhouseID,
                                     StoreName = a.WhouseName,
                                     SectionName = a.RefSectionID.SectionName,
                                     Description = a.Description,
                                     Status = a.Status,
                                     IsActive = a.IsActive,
                                 }).OrderByDescending(c => c.StoreID);

                int totalRow = 0;
                int totalPage = 0;
                int skip = pageIndex * pageSize;

                var models = detailrec.Where(x => (x.StoreName.Contains(search))
                             && (x.Status == status ||  "All" == status))
                            .Skip(skip)
                            .Take(pageSize)
                            .ToList();
                totalRow = detailrec.Where(x => (x.Status.Contains(search))
                            && (x.Status == status || "All" == status)).Count();
           
                totalPage = (totalRow + pageSize - 1) / pageSize;
                totalPage = totalPage == 0 ? 1 : totalPage;

                var hocs = models == null ? null :
                            models.Select(a =>
                             new
                             {
                                 a.StoreID,
                                 a.StoreName,
                                 a.Description,
                                 a.SectionName,
                                 a.Status,
                                 a.IsActive,
                                 ShowButton = true,
                                 DetailURL = EncyptURLHelper.EncodedActionUrl("DetailStore", "Store", new { StoreID = a.StoreID, area = "Part" }),
                                 EditURL = EncyptURLHelper.EncodedActionUrl("EditStore", "Store", new { StoreID = a.StoreID, area = "Part" }),
                             });
                return Json(new { hocs = hocs, totalPage = totalPage,  totalRow = totalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Console.WriteLine("Page Error. Contact System Administrator");
                throw;
            }
        }

        public ActionResult CheckDuplicate(string storename)
        {

            if (db.Stores.Where(x => x.WhouseName == storename).Count() > 0)
            {
                ViewBag.CanDelete = "No";
                return Json(new { success = false, message = "Duplicate Store : " }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreateStore()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(x => x.Id == userId);
            ViewBag.SectionID = new SelectList(db.Sections, "SectionID", "sectionName");

            return View("CreateStore");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateStore (Store store)
        {
            {
                store.IsActive = true;
                store.Status = "Draft";
                db.Stores.Add(store);
                db.SaveChanges();
            }
            return RedirectToAction("IndexStore");
        }

        [EncryptedActionParameter]
        public ActionResult EditStore(int? WhouseID)
        {
            Store storerec = db.Stores.Find(WhouseID);
            if (WhouseID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (storerec == null)
            {
                return HttpNotFound();
            }
            ViewBag.SectionID = new SelectList(db.Sections, "SectionID", "SectionName", storerec.SectionID);

            return View("EditStore", storerec);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult EditStore([Bind(Include = "WhouseID, WhouseName, SectionID, Description, Status, IsActive, DateCreated, DateModified, UserCreated, UserModified")] Store store)
        {
            if (ModelState.IsValid)
            {
                var original_value = db.Stores.AsNoTracking().Where(P => P.WhouseID == store.WhouseID).FirstOrDefault();
                store.Status = original_value.Status;
                store.UserCreated = original_value.UserCreated;
                store.DateCreated = original_value.DateCreated;
                db.Entry(store).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.SectionID = new SelectList(db.Sections, "SectionID", "SectionName", store.SectionID);

                return RedirectToAction("IndexStore");
            }
            return View(store);
        }

            
        [EncryptedActionParameter]

        public ActionResult DetailStore(int? WhouseID)
        {
            Store storerec = db.Stores.Find(WhouseID);
            if (WhouseID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (storerec == null)
            {
                return HttpNotFound();
            }
            ViewBag.SectionID = new SelectList(db.Sections, "SectionID", "SectionName", storerec.SectionID);

            return View(storerec);
        }

        [EncryptedActionParameter]
        public ActionResult DeleteStore(int? WhouseID)
        {
            Store storerec = db.Stores.Find(WhouseID);
            if (WhouseID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (storerec == null)
            {
                return HttpNotFound();
            }
            ViewBag.SectionID = new SelectList(db.Sections, "SectionID", "SectionName", storerec.SectionID);
            if (db.PartTransferOuts.Where(x => x.ToStoreID == WhouseID).Count() > 0)
            {
                ViewBag.CanDelete = "No";
                return View(storerec);
            }

            return View(storerec);
        }

        [HttpPost, ActionName("DeleteStore")]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult DeleteStoreConfirmed(int WhouseID)
        {
            {
                Store storerec = db.Stores.Find(WhouseID);
                db.Stores.Remove(storerec);
                db.SaveChanges();
                ViewBag.SectionID = new SelectList(db.Sections, "SectionID", "SectionName", storerec.SectionID);

                return RedirectToAction("IndexStore");
            }
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

