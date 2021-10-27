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
using Core.Entities.Data.Admin;

namespace AspNetGroupBasedPermissions.Areas.Part.Controllers
{
    public class BusinessUnitController : Controller
    {
        private Infrastructures.Data.ApplicationDbContext db = new Infrastructures.Data.ApplicationDbContext();

        [EncryptedActionParameter]

        public ActionResult IndexBusinessUnit()
        {
            var DetailCD = db.BusinessUnits;
            ViewBag.Status = DetailCD.Select(x => x.Status).Distinct();
            return View();
        }


        [HttpPost]
        public ActionResult GetBusinessUnit(string search, int pageSize, int pageIndex, string status)
        {
            try
            {
                var detailrec = (from a in db.BusinessUnits
                                 select new
                                 {
                                     BUID = a.BUID,
                                     BUName = a.BusinessUnitName,
                                     Description = a.Description,
                                     Status = a.Status,
                                     IsActive = a.IsActive,
                                 }).OrderByDescending(c => c.BUID);

                int totalRow = 0;
                int totalPage = 0;
                int skip = pageIndex * pageSize;

                var models = detailrec.Where(x => (x.BUName.Contains(search))
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
                                 a.BUID,
                                 a.BUName,
                                 a.Description,
                                 a.Status,
                                 a.IsActive,
                                 ShowButton = true,
                                 DetailURL = EncyptURLHelper.EncodedActionUrl("DetailBusinessUnit", "BusinessUnit", new { ID = a.BUID, area = "Part" }),
                                 EditURL = EncyptURLHelper.EncodedActionUrl("EditBusinessUnit", "BusinessUnit", new { ID = a.BUID, area = "Part" }),
                             });
                return Json(new { hocs = hocs, totalPage = totalPage,  totalRow = totalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Console.WriteLine("Page Error. Contact System Administrator");
                throw;
            }
        }


        public ActionResult CheckDuplicate(string bizunit)
        {

            if (db.BusinessUnits.Where(x => x.BusinessUnitName == bizunit).Count() > 0)
            {
                ViewBag.CanDelete = "No";
                return Json(new { success = false, message = "Duplicate Biz Unit : " }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CreateBusinessUnit()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(x => x.Id == userId);
            return View("CreateBusinessUnit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBusinessUnit (BusinessUnit bunit)
        {
            {
                bunit.IsActive = true;
                bunit.Status = "Draft";
                db.BusinessUnits.Add(bunit);
                db.SaveChanges();
            }
            return RedirectToAction("IndexBusinessUnit");
        }

        [EncryptedActionParameter]
        public ActionResult EditBusinessUnit(int? BUID)
        {
            BusinessUnit bunit = db.BusinessUnits.Find(BUID);
            if (BUID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (BUID == null)
            {
                return HttpNotFound();
            }
            return View("EditBusinessUnit", bunit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult EditBusinessUnit([Bind(Include = "BUID, BusinessUnitName, Description, Status, IsActive, DateCreated, DateModified, UserCreated, UserModified")] BusinessUnit bunit)
        {
            if (ModelState.IsValid)
            {
                var original_value = db.BusinessUnits.AsNoTracking().Where(P => P.BUID == bunit.BUID).FirstOrDefault();
                bunit.Status = original_value.Status;
                bunit.UserCreated = original_value.UserCreated;
                bunit.DateCreated = original_value.DateCreated;
                db.Entry(bunit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexBusinessUnit");
            }
            return View(bunit);
        }

            
        [EncryptedActionParameter]

        public ActionResult DetailBusinessUnit(int? BUID)
        {
            BusinessUnit bunitrec = db.BusinessUnits.Find(BUID);
            if (BUID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (bunitrec == null)
            {
                return HttpNotFound();
            }
            return View(bunitrec);
        }

        [EncryptedActionParameter]
        public ActionResult DeleteBusinessUnit(int? BUID)
        {
            BusinessUnit bunitrecs = db.BusinessUnits.Find(BUID);
            if (BUID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (bunitrecs == null)
            {
                return HttpNotFound();
            }
            if (db.Sections.Where(x => x.BUID == BUID).Count() > 0)
            {
                ViewBag.CanDelete = "No";
                return View(bunitrecs);
            }

            return View(bunitrecs);
        }

        [HttpPost, ActionName("DeleteBusinessUnit")]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult DeleteBusinessUnitConfirmed(int BUID)
        {
            {
                BusinessUnit bunitrecs = db.BusinessUnits.Find(BUID);
                db.BusinessUnits.Remove(bunitrecs);
                db.SaveChanges();
                return RedirectToAction("IndexBusinessUnit");
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

