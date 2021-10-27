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
using Core.Entities.Data.PartRequest;

namespace AspNetGroupBasedPermissions.Areas.PartRequest.Controllers
{
    public class VendorController : Controller
    {
        private Infrastructures.Data.ApplicationDbContext db = new Infrastructures.Data.ApplicationDbContext();

        [EncryptedActionParameter]

        public ActionResult IndexVendor()
        {
            var DetailCD = db.Vendors;
            ViewBag.Status = DetailCD.Select(x => x.Status).Distinct();
            return View();
        }


        [HttpPost]
        public ActionResult GetVendor(string search, int pageSize, int pageIndex, string status)
        {
            try
            {
                var detailrec = (from a in db.Vendors
                                 select new
                                 {
                                     VendorID = a.VendorID,
                                     VendorNo = a.VendorNo,
                                     VendorName = a.VendorName,
                                     Description = a.Description,
                                     Status = a.Status,
                                     IsActive = a.IsActive,
                                 }).OrderByDescending(c => c.VendorID);

                int totalRow = 0;
                int totalPage = 0;
                int skip = pageIndex * pageSize;

                var models = detailrec.Where(x => (x.VendorName.Contains(search))
                             && (x.Status == status ||  "All" == status))
                            .Skip(skip)
                            .Take(pageSize)
                            .ToList();
                totalRow = detailrec.Where(x => (x.VendorName.Contains(search))
                            && (x.Status == status || "All" == status)).Count();
           
                totalPage = (totalRow + pageSize - 1) / pageSize;
                totalPage = totalPage == 0 ? 1 : totalPage;

                var hocs = models == null ? null :
                            models.Select(a =>
                             new
                             {
                                 a.VendorID,
                                 a.VendorNo,
                                 a.VendorName,
                                 a.Description,
                                 a.Status,
                                 a.IsActive,
                                 ShowButton = true,
                                 DetailURL = EncyptURLHelper.EncodedActionUrl("DetailVendor", "Vendor", new { VendorID = a.VendorID, area = "Part" }),
                                 EditURL = EncyptURLHelper.EncodedActionUrl("EditVendor", "Vendor", new { VendorID = a.VendorID, area = "Part" }),
                             });
                return Json(new { hocs = hocs, totalPage = totalPage,  totalRow = totalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Console.WriteLine("Page Error. Contact System Administrator");
                throw;
            }
        }


        public ActionResult CheckDuplicate(string vendorno)
        {

            if (db.Vendors.Where(x => x.VendorNo == vendorno).Count() > 0)
            {
                ViewBag.CanDelete = "No";
                return Json(new { success = false, message = "Duplicate Vendor : " }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CreateVendor()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(x => x.Id == userId);
            return View("CreateVendor");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateVendor (Vendor vendor)
        {
            {
                vendor.IsActive = true;
                vendor.Status = "Draft";
                db.Vendors.Add(vendor);
                db.SaveChanges();
            }
            return RedirectToAction("IndexVendor");
        }

        [EncryptedActionParameter]
        public async Task<ActionResult> EditVendor(int? VendorID)
        {
            Vendor vendor = await db.Vendors.FindAsync(VendorID);
            if (VendorID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (VendorID == null)
            {
                return HttpNotFound();
            }
            return View("EditVendor", vendor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult EditVendor([Bind(Include = "VendorID, VendorNo, AccountGroup, VendorName, SAPVendorCode, PhoneNo, Email, Description, Status, IsActive, DateCreated, DateModified, UserCreated, UserModified")] Vendor vendor)
        {
            if (ModelState.IsValid)
            {
                var original_value = db.Vendors.AsNoTracking().Where(P => P.VendorID == vendor.VendorID).FirstOrDefault();
                vendor.Status = original_value.Status;
                vendor.UserCreated = original_value.UserCreated;
                vendor.DateCreated = original_value.DateCreated;
                db.Entry(vendor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexVendor");
            }
            return View(vendor);
        }

            
        [EncryptedActionParameter]

        public ActionResult DetailVendor(int? VendorID)
        {
            Vendor vendorrec = db.Vendors.Find(VendorID);
            if (VendorID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (vendorrec == null)
            {
                return HttpNotFound();
            }
            return View(vendorrec);
        }

        [EncryptedActionParameter]
        public ActionResult DeleteVendor(int? VendorID)
        {
            Vendor vendorrecs = db.Vendors.Find(VendorID);
            if (VendorID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (vendorrecs == null)
            {
                return HttpNotFound();
            }

            return View(vendorrecs);
        }

        [HttpPost, ActionName("DeleteVendor")]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult DeleteVendorConfirmed(int VendorID)
        {
            {
                Vendor vendorrecs = db.Vendors.Find(VendorID);
                db.Vendors.Remove(vendorrecs);
                db.SaveChanges();
                return RedirectToAction("IndexVendor");
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

