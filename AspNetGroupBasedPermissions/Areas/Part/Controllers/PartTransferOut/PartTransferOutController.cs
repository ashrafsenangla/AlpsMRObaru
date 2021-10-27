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

namespace AspNetGroupBasedPermissions.Areas.Part.Controllers
{
    public class PartTransferOutController : Controller
    {
        private Infrastructures.Data.ApplicationDbContext db = new Infrastructures.Data.ApplicationDbContext();

        [EncryptedActionParameter]
        public ActionResult IndexPartTransferOut()
        {
            var DetailCD = db.PartTransferOuts.Include(x => x.RefPartID).Include(y => y.RefCabinetID).Include(y => y.RefStoreID);
            ViewBag.Part = DetailCD.Select(x => x.RefPartID.PartName).Distinct();
            ViewBag.FromCabinet = DetailCD.Select(x => x.RefCabinetID.CabinetName).Distinct();
            ViewBag.Store = DetailCD.Select(x => x.RefStoreID.WhouseName).Distinct();
            ViewBag.Status = DetailCD.Select(x => x.Status).Distinct();
            ViewBag.IsActive = DetailCD.Select(x => x.IsActive == true ? "Yes" : "No").Distinct();
            return View();
        }

        [EncryptedActionParameter]
        public ActionResult IndexPartTransferOutInternal()
        {
            var DetailCD = db.PartTransferOuts.Include(x => x.RefPartID).Include(y => y.RefCabinetID).Include(y => y.RefStoreID);
            ViewBag.Part = DetailCD.Select(x => x.RefPartID.PartName).Distinct();
            ViewBag.FromCabinet = DetailCD.Select(x => x.RefCabinetID.CabinetName).Distinct();
            ViewBag.Store = DetailCD.Select(x => x.RefStoreID.WhouseName).Distinct();
            ViewBag.Status = DetailCD.Select(x => x.Status).Distinct();
            ViewBag.IsActive = DetailCD.Select(x => x.IsActive == true ? "Yes" : "No").Distinct();
            return View();
        }

        [HttpPost]
        public ActionResult GetPartTransferOut(string search, int pageSize, int pageIndex, 
             string storename, string cabinetname, string partname, string status, string isactive)
        {
            try
            {
                var detailrec = (from a in db.PartTransferOuts
                                 select new
                                 {
                                     PartTransferOutID = a.PartTransferOutID,
                                     PartName = a.RefPartID.PartName,
                                     Quantity = a.Qty,
                                     CabinetName = a.RefCabinetID.CabinetName,
                                     StoreName = a.RefStoreID.WhouseName,
                                     Description = a.Description,
                                     Status = a.Status,
                                     IsActive = a.IsActive == true ? "Yes" : "No",
                                 }).OrderByDescending(c => c.PartTransferOutID);

                int totalRow = 0;
                int totalPage = 0;
                int skip = pageIndex * pageSize;

                var models = detailrec.Where(x => (x.PartName.Contains(search))
                             && (x.Status == status ||  "All" == status)
                             && (x.IsActive == isactive || "All" == isactive)
                             && (x.PartName == partname || "All" == partname)
                             && (x.CabinetName == cabinetname || "All" == cabinetname)
                             && (x.StoreName == storename || "All" == storename))
                            .Skip(skip)
                            .Take(pageSize)
                            .ToList();
                totalRow = detailrec.Where(x => (x.Status.Contains(search))
                             && (x.Status == status || "All" == status)
                             && (x.IsActive == isactive || "All" == isactive)
                             && (x.PartName == partname || "All" == partname)
                             && (x.CabinetName == cabinetname || "All" == cabinetname)
                             && (x.StoreName == storename || "All" == storename)).Count();

                totalPage = (totalRow + pageSize - 1) / pageSize;
                totalPage = totalPage == 0 ? 1 : totalPage;

                var hocs = models == null ? null :
                            models.Select(a =>
                             new
                             {
                                 a.PartTransferOutID,
                                 a.PartName,
                                 a.Quantity,
                                 a.CabinetName,
                                 a.StoreName,
                                 a.Description,
                                 a.Status,
                                 a.IsActive,
                                 ShowButton = true,
                                 DetailURL = EncyptURLHelper.EncodedActionUrl("DetailPartTransferOut", "PartTransferOut", new { PartTransferOutID = a.PartTransferOutID, area = "PartTransferOut" }),
                                 EditURL = EncyptURLHelper.EncodedActionUrl("EditPartTransferOut", "PartTransferOut", new { PartTransferOutID = a.PartTransferOutID, area = "PartTransferOut" }),
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
        public ActionResult GetPartTransferOutInternal(string search, int pageSize, int pageIndex,
             string storename, string cabinetname, string partname, string status, string isactive)
        {
            try
            {
                var detailrec = (from a in db.PartTransferOuts
                                 select new
                                 {
                                     PartTransferOutID = a.PartTransferOutID,
                                     PartName = a.RefPartID.PartName,
                                     Quantity = a.Qty,
                                     CabinetName = a.RefCabinetID.CabinetName,
                                     StoreName = a.RefStoreID.WhouseName,
                                     Description = a.Description,
                                     Status = a.Status,
                                     IsActive = a.IsActive == true ? "Yes" : "No",
                                 }).OrderByDescending(c => c.PartTransferOutID);

                int totalRow = 0;
                int totalPage = 0;
                int skip = pageIndex * pageSize;

                var models = detailrec.Where(x => (x.PartName.Contains(search))
                             && (x.Status == status || "All" == status)
                             && (x.IsActive == isactive || "All" == isactive)
                             && (x.PartName == partname || "All" == partname)
                             && (x.CabinetName == cabinetname || "All" == cabinetname)
                             && (x.StoreName == storename || "All" == storename))
                            .Skip(skip)
                            .Take(pageSize)
                            .ToList();
                totalRow = detailrec.Where(x => (x.Status.Contains(search))
                             && (x.Status == status || "All" == status)
                             && (x.IsActive == isactive || "All" == isactive)
                             && (x.PartName == partname || "All" == partname)
                              && (x.CabinetName == cabinetname || "All" == cabinetname)
                             && (x.StoreName == storename || "All" == storename)).Count();

                totalPage = (totalRow + pageSize - 1) / pageSize;
                totalPage = totalPage == 0 ? 1 : totalPage;

                var hocs = models == null ? null :
                            models.Select(a =>
                             new
                             {
                                 a.PartTransferOutID,
                                 a.PartName,
                                 a.Quantity,
                                 a.CabinetName,
                                 a.StoreName,
                                 a.Description,
                                 a.Status,
                                 a.IsActive,
                                 ShowButton = true,
                                 DetailURL = EncyptURLHelper.EncodedActionUrl("DetailPartTransferOut", "PartTransferOut", new { PartTransferOutID = a.PartTransferOutID, area = "PartTransferOut" }),
                                 EditURL = EncyptURLHelper.EncodedActionUrl("EditPartTransferOut", "PartTransferOut", new { PartTransferOutID = a.PartTransferOutID, area = "PartTransferOut" }),
                             });
                return Json(new { hocs = hocs, totalPage = totalPage, totalRow = totalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Console.WriteLine("Page Error. Contact System Administrator");
                throw;
            }
        }


        public ActionResult CreatePartTransferOut()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(x => x.Id == userId);

            var username = User.Identity.GetUserName();
            var query = (from a in db.UserBizUnits
                         join b in db.Sections
                         on a.BUID equals b.BUID
                         where a.UserId == userId
                         && b.IsActive == true
                         select new
                         {
                             b.SectionID,
                             b.SectionName
                         }).ToList();

            //            ViewBag.SectionID = query.Select(x => x.SectionName).Distinct();

            ViewBag.PlantID = new SelectList(db.Plants.Where(x => x.IsActive == true), "PlantID", "PlantName");
            ViewBag.BUID = new SelectList(db.BusinessUnits.Where(x => x.IsActive == true), "BUID", "BusinessUnitName");
            ViewBag.CostCentreID = new SelectList(db.CostCentres.Where(x => x.IsActive == true), "CostCentreID", "CostCentreName");
            ViewBag.SectionID = new SelectList(db.Sections.Where(x => x.IsActive == true), "SectionID", "SectionName");
            ViewBag.StoreID = new SelectList(db.Stores.Where(x => x.IsActive == true), "WhouseID", "WhouseName");
            ViewBag.FromCabinetID = new SelectList(db.Cabinets.Where(x => x.IsActive == true), "CabinetID", "CabinetName");

            ViewBag.ToStoreID = new SelectList(db.Stores.Where(x => x.IsActive==true), "WhouseID", "WhouseName");

            //    var pcname = db.PartMasters.Where(x => (x.PartName.ToUpper().Contains(partname.Trim().ToUpper()) || partname == "") && x.IsActive == true).Select(x => new { PartName = "(" + x.PartNo + ")-" + x.PartName, x.PartID, x.PartNo }).ToList();

            ViewBag.PartID = new SelectList(db.PartMasters.Where(x => x.IsActive == true), "PartID", "PartName");
            ViewBag.CategoryID = new SelectList(db.Categorys.Where(x => x.IsActive == true), "CategoryID", "CategoryName");

            return View("CreatePartTransferOut");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePartTransferOut(PartTransferOut PartTransferOut)
        {
            PartTransferOut.IsActive = true;
            PartTransferOut.Status = "Draft";
            PartTransferOut.TransferType = "Charge Out";
            PartTransferOut.ToStoreID = 1;
            db.PartTransferOuts.Add(PartTransferOut);
            db.SaveChanges();
              
            return RedirectToAction("IndexPartTransferOut");
        }


        public ActionResult CreatePartTransferOutInternal()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(x => x.Id == userId);
            ViewBag.ToStoreID = new SelectList(db.Stores.Where(x => x.IsActive == true), "WhouseID", "WhouseName");
            ViewBag.PartID = new SelectList(db.PartMasters.Where(x => x.IsActive == true), "PartID", "PartName");
            ViewBag.CabinetID = new SelectList(db.Cabinets.Where(x => x.IsActive == true), "CabinetID", "CabinetName");

            return View("CreatePartTransferOut");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePartTransferOutInternal(PartTransferOut PartTransferOut)
        {
            PartTransferOut.IsActive = true;
            PartTransferOut.Status = "Draft";
            PartTransferOut.TransferType = "Internal";
            db.PartTransferOuts.Add(PartTransferOut);
            db.SaveChanges();

            return RedirectToAction("IndexPartTransferOut");
        }



        [EncryptedActionParameter]
        public async Task<ActionResult> EditPartTransferOut(int? PartTransferOutID)
        {
            PartTransferOut parttransferoutrec = await db.PartTransferOuts.FindAsync(PartTransferOutID);
            if (PartTransferOutID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (parttransferoutrec == null)
            {
                return HttpNotFound();
            }
            ViewBag.ToStoreID = new SelectList(db.Stores.Where(x => x.IsActive == true), "WhouseID", "WhouseName", parttransferoutrec.ToStoreID);
            ViewBag.PartID = new SelectList(db.PartMasters.Where(x => x.IsActive == true), "PartID", "PartName", parttransferoutrec.PartID);
            ViewBag.CabinetID = new SelectList(db.Cabinets.Where(x => x.IsActive == true), "CabinetID", "CabinetName", parttransferoutrec.FromCabinetID);

            return View("EditPartTransferOut", parttransferoutrec);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult EditPartTransferOut([Bind(Include = "PartTransferOutID, PartID, ToStoreID, Qty , FromCabinetID, Description, Status, IsActive, DateCreated, DateModified, UserCreated, UserModified")] PartTransferOut PartTransferOut)
        {
            if (ModelState.IsValid)
            {
                var original_value = db.PartTransferOuts.AsNoTracking().Where(P => P.PartTransferOutID == PartTransferOut.PartTransferOutID).FirstOrDefault();
                PartTransferOut.Status = original_value.Status;
                PartTransferOut.TransferType = original_value.TransferType;
                PartTransferOut.UserCreated = original_value.UserCreated;
                PartTransferOut.DateCreated = original_value.DateCreated;
                db.Entry(PartTransferOut).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("IndexPartTransferOut");
            }
            return View(PartTransferOut);
        }


        [EncryptedActionParameter]
        public async Task<ActionResult> EditPartTransferOutInternal(int? PartTransferOutID)
        {
            PartTransferOut parttransferoutrec = await db.PartTransferOuts.FindAsync(PartTransferOutID);
            if (PartTransferOutID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (parttransferoutrec == null)
            {
                return HttpNotFound();
            }
            ViewBag.ToStoreID = new SelectList(db.Stores.Where(x => x.IsActive == true), "WhouseID", "WhouseName", parttransferoutrec.ToStoreID);
            ViewBag.PartID = new SelectList(db.PartMasters.Where(x => x.IsActive == true), "PartID", "PartName", parttransferoutrec.PartID);
            ViewBag.FromCabinetID = new SelectList(db.Cabinets.Where(x => x.IsActive == true), "CabinetID", "CabinetName", parttransferoutrec.FromCabinetID);

            return View("EditPartTransferOut", parttransferoutrec);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult EditPartTransferOutInternal([Bind(Include = "PartTransferOutID, PartID, ToStoreID, Qty , FromCabinetID, Description, Status, IsActive, DateCreated, DateModified, UserCreated, UserModified")] PartTransferOut PartTransferOut)
        {
            if (ModelState.IsValid)
            {
                var original_value = db.PartTransferOuts.AsNoTracking().Where(P => P.PartTransferOutID == PartTransferOut.PartTransferOutID).FirstOrDefault();
                PartTransferOut.Status = original_value.Status;
                PartTransferOut.TransferType = original_value.TransferType;
                PartTransferOut.UserCreated = original_value.UserCreated;
                PartTransferOut.DateCreated = original_value.DateCreated;
                db.Entry(PartTransferOut).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("IndexPartTransferOut");
            }
            return View(PartTransferOut);
        }


        [EncryptedActionParameter]

        public ActionResult DetailPartTransferOut(int? PartTransferOutID)
        {
            PartTransferOut parttransferoutrec = db.PartTransferOuts.Find(PartTransferOutID);
            if (PartTransferOutID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (parttransferoutrec == null)
            {
                return HttpNotFound();
            }
            ViewBag.ToStoreID = new SelectList(db.Stores, "WhouseID", "WhouseName", parttransferoutrec.ToStoreID);
            ViewBag.PartID = new SelectList(db.PartMasters, "PartID", "PartName", parttransferoutrec.PartID);
            ViewBag.FromCabinetID = new SelectList(db.Cabinets, "CabinetID", "CabinetName", parttransferoutrec.FromCabinetID);

            return View(parttransferoutrec);
        }

        [EncryptedActionParameter]

        public ActionResult DetailPartTransferOutInternal(int? PartTransferOutID)
        {
            PartTransferOut parttransferoutrec = db.PartTransferOuts.Find(PartTransferOutID);
            if (PartTransferOutID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (parttransferoutrec == null)
            {
                return HttpNotFound();
            }
            ViewBag.ToStoreID = new SelectList(db.Stores, "WhouseID", "WhouseName", parttransferoutrec.ToStoreID);
            ViewBag.PartID = new SelectList(db.PartMasters, "PartID", "PartName", parttransferoutrec.PartID);
            ViewBag.FromCabinetID = new SelectList(db.Cabinets, "CabinetID", "CabinetName", parttransferoutrec.FromCabinetID);

            return View(parttransferoutrec);
        }

        [EncryptedActionParameter]
        public ActionResult DeletePartTransferOut(int? PartTransferOutID)
        {
            PartTransferOut parttransferoutrec = db.PartTransferOuts.Find(PartTransferOutID);
            if (PartTransferOutID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (parttransferoutrec == null)
            {
                return HttpNotFound();
            }
            ViewBag.ToStoreID = new SelectList(db.Stores, "WhouseID", "WhouseName", parttransferoutrec.ToStoreID);
            ViewBag.PartID = new SelectList(db.PartMasters, "PartID", "PartName", parttransferoutrec.PartID);
            ViewBag.FromCabinetID = new SelectList(db.Cabinets, "CabinetID", "CabinetName", parttransferoutrec.FromCabinetID);

            return View(parttransferoutrec);
        }

        [HttpPost, ActionName("DeletePartTransferOut")]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult DeletePartTransferOutConfirmed(int PartTransferOutID)
        {
            {
                PartTransferOut parttransferoutrec = db.PartTransferOuts.Find(PartTransferOutID);
                db.PartTransferOuts.Remove(parttransferoutrec);
                db.SaveChanges();
                return RedirectToAction("IndexPartTransferOut");
            }
        }

        [EncryptedActionParameter]
        public ActionResult DeletePartTransferOutInternal(int? PartTransferOutID)
        {
            PartTransferOut parttransferoutrec = db.PartTransferOuts.Find(PartTransferOutID);
            if (PartTransferOutID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (parttransferoutrec == null)
            {
                return HttpNotFound();
            }
            ViewBag.ToStoreID = new SelectList(db.Stores, "WhouseID", "WhouseName", parttransferoutrec.ToStoreID);
            ViewBag.PartID = new SelectList(db.PartMasters, "PartID", "PartName", parttransferoutrec.PartID);
            ViewBag.FromCabinetID = new SelectList(db.Cabinets, "CabinetID", "CabinetName", parttransferoutrec.FromCabinetID);

            return View(parttransferoutrec);
        }

        [HttpPost, ActionName("DeletePartTransferOut")]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult DeletePartTransferOutInternalConfirmed(int PartTransferOutID)
        {
            {
                PartTransferOut parttransferoutrec = db.PartTransferOuts.Find(PartTransferOutID);
                db.PartTransferOuts.Remove(parttransferoutrec);
                db.SaveChanges();
                return RedirectToAction("IndexPartTransferOut");
            }
        }


        public ActionResult GetPartInfo(int strSelected)
        {
            List<PartMaster> listPM = (from j in db.PartMasters
                                       where j.PartID == strSelected
                                       select j).ToList();
            PartMaster itemList = listPM.FirstOrDefault();
            var partNo = itemList.PartNo;

            List<PartMaker> listPMaker = (from j in db.PartMakers
                                          where j.PartID == strSelected
                                          select j).ToList();
            PartMaker itemListmaker = listPMaker.FirstOrDefault();
            var makername = itemListmaker.RefMakerID.MakerName;

            List<PartInv> listPInv = (from j in db.PartInvs
                                      where j.PartID == strSelected
                                      select j).ToList();
            PartInv itemInv = listPInv.FirstOrDefault();
            var qty = itemInv.StockInv;

            return Json(new { success = true, qty = qty, makername = makername, partno = partNo, message = "Success : " + strSelected }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult GetPartInfo2(string strSelected)
        {
            List<PartMaker> listPMaker = (from j in db.PartMakers
                                          where j.RefPartMasterID.PartNo == strSelected
                                          select j).ToList();
            PartMaker itemListmaker = listPMaker.FirstOrDefault();
            var makername = itemListmaker.RefMakerID.MakerName;

            List<PartInv> listPInv = (from j in db.PartInvs
                                      where j.RefPartMasterPartInvID.PartNo == strSelected
                                      select j).ToList();
            PartInv itemInv = listPInv.FirstOrDefault();
            var qty = itemInv.StockInv;

            return Json(new { success = true, qty = qty, makername = makername,  message = "Success : " + strSelected }, JsonRequestBehavior.AllowGet);

        }



        [HttpPost]
        public ActionResult GetPartName(string partname, string partno ,string filterby)
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

            return Json( JsonRequestBehavior.AllowGet);
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

