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

namespace AspNetGroupBasedPermissions.Areas.Part.Controllers
{
    public class PartTransferInController : Controller
    {
        private Infrastructures.Data.ApplicationDbContext db = new Infrastructures.Data.ApplicationDbContext();

        [EncryptedActionParameter]

        public ActionResult IndexPartTransferIn()
        {
            var DetailCD = db.PartTransferIns.Include(x => x.RefPartID).Include(y => y.RefCabinetID).Include(y => y.RefBUnitID);
            ViewBag.Part = DetailCD.Select(x => x.RefPartID.PartName).Distinct();
            ViewBag.Drawer = DetailCD.Select(x => x.RefCabinetID.CabinetName).Distinct();
            ViewBag.BUnit = DetailCD.Select(x => x.RefBUnitID.BusinessUnitName).Distinct();
            ViewBag.Status = DetailCD.Select(x => x.Status).Distinct();
            ViewBag.IsActive = DetailCD.Select(x => x.IsActive == true ? "Yes" : "No").Distinct();
            return View();
        }


        [HttpPost]
        public ActionResult GetPartTransferIn(string search, int pageSize, int pageIndex, 
             string buname, string drawername, string partname, string status, string isactive)
        {
            try
            {
                var detailrec = (from a in db.PartTransferIns
                                 select new
                                 {
                                     PartTransferInID = a.PartTransferInID,
                                     BUName = a.RefBUnitID.BusinessUnitName,
                                     PartName = a.RefPartID.PartName,
                                     DrawerName = a.RefCabinetID.CabinetName,
                                     Description = a.Description,
                                     Quantity = a.Qty,
                                     Status = a.Status,
                                     IsActive = a.IsActive == true ? "Yes" : "No",
                                 }).OrderByDescending(c => c.PartTransferInID);

                int totalRow = 0;
                int totalPage = 0;
                int skip = pageIndex * pageSize;

                var models = detailrec.Where(x => (x.PartName.Contains(search))
                             && (x.Status == status || "All" == status)
                             && (x.IsActive == isactive || "All" == isactive)
                             && (x.PartName == partname || "All" == partname)
                             && (x.DrawerName == drawername || "All" == drawername)
                             && (x.BUName == buname || "All" == buname))
                            .Skip(skip)
                            .Take(pageSize)
                            .ToList();
                totalRow = detailrec.Where(x => (x.Status.Contains(search))
                             && (x.Status == status || "All" == status)
                             && (x.IsActive == isactive || "All" == isactive)
                             && (x.PartName == partname || "All" == partname)
                             && (x.DrawerName == drawername || "All" == drawername)
                             && (x.BUName == buname || "All" == buname)).Count();


                totalPage = (totalRow + pageSize - 1) / pageSize;
                totalPage = totalPage == 0 ? 1 : totalPage;

                var hocs = models == null ? null :
                            models.Select(a =>
                             new
                             {
                                 a.PartTransferInID,
                                 a.BUName,
                                 a.PartName,
                                 a.Quantity,
                                 a.DrawerName,
                                 a.Description,
                                 a.Status,
                                 a.IsActive,
                                 ShowButton = true,
                                 DetailURL = EncyptURLHelper.EncodedActionUrl("DetailPartTransferIn", "PartTransferIn", new { PartTransferInID = a.PartTransferInID, area = "PartTransferIn" }),
                                 EditURL = EncyptURLHelper.EncodedActionUrl("EditPartTransferIn", "PartTransferIn", new { PartTransferInID = a.PartTransferInID, area = "PartTransferIn" }),
                             });
                return Json(new { hocs = hocs, totalPage = totalPage,  totalRow = totalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Console.WriteLine("Page Error. Contact System Administrator");
                throw;
            }
        }

        public ActionResult CreatePartTransferIn()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(x => x.Id == userId);
            ViewBag.BUID = new SelectList(db.BusinessUnits.Where(x => x.IsActive==true), "ID", "BUName");
            ViewBag.PartID = new SelectList(db.PartMasters.Where(x => x.IsActive == true), "PartID", "PartName");
            ViewBag.CabinetID = new SelectList(db.Cabinets.Where(x => x.IsActive == true), "CabinetID", "CabinetName");

            return View("CreatePartTransferIn");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePartTransferIn(PartTransferIn PartTransferIn)
        {
            PartTransferIn.IsActive = true;
            PartTransferIn.Status = "Draft";
            db.PartTransferIns.Add(PartTransferIn);
            db.SaveChanges();
              
            return RedirectToAction("IndexPartTransferIn");
        }


        [EncryptedActionParameter]
        public async Task<ActionResult> EditPartTransferIn(int? PartTransferInID)
        {
            PartTransferIn parttransferinrec = await db.PartTransferIns.FindAsync(PartTransferInID);
            if (PartTransferInID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (parttransferinrec == null)
            {
                return HttpNotFound();
            }
            ViewBag.BUID = new SelectList(db.BusinessUnits.Where(x => x.IsActive == true), "ID", "BUName", parttransferinrec.BUID);
            ViewBag.PartID = new SelectList(db.PartMasters.Where(x => x.IsActive == true), "PartID", "PartName", parttransferinrec.PartID);
            ViewBag.CabinetID = new SelectList(db.Cabinets.Where(x => x.IsActive == true), "CabinetID", "CabinetName", parttransferinrec.CabinetID);

            return View("EditPartTransferIn", parttransferinrec);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult EditPartTransferIn([Bind(Include = "PartTransferInID, PartID, BUID, Qty , DrawerID, Description, Status, IsActive, DateCreated, DateModified, UserCreated, UserModified")] PartTransferIn parttransferin)
        {
            if (ModelState.IsValid)
            {
                var original_value = db.PartTransferIns.AsNoTracking().Where(P => P.PartTransferInID == parttransferin.PartTransferInID).FirstOrDefault();
                parttransferin.Status = original_value.Status;
                parttransferin.UserCreated = original_value.UserCreated;
                parttransferin.DateCreated = original_value.DateCreated;
                db.Entry(parttransferin).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("IndexPartTransferIn");
            }
            return View(parttransferin);
        }

            
        [EncryptedActionParameter]

        public ActionResult DetailPartTransferIn(int? PartTransferInID)
        {
            PartTransferIn parttransferinrec = db.PartTransferIns.Find(PartTransferInID);
            if (PartTransferInID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (parttransferinrec == null)
            {
                return HttpNotFound();
            }
            ViewBag.BUID = new SelectList(db.BusinessUnits, "ID", "BUName", parttransferinrec.PartID);
            ViewBag.PartID = new SelectList(db.PartMasters, "PartID", "PartName", parttransferinrec.PartID);
            ViewBag.CabinetID = new SelectList(db.Cabinets, "CabinetID", "CabinetName", parttransferinrec.CabinetID);

            return View(parttransferinrec);
        }

        [EncryptedActionParameter]
        public ActionResult DeletePartTransferIn(int? PartTransferInID)
        {
            PartTransferIn parttransferinrec = db.PartTransferIns.Find(PartTransferInID);
            if (PartTransferInID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (parttransferinrec == null)
            {
                return HttpNotFound();
            }
            ViewBag.BUID = new SelectList(db.BusinessUnits, "BUID", "BUName", parttransferinrec.PartID);
            ViewBag.PartID = new SelectList(db.PartMasters, "PartID", "PartName", parttransferinrec.PartID);
            ViewBag.CabinetID = new SelectList(db.Cabinets, "CabinetID", "CabinetName", parttransferinrec.CabinetID);

            return View(parttransferinrec);
        }

        [HttpPost, ActionName("DeletePartTransferIn")]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult DeletePartTransferInConfirmed(int PartTransferInID)
        {
            {
                PartTransferIn PartTransferInrec = db.PartTransferIns.Find(PartTransferInID);
                db.PartTransferIns.Remove(PartTransferInrec);
                db.SaveChanges();
                ViewBag.PartID = new SelectList(db.PartMasters, "PartID", "PartName", PartTransferInrec.PartID);
                ViewBag.DrawerID = new SelectList(db.Makers, "DrawerID", "DrawerName", PartTransferInrec.CabinetID);
                return RedirectToAction("IndexPartTransferIn");
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

