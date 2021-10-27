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
    public class CostCentreController : Controller
    {
        private Infrastructures.Data.ApplicationDbContext db = new Infrastructures.Data.ApplicationDbContext();

        [EncryptedActionParameter]

        public ActionResult IndexCostCentre()
        {
            var DetailCD = db.CostCentres;
            ViewBag.Status = DetailCD.Select(x => x.Status).Distinct();
            return View();
        }


        [HttpPost]
        public ActionResult GetCostCentre(string search, int pageSize, int pageIndex, string status)
        {
            try
            {
                var detailrec = (from a in db.CostCentres
                                 select new
                                 {
                                     CostCentreID = a.CostCentreID,
                                     CostCentreName = a.CostCentreName,
                                     Description = a.Description,
                                     Status = a.Status,
                                     IsActive = a.IsActive,
                                 }).OrderByDescending(c => c.CostCentreID);

                int totalRow = 0;
                int totalPage = 0;
                int skip = pageIndex * pageSize;

                var models = detailrec.Where(x => (x.CostCentreName.Contains(search))
                             && (x.Status == status ||  "All" == status))
                            .Skip(skip)
                            .Take(pageSize)
                            .ToList();
                totalRow = detailrec.Where(x => (x.CostCentreName.Contains(search))
                            && (x.Status == status || "All" == status)).Count();
           
                totalPage = (totalRow + pageSize - 1) / pageSize;
                totalPage = totalPage == 0 ? 1 : totalPage;

                var hocs = models == null ? null :
                            models.Select(a =>
                             new
                             {
                                 a.CostCentreID,
                                 a.CostCentreName,
                                 a.Description,
                                 a.Status,
                                 a.IsActive,
                                 ShowButton = true,
                                 DetailURL = EncyptURLHelper.EncodedActionUrl("DetailCostCentre", "CostCentre", new { CostCentreID = a.CostCentreID, area = "Part" }),
                                 EditURL = EncyptURLHelper.EncodedActionUrl("EditCostCentre", "CostCentre", new { CostCentreID = a.CostCentreID, area = "Part" }),
                             });
                return Json(new { hocs = hocs, totalPage = totalPage,  totalRow = totalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Console.WriteLine("Page Error. Contact System Administrator");
                throw;
            }
        }


        public ActionResult CheckDuplicate(string costcenter)
        {

            if (db.CostCentres.Where(x => x.CostCentreName == costcenter).Count() > 0)
            {
                ViewBag.CanDelete = "No";
                return Json(new { success = false, message = "Duplicate Cost Center : " }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CreateCostCentre()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(x => x.Id == userId);
            return View("CreateCostCentre");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCostCentre (CostCentre costcentre)
        {
            {
                costcentre.IsActive = true;
                costcentre.Status = "Draft";
                db.CostCentres.Add(costcentre);
                db.SaveChanges();
            }
            return RedirectToAction("IndexCostCentre");
        }

        [EncryptedActionParameter]
        public ActionResult EditCostCentre(int? CostCentreID)
        {
            CostCentre costcentre = db.CostCentres.Find(CostCentreID);
            if (CostCentreID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (CostCentreID == null)
            {
                return HttpNotFound();
            }
            return View("EditCostCentre", costcentre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult EditCostCentre([Bind(Include = "CostCentreID, CostCentreName, Description, Status, IsActive, DateCreated, DateModified, UserCreated, UserModified")] CostCentre costcentre)
        {
            if (ModelState.IsValid)
            {
                var original_value = db.CostCentres.AsNoTracking().Where(P => P.CostCentreID == costcentre.CostCentreID).FirstOrDefault();
                costcentre.Status = original_value.Status;
                costcentre.UserCreated = original_value.UserCreated;
                costcentre.DateCreated = original_value.DateCreated;
                db.Entry(costcentre).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexCostCentre");
            }
            return View(costcentre);
        }

            
        [EncryptedActionParameter]

        public ActionResult DetailCostCentre(int? CostCentreID)
        {
            CostCentre costcentrerec = db.CostCentres.Find(CostCentreID);
            if (CostCentreID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (costcentrerec == null)
            {
                return HttpNotFound();
            }
            return View(costcentrerec);
        }

        [EncryptedActionParameter]
        public ActionResult DeleteCostCentre(int? CostCentreID)
        {
            CostCentre costcentrerecs = db.CostCentres.Find(CostCentreID);
            if (CostCentreID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (costcentrerecs == null)
            {
                return HttpNotFound();
            }
            if (db.Sections.Where(x => x.CostCentreID == CostCentreID).Count() > 0)
            {
                ViewBag.CanDelete = "No";
                return View(costcentrerecs);
            }

            return View(costcentrerecs);
        }

        [HttpPost, ActionName("DeleteCostCentre")]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult DeleteCostCentreConfirmed(int CostCentreID)
        {
            {
                CostCentre costcentrerecs = db.CostCentres.Find(CostCentreID);
                db.CostCentres.Remove(costcentrerecs);
                db.SaveChanges();
                return RedirectToAction("IndexCostCentre");
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

