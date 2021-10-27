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
    public class CabinetController : Controller
    {
        private Infrastructures.Data.ApplicationDbContext db = new Infrastructures.Data.ApplicationDbContext();

        [EncryptedActionParameter]

        public ActionResult IndexCabinet()
        {
            var DetailCD = db.Cabinets.Include(x => x.RefStoreID);
            ViewBag.Store = DetailCD.Select(x => x.RefStoreID.WhouseName).Distinct();
            ViewBag.Status = DetailCD.Select(x => x.Status).Distinct();
            return View();
        }


        [HttpPost]
        public ActionResult GetCabinet(string search, int pageSize, int pageIndex, string store ,string status)
        {
            try
            {
                var detailrec = (from a in db.Cabinets
                                 select new
                                 {
                                     CabinetID = a.CabinetID,
                                     CabinetName = a.CabinetName,
                                     WhouseName = a.RefStoreID.WhouseName,
                                     Description = a.Description,
                                     Status = a.Status,
                                     IsActive = a.IsActive,
                                 }).OrderByDescending(c => c.CabinetID);

                int totalRow = 0;
                int totalPage = 0;
                int skip = pageIndex * pageSize;

                var models = detailrec.Where(x => (x.CabinetName.Contains(search))
                             && (x.WhouseName == store ||  "All" == store)
                             && (x.Status == status || "All" == status))
                            .Skip(skip)
                            .Take(pageSize)
                            .ToList();
                totalRow = detailrec.Where(x => (x.CabinetName.Contains(search))
                             && (x.WhouseName == store || "All" == store)
                             && (x.Status == status || "All" == status)).Count();
           
                totalPage = (totalRow + pageSize - 1) / pageSize;
                totalPage = totalPage == 0 ? 1 : totalPage;

                var hocs = models == null ? null :
                            models.Select(a =>
                             new
                             {
                                 a.CabinetID,
                                 a.CabinetName,
                                 a.WhouseName,
                                 a.Description,
                                 a.Status,
                                 a.IsActive,
                                 ShowButton = true,
                                 DetailURL = EncyptURLHelper.EncodedActionUrl("DetailCabinet", "Cabinet", new { CabinetID = a.CabinetID, area = "Part" }),
                                 EditURL = EncyptURLHelper.EncodedActionUrl("EditCabinet", "Cabinet", new { CabinetID = a.CabinetID, area = "Part" }),
                             });
                return Json(new { hocs = hocs, totalPage = totalPage,  totalRow = totalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Console.WriteLine("Page Error. Contact System Administrator");
                throw;
            }
        }



        public ActionResult CheckDuplicate(string cabinetname)
        {

            if (db.Cabinets.Where(x => x.CabinetName == cabinetname).Count() > 0)
            {
                ViewBag.CanDelete = "No";
                return Json(new { success = false, message = "Duplicate Cabinet : " }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateCabinet()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(x => x.Id == userId);
            ViewBag.WhouseID = new SelectList(db.Stores, "WhouseID", "WhouseName");
            return View("CreateCabinet");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCabinet (Cabinet cabinet)
        {
            {
                cabinet.IsActive = true;
                cabinet.Status = "Draft";
                db.Cabinets.Add(cabinet);
                db.SaveChanges();
            }
            return RedirectToAction("IndexCabinet");
        }

        [EncryptedActionParameter]
        public ActionResult EditCabinet(int? CabinetID)
        {
            Cabinet cabinetrec = db.Cabinets.Find(CabinetID);
            if (CabinetID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (CabinetID == null)
            {
                return HttpNotFound();
            }
            ViewBag.WhouseID = new SelectList(db.Stores, "WhouseID", "WhouseName", cabinetrec.WhouseID);

            return View("EditCabinet", cabinetrec);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult EditCabinet([Bind(Include = "CabinetID, CabinetName, WhouseID ,Description, Status, IsActive, DateCreated, DateModified, UserCreated, UserModified")] Cabinet cabinet)
        {
            if (ModelState.IsValid)
            {
                var original_value = db.Cabinets.AsNoTracking().Where(P => P.CabinetID == cabinet.CabinetID).FirstOrDefault();
                cabinet.Status = original_value.Status;
                cabinet.UserCreated = original_value.UserCreated;
                cabinet.DateCreated = original_value.DateCreated;
                db.Entry(cabinet).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.WhouseID = new SelectList(db.Stores, "WhouseID", "WhouseName", cabinet.WhouseID);

                return RedirectToAction("IndexCabinet");
            }
            return View(cabinet);
        }

            
        [EncryptedActionParameter]

        public ActionResult DetailCabinet(int? CabinetID)
        {
            Cabinet cabinetrec = db.Cabinets.Find(CabinetID);
            if (CabinetID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (cabinetrec == null)
            {
                return HttpNotFound();
            }
            ViewBag.WhouseID = new SelectList(db.Stores, "WhouseID", "WhouseName", cabinetrec.WhouseID);

            return View(cabinetrec);
        }

        [EncryptedActionParameter]
        public ActionResult DeleteCabinet(int? CabinetID)
        {
            Cabinet cabinetrec = db.Cabinets.Find(CabinetID);
            if (CabinetID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (cabinetrec == null)
            {
                return HttpNotFound();
            }
            ViewBag.WhouseID = new SelectList(db.Stores, "WhouseID", "WhouseName", cabinetrec.WhouseID);
            return View(cabinetrec);
        }

        [HttpPost, ActionName("DeleteCabinet")]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult DeleteCabinetConfirmed(int CabinetID)
        {
            {
                Cabinet cabinetrec = db.Cabinets.Find(CabinetID);
                db.Cabinets.Remove(cabinetrec);
                db.SaveChanges();
                ViewBag.WhouseID = new SelectList(db.Stores, "WhouseID", "WhouseName", cabinetrec.WhouseID);
                //if (db.PartInvs.Where(x => x.DrawerID == DrawerID).Count() > 0)
                //{
                //    ViewBag.CanDelete = "No";
                //    return View(drawerrec);
                //}
                return RedirectToAction("IndexCabinet");
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

