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
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Core.Entities.Data.PartData;

namespace AspNetGroupBasedPermissions.Areas.Part.Controllers
{
    public class PlantController : Controller
    {
        private Infrastructures.Data.ApplicationDbContext db = new Infrastructures.Data.ApplicationDbContext();

        [EncryptedActionParameter]

        public ActionResult IndexPlant()
        {
            var DetailCD = db.Plants;
            ViewBag.Status = DetailCD.Select(x => x.Status).Distinct();
            return View();
        }


        [HttpPost]
        public ActionResult GetPlant(string search, int pageSize, int pageIndex)
        {
            try
            {
                var detailrec = (from a in db.Plants
                                 select new
                                 {
                                     PlantID = a.PlantID,
                                     PlantName = a.PlantName,
                                     Description = a.Description,
                                     Status = a.Status,
                                     IsActive = a.IsActive,
                                 }).OrderByDescending(c => c.PlantID);

                int totalRow = 0;
                int totalPage = 0;
                int skip = pageIndex * pageSize;

                var models = detailrec.Where(x => (x.PlantName.Contains(search))
                                    && x.Status == "Draft")
                            .Skip(skip)
                            .Take(pageSize)
                            .ToList();
                totalRow = detailrec.Where(x => (x.PlantName.Contains(search))
                                    && x.Status == "Draft").Count();
           
                totalPage = (totalRow + pageSize - 1) / pageSize;
                totalPage = totalPage == 0 ? 1 : totalPage;

                var hocs = models == null ? null :
                            models.Select(a =>
                             new
                             {
                                 a.PlantID,
                                 a.PlantName,
                                 a.Description,
                                 a.Status,
                                 a.IsActive,
                                 ShowButton = true,
                                 DetailURL = EncyptURLHelper.EncodedActionUrl("DetailPLant", "Plant", new { PlantID = a.PlantID, area = "Plant" }),
                                 EditURL = EncyptURLHelper.EncodedActionUrl("EditPlant", "Plant", new { PlanID = a.PlantID, area = "Plant" }),
                             });
                return Json(new { hocs = hocs, totalPage = totalPage,  totalRow = totalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Console.WriteLine("Page Error. Contact System Administrator");
                throw;
            }
        }


        public ActionResult CheckDuplicate(string plantname)
        {

            if (db.Plants.Where(x => x.PlantName == plantname).Count() > 0)
            {
                ViewBag.CanDelete = "No";
                return Json(new { success = false, message = "Duplicate Plant : " }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreatePlant()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(x => x.Id == userId);
            return View("CreatePlant");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePlant (Plant plants)
        {
            {
                plants.IsActive = true;
                plants.Status = "Draft";
                db.Plants.Add(plants);
                db.SaveChanges();
            }
            return RedirectToAction("IndexPlant");
        }

        [EncryptedActionParameter]
        public ActionResult EditPlant(int? PlantID)
        {
            Plant plantrec = db.Plants.Find(PlantID);
            if (PlantID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (plantrec == null)
            {
                return HttpNotFound();
            }
            return View("EditPlant", plantrec);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult EditPlant([Bind(Include = "PlantID, PlantName, Description, Status, IsActive, DateCreated, DateModified, UserCreated, UserModified")] Plant plant)
        {
            if (ModelState.IsValid)
            {
                var original_value = db.Plants.AsNoTracking().Where(P => P.PlantID == plant.PlantID).FirstOrDefault();
                plant.Status = original_value.Status;
                plant.UserCreated = original_value.UserCreated;
                plant.DateCreated = original_value.DateCreated;
                db.Entry(plant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexPlant");
            }
            return View(plant);
        }

            
        [EncryptedActionParameter]

        public ActionResult DetailPlant(int? PlantID)
        {
            Plant plantrecs = db.Plants.Find(PlantID);
            if (PlantID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (plantrecs == null)
            {
                return HttpNotFound();
            }
            return View(plantrecs);
        }

        [EncryptedActionParameter]
        public ActionResult DeletePlant(int? PlantID)
        {
            Plant plantrecs = db.Plants.Find(PlantID);
            if (PlantID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (plantrecs == null)
            {
                return HttpNotFound();
            }
            if (db.Sections.Where(x => x.PlantID == PlantID).Count() > 0)
            {
                ViewBag.CanDelete = "No";
                return View(plantrecs);
            }

            return View(plantrecs);
        }

        [HttpPost, ActionName("DeletePlant")]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult DeletePlantConfirmed(int PlantID)
        {
            {
                Plant plant = db.Plants.Find(PlantID);
                db.Plants.Remove(plant);
                db.SaveChanges();
                return RedirectToAction("IndexPlant");
            }
        }

        [HttpPost]
        public ActionResult DeletePlantx(List<int> plantrecid)
        {
            if (plantrecid.Count > 0)
            {
                var plantrecids = db.Plants.Where(x => x.Status == "Draft" && plantrecid.Contains(x.PlantID));
                foreach (var listrec in plantrecids)
                {
                    Plant plantrecdelete = db.Plants.Find(listrec.PlantID);
                    db.Plants.Remove(plantrecdelete);
                    db.SaveChanges();
                }
            }
            return Json("OK");
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

