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
using System.Web;
using System.Collections.Generic;
using Core.Entities.Data.PartRequest;

namespace AspNetGroupBasedPermissions.Areas.PartRequest.Controllers
{
    public class PartVendorController : Controller
    {
        private Infrastructures.Data.ApplicationDbContext db = new Infrastructures.Data.ApplicationDbContext();

        [EncryptedActionParameter]

        public ActionResult IndexPartVendor()
        {
            var DetailCD = db.PartVendors.Include(x => x.RefPartMasterID).Include(y => y.RefVendorID);
            ViewBag.Part = DetailCD.Select(x => x.RefPartMasterID.PartName).Distinct();
            ViewBag.Vendor = DetailCD.Select(x => x.RefVendorID.VendorName).Distinct();

            ViewBag.Status = DetailCD.Select(x => x.Status).Distinct();
            return View();
        }


        [HttpPost]
        public ActionResult GetPartVendorOld(string search, string searchpartno, string searchvendor, string searchitemno, int pageSize, int pageIndex, 
             string vendorname, string status)
        {
            try
            {
                var detailrec = (from a in db.PartVendors
                                 select new
                                 {
                                     PartVendorID = a.PartVendorID,
                                     PartName = a.RefPartMasterID.PartName,
                                     VendorName = a.RefVendorID.VendorName,
                                     PartNo = a.RefPartMasterID.PartNo,
                                     ItemNo = a.RefPartMasterID.ItemNo,
                                     Price = a.Price,
                                     Description = a.Description,
                                     Status = a.Status,
                                     IsActive = a.IsActive,
                                 }).OrderByDescending(c => c.PartVendorID);

                int totalRow = 0;
                int totalPage = 0;
                int skip = pageIndex * pageSize;

                var models = detailrec.Where(x => (x.PartName.Contains(search) && x.PartNo.Contains(searchpartno) && x.ItemNo.Contains(searchitemno) && x.VendorName.Contains(searchvendor))
                             && (x.VendorName == vendorname || "All" == vendorname)
                             && (x.Status == status ||  "All" == status))
                            .Skip(skip)
                            .Take(pageSize)
                            .ToList();
                totalRow = detailrec.Where(x => (x.PartName.Contains(search) && x.PartNo.Contains(searchpartno) && x.ItemNo.Contains(searchitemno) && x.VendorName.Contains(searchvendor))
                             && (x.VendorName == vendorname || "All" == vendorname)
                             && (x.Status == status || "All" == status)).Count();
                totalPage = (totalRow + pageSize - 1) / pageSize;
                totalPage = totalPage == 0 ? 1 : totalPage;

                var hocs = models == null ? null :
                            models.Select(a =>
                             new
                             {
                                 a.PartVendorID,
                                 a.PartName,
                                 a.VendorName,
                                 a.PartNo,
                                 a.ItemNo,
                                 a.Price,
                                 a.Description,
                                 a.Status,
                                 a.IsActive,
                                 ShowButton = true,
                                 DetailURL = EncyptURLHelper.EncodedActionUrl("DetailPartVendor", "PartVendor", new { PartMakerID = a.PartVendorID, area = "PartRequest" }),
                                 EditURL = EncyptURLHelper.EncodedActionUrl("EditPartVendor", "PartVendor", new { PartMakerID = a.PartVendorID, area = "PartRequest" }),
                             });
                return Json(new { hocs = hocs, totalPage = totalPage,  totalRow = totalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Console.WriteLine("Page Error. Contact System Administrator");
                throw;
            }
        }

        [HttpPost]
        public ActionResult GetPartVendor(string search, string searchpartno, string searchvendor, string searchitemno, int pageSize, int pageIndex,
             string vendorname, string status)
        {
            try
            {
                var detailrec = (from a in db.PartInvs
                                 select new
                                 {
                                     PartVendorID = a.PartInvID,
                                     PartName = a.RefPartMasterPartInvID.PartName,
                                     VendorName = a.RefVendorID.VendorName,
                                     PartNo = a.RefPartMasterPartInvID.PartNo,
                                     ItemNo = a.RefPartMasterPartInvID.ItemNo,
                                     Price = a.Price,
                                     Description = a.Description,
                                     Status = a.Status,
                                     IsActive = a.IsActive,
                                 }).OrderByDescending(c => c.PartVendorID);

                int totalRow = 0;
                int totalPage = 0;
                int skip = pageIndex * pageSize;

                var models = detailrec.Where(x => (x.PartName.Contains(search) && x.PartNo.Contains(searchpartno) && x.ItemNo.Contains(searchitemno) && x.VendorName.Contains(searchvendor))
                             && (x.VendorName == vendorname || "All" == vendorname)
                             && (x.Status == status || "All" == status))
                            .Skip(skip)
                            .Take(pageSize)
                            .ToList();
                totalRow = detailrec.Where(x => (x.PartName.Contains(search) && x.PartNo.Contains(searchpartno) && x.ItemNo.Contains(searchitemno) && x.VendorName.Contains(searchvendor))
                             && (x.VendorName == vendorname || "All" == vendorname)
                             && (x.Status == status || "All" == status)).Count();
                totalPage = (totalRow + pageSize - 1) / pageSize;
                totalPage = totalPage == 0 ? 1 : totalPage;

                var hocs = models == null ? null :
                            models.Select(a =>
                             new
                             {
                                 a.PartVendorID,
                                 a.PartName,
                                 a.VendorName,
                                 a.PartNo,
                                 a.ItemNo,
                                 a.Price,
                                 a.Description,
                                 a.Status,
                                 a.IsActive,
                                 ShowButton = true,
                                 DetailURL = EncyptURLHelper.EncodedActionUrl("DetailPartVendor", "PartVendor", new { PartMakerID = a.PartVendorID, area = "PartRequest" }),
                                 EditURL = EncyptURLHelper.EncodedActionUrl("EditPartVendor", "PartVendor", new { PartMakerID = a.PartVendorID, area = "PartRequest" }),
                             });
                return Json(new { hocs = hocs, totalPage = totalPage, totalRow = totalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Console.WriteLine("Page Error. Contact System Administrator");
                throw;
            }
        }

        [HttpPost]
        public ActionResult GetPartName(string partname, string partno, string filterby)
        {
            if (filterby == "partname")
            {
                var pcname = db.PartMasters.Where(x => (x.PartName.ToUpper().Contains(partname.Trim().ToUpper()) || partname == "") && x.IsActive == true).Select(x => new { x.PartName, x.PartID, x.PartNo }).ToList();
                return Json(pcname, JsonRequestBehavior.AllowGet);
            }
            if (filterby == "partno")
            {
                List<PartMaker> listPMaker = (from j in db.PartMakers
                                              where j.PartID == 2
                                              select j).ToList();
                PartMaker itemListmaker = listPMaker.FirstOrDefault();
                var makername = itemListmaker.RefMakerID.MakerName;

                List<PartInv> listPInv = (from j in db.PartInvs
                                          where j.PartID == 2
                                          select j).ToList();
                PartInv itemInv = listPInv.FirstOrDefault();
                var qty = itemInv.StockInv;

                var pcname = db.PartMasters.Where(x => x.PartNo == partno && x.IsActive == true).Select(x => new { x.PartName, x.PartID, x.PartNo }).ToList();
                return Json(pcname, JsonRequestBehavior.AllowGet);

            }

            return Json(JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GetVendorName(string vendorname,  string filterby)
        {
            if (filterby == "partname")
            {
                var pcname = db.Vendors.Where(x => (x.VendorName.ToUpper().Contains(vendorname.Trim().ToUpper()) || vendorname == "") && x.IsActive == true).Select(x => new {  x.VendorName, x.VendorID}).ToList();
                return Json(pcname, JsonRequestBehavior.AllowGet);
            }

            return Json(JsonRequestBehavior.AllowGet);
        }






        public ActionResult CreatePartVendor()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(x => x.Id == userId);
            ViewBag.PartID = new SelectList(db.PartMasters, "PartID", "PartName");
            ViewBag.VendorID = new SelectList(db.Vendors, "VendorID", "VendorName");

            return View("CreatePartVendor");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePartVendor(PartVendor partvendor)
        {
            partvendor.IsActive = true;
            partvendor.Status = "Draft";
            db.PartVendors.Add(partvendor);
            db.SaveChanges();
              
            return RedirectToAction("IndexPartVendor");
        }


        [EncryptedActionParameter]
        public async Task<ActionResult> EditPartVendor(int? PartVendorID)
        {
            PartVendor partvendrec = await db.PartVendors.FindAsync(PartVendorID);
            if (PartVendorID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (partvendrec == null)
            {
                return HttpNotFound();
            }
            ViewBag.PartID = new SelectList(db.PartMasters, "PartID", "PartName", partvendrec.PartID);
            ViewBag.VendorID = new SelectList(db.Vendors, "VendorID", "VendorName", partvendrec.VendorID);

            return View("EditPartVendor", partvendrec);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult EditPartVendor([Bind(Include = "PartVendorID, PartID, VendorID, Price , Description, Status, IsActive, DateCreated, DateModified, UserCreated, UserModified")] PartVendor partvendor)
        {
            if (ModelState.IsValid)
            {
                var original_value = db.PartVendors.AsNoTracking().Where(P => P.PartID == partvendor.PartID).FirstOrDefault();
                partvendor.Status = original_value.Status;
                partvendor.UserCreated = original_value.UserCreated;
                partvendor.DateCreated = original_value.DateCreated;
                db.Entry(partvendor).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.PartID = new SelectList(db.PartMasters, "PartID", "PartName", partvendor.PartID);
                ViewBag.VendorID = new SelectList(db.Vendors, "VendorID", "VendorName", partvendor.VendorID);

                return RedirectToAction("IndexPartVendor");
            }
            return View(partvendor);
        }

            
        [EncryptedActionParameter]
        public ActionResult DetailPartVendor(int? PartVendorID)
        {
            PartInv partvendorrec = db.PartInvs.Find(PartVendorID);
            if (PartVendorID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (partvendorrec == null)
            {
                return HttpNotFound();
            }
            ViewBag.PartID = new SelectList(db.PartMasters, "PartID", "PartName", partvendorrec.PartID);
            ViewBag.VendorID = new SelectList(db.Vendors, "VendorID", "VendorName", partvendorrec.VendorID);

            return View(partvendorrec);
        }


        [EncryptedActionParameter]
        public ActionResult DetailPartVendorOld(int? PartVendorID)
        {
            PartVendor partvendorrec = db.PartVendors.Find(PartVendorID);
            if (PartVendorID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (partvendorrec == null)
            {
                return HttpNotFound();
            }
            ViewBag.PartID = new SelectList(db.PartMasters, "PartID", "PartName", partvendorrec.PartID);
            ViewBag.VendorID = new SelectList(db.Vendors, "VendorID", "VendorName", partvendorrec.VendorID);

            return View(partvendorrec);
        }

        [EncryptedActionParameter]
        public ActionResult DeletePartMaker(int? PartMakerID)
        {
            PartMaker partmakerrec = db.PartMakers.Find(PartMakerID);
            if (PartMakerID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (partmakerrec == null)
            {
                return HttpNotFound();
            }
            ViewBag.PartID = new SelectList(db.PartMasters, "PartID", "PartName", partmakerrec.PartID);
            ViewBag.MakerID = new SelectList(db.Makers, "MakerID", "MakerName", partmakerrec.MakerID);

            //if (db.QuaRec.Where(x => x.PartMakerID == PartMakerID).Count() > 0)
            //{
            //    ViewBag.CanDelete = "No";
            //    return View(partmakerrec);
            //}

            return View(partmakerrec);
        }

        [HttpPost, ActionName("DeletePartMaker")]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult DeletePartMakerConfirmed(int PartMakerID)
        {
            {
                PartMaker partmakerrec = db.PartMakers.Find(PartMakerID);
                db.PartMakers.Remove(partmakerrec);
                db.SaveChanges();
                ViewBag.PartID = new SelectList(db.PartMasters, "PartID", "PartName", partmakerrec.PartID);
                ViewBag.MakerID = new SelectList(db.Makers, "MakerID", "MakerName", partmakerrec.MakerID);

                return RedirectToAction("IndexPartMaker");
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

