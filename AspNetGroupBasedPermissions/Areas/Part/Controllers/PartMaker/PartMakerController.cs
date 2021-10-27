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
    public class PartMakerController : Controller
    {
        private Infrastructures.Data.ApplicationDbContext db = new Infrastructures.Data.ApplicationDbContext();

        [EncryptedActionParameter]

        public ActionResult IndexPartMaker()
        {
            var DetailCD = db.PartMakers.Include(x => x.RefPartMasterID).Include(y => y.RefMakerID);
            ViewBag.Part = DetailCD.Where(x=> x.RefPartMasterID.PartName != "PUNCH (COVER)").Select(x => x.RefPartMasterID.PartName).Distinct();
            ViewBag.Maker = DetailCD.Select(x => x.RefMakerID.MakerName).Distinct();

            ViewBag.Status = DetailCD.Select(x => x.Status).Distinct();
            return View();
        }


        [HttpPost]
        public ActionResult GetPartMakerOld(string search, string searchpartno, string searchitemno, int pageSize, int pageIndex, 
             string partname, string makername, string status)
        {
            try
            {
                var detailrec = (from a in db.PartMakers
                                 select new
                                 {
                                     PartMakerID = a.PartMakerID,
                                     PartName = a.RefPartMasterID.PartName,
                                     PartNo = a.RefPartMasterID.PartNo,
                                     ItemNo = a.RefPartMasterID.ItemNo,
                                     MakerName = a.RefMakerID.MakerName,
                                     Description = a.Description,
                                     Status = a.Status,
                                     IsActive = a.IsActive,
                                 }).OrderByDescending(c => c.PartMakerID);

                int totalRow = 0;
                int totalPage = 0;
                int skip = pageIndex * pageSize;

                var models = detailrec.Where(x => (x.PartName.Contains(search) && x.PartNo.Contains(searchpartno) && x.ItemNo.Contains(searchitemno))
                             && (x.MakerName == makername || "All" == makername)
                             && (x.Status == status ||  "All" == status))
                            .Skip(skip)
                            .Take(pageSize)
                            .ToList();
                totalRow = detailrec.Where(x => (x.PartName.Contains(search) && x.PartNo.Contains(searchpartno) && x.ItemNo.Contains(searchitemno))
                            && (x.MakerName == makername || "All" == makername)
                            && (x.Status == status || "All" == status)).Count();
           
                totalPage = (totalRow + pageSize - 1) / pageSize;
                totalPage = totalPage == 0 ? 1 : totalPage;

                var hocs = models == null ? null :
                            models.Select(a =>
                             new
                             {
                                 a.PartMakerID,
                                 a.PartName,
                                 a.MakerName,
                                 a.PartNo,
                                 a.ItemNo,
                                 a.Description,
                                 a.Status,
                                 a.IsActive,
                                 ShowButton = true,
                                 DetailURL = EncyptURLHelper.EncodedActionUrl("DetailPartMaker", "PartMaker", new { PartMakerID = a.PartMakerID, area = "PartMaker" }),
                                 EditURL = EncyptURLHelper.EncodedActionUrl("EditPartMaker", "PartMaker", new { PartMakerID = a.PartMakerID, area = "PartMaker" }),
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
        public ActionResult GetPartMaker(string search, string searchpartno, string searchitemno, int pageSize, int pageIndex,
             string partname, string makername, string status)
        {
            try
            {
                var detailrec = (from a in db.PartInvs
                                 select new
                                 {
                                     PartMakerID = a.PartID,
                                     PartName = a.RefPartMasterPartInvID.PartName,
                                     PartNo = a.RefPartMasterPartInvID.PartNo,
                                     ItemNo = a.RefPartMasterPartInvID.ItemNo,
                                     MakerName = a.RefMakerID.MakerName,
                                     Description = a.Description,
                                     Status = a.Status,
                                     IsActive = a.IsActive,
                                 }).OrderByDescending(c => c.PartMakerID);

                int totalRow = 0;
                int totalPage = 0;
                int skip = pageIndex * pageSize;

                var models = detailrec.Where(x => (x.PartName.Contains(search) && x.PartNo.Contains(searchpartno) && x.ItemNo.Contains(searchitemno))
                             && (x.MakerName == makername || "All" == makername)
                             && (x.Status == status || "All" == status))
                            .Skip(skip)
                            .Take(pageSize)
                            .ToList();
                totalRow = detailrec.Where(x => (x.PartName.Contains(search) && x.PartNo.Contains(searchpartno) && x.ItemNo.Contains(searchitemno))
                            && (x.MakerName == makername || "All" == makername)
                            && (x.Status == status || "All" == status)).Count();

                totalPage = (totalRow + pageSize - 1) / pageSize;
                totalPage = totalPage == 0 ? 1 : totalPage;

                var hocs = models == null ? null :
                            models.Select(a =>
                             new
                             {
                                 a.PartMakerID,
                                 a.PartName,
                                 a.MakerName,
                                 a.PartNo,
                                 a.ItemNo,
                                 a.Description,
                                 a.Status,
                                 a.IsActive,
                                 ShowButton = true,
                                 DetailURL = EncyptURLHelper.EncodedActionUrl("DetailPartMaker", "PartMaker", new { PartMakerID = a.PartMakerID, area = "PartMaker" }),
                                 EditURL = EncyptURLHelper.EncodedActionUrl("EditPartMaker", "PartMaker", new { PartMakerID = a.PartMakerID, area = "PartMaker" }),
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
        public ActionResult GetMakerName(string makername)
        {
                var pcname = db.Makers.Where(x => (x.MakerName.ToUpper().Contains(makername.Trim().ToUpper()) || makername == "") && x.IsActive == true).Select(x => new { x.MakerName, x.MakerID }).ToList();
                return Json(pcname, JsonRequestBehavior.AllowGet);

        }


        public ActionResult CreatePartMaker()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(x => x.Id == userId);
            ViewBag.PartID = new SelectList(db.PartMasters, "PartID", "PartName");
            ViewBag.MakerID = new SelectList(db.Makers, "MakerID", "MakerName");

            return View("CreatePartMaker");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePartMaker(PartMaker partmaker)
        {
            partmaker.IsActive = true;
            partmaker.Status = "Draft";
            db.PartMakers.Add(partmaker);
            db.SaveChanges();
              
            return RedirectToAction("IndexPartMaker");
        }


        [EncryptedActionParameter]
        public async Task<ActionResult> EditPartMaker(int? PartMakerID)
        {
            PartMaker partmakerrec = await db.PartMakers.FindAsync(PartMakerID);
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

            return View("EditPartMaker", partmakerrec);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult EditPartMaker([Bind(Include = "PartMakerID, PartID, MakerID, Description, Status, IsActive, DateCreated, DateModified, UserCreated, UserModified")] PartMaker partmaker)
        {
            if (ModelState.IsValid)
            {
                var original_value = db.PartMakers.AsNoTracking().Where(P => P.PartID == partmaker.PartID).FirstOrDefault();
                partmaker.Status = original_value.Status;
                partmaker.UserCreated = original_value.UserCreated;
                partmaker.DateCreated = original_value.DateCreated;
                db.Entry(partmaker).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.PartID = new SelectList(db.PartMasters, "PartID", "PartName", partmaker.PartID);
                ViewBag.MakerID = new SelectList(db.Makers, "MakerID", "MakerName", partmaker.MakerID);

                return RedirectToAction("IndexPartMaker");
            }
            return View(partmaker);
        }

            
        [EncryptedActionParameter]
        public ActionResult DetailPartMaker(int? PartMakerID)
        {
            PartInv partmakerrec = db.PartInvs.Find(PartMakerID);
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

            return View(partmakerrec);
        }


        [EncryptedActionParameter]
        public ActionResult DetailPartMakerOld(int? PartMakerID)
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

            return View(partmakerrec);
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

