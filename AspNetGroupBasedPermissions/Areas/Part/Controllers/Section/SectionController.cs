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
    public class SectionController : Controller
    {
        private Infrastructures.Data.ApplicationDbContext db = new Infrastructures.Data.ApplicationDbContext();

        [EncryptedActionParameter]

        public ActionResult IndexSection()
        {
            var DetailCD = db.Sections.Include(x => x.RefBUnitID).Include(y => y.RefCostCentreID).Include(z => z.RefPlantID);
            ViewBag.BU = DetailCD.Select(x => x.RefBUnitID.BusinessUnitName).Distinct();
            ViewBag.Plant = DetailCD.Select(x => x.RefPlantID.PlantName).Distinct();
            ViewBag.CostCentre = DetailCD.Select(x => x.RefCostCentreID.CostCentreName).Distinct();

            ViewBag.Status = DetailCD.Select(x => x.Status).Distinct();
            return View();
        }


        [HttpPost]
        public ActionResult FillSection(int filterid, string filtercat )
        {
            if (filtercat == "plant")
            {
                var results = db.Sections.Where(x => x.PlantID == filterid).Select(x => new { x.BUID, x.SectionName }).ToList();
                return Json(results, JsonRequestBehavior.AllowGet);
            }
            if (filtercat == "bu")
            {
                var results = db.Sections.Where(x => x.BUID == filterid).Select(x => new { x.BUID, x.SectionName }).ToList();
                return Json(results, JsonRequestBehavior.AllowGet);
            }
            if (filtercat == "cc")
            {
                var results = db.Sections.Where(x => x.CostCentreID == filterid).Select(x => new { x.BUID, x.SectionName }).ToList();
                return Json(results, JsonRequestBehavior.AllowGet);
            }

            return Json(JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult FillStore(int filterid, string filtercat)
        {
            if (filtercat == "section")
            {
                var results = db.Stores.Where(x => x.WhouseID == filterid).Select(x => new { x.WhouseID, x.WhouseName }).ToList();
                return Json(results, JsonRequestBehavior.AllowGet);
            }
            if (filtercat == "bu")
            {
                var results = db.Stores.Where(x => x.WhouseID == filterid).Select(x => new { x.WhouseID, x.WhouseName }).ToList();
                return Json(results, JsonRequestBehavior.AllowGet);
            }
            if (filtercat == "cc")
            {
                var results = db.Stores.Where(x => x.WhouseID == filterid).Select(x => new { x.WhouseID, x.WhouseName }).ToList();
                return Json(results, JsonRequestBehavior.AllowGet);
            }

            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult FillCabinet(int filterid, string filtercat)
        {
            if (filtercat == "store")
            {
                var results = db.Cabinets.Where(x => x.WhouseID == filterid).Select(x => new { x.CabinetID, x.CabinetName }).ToList();
                return Json(results, JsonRequestBehavior.AllowGet);
            }

            return Json(JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GetBizUnit(string bizunit)
        {
            var pcname = db.BusinessUnits.Where(x => (x.BusinessUnitName.ToUpper().Contains(bizunit.Trim().ToUpper()) || bizunit == "") && x.IsActive == true).Select(x => new { x.BusinessUnitName, x.BUID }).ToList();
            return Json(pcname, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GetCostCenter(string costcentername)
        {
            var pcname = db.CostCentres.Where(x => (x.CostCentreName.ToUpper().Contains(costcentername.Trim().ToUpper()) || costcentername == "") && x.IsActive == true).Select(x => new { x.CostCentreName, x.CostCentreID }).ToList();
            return Json(pcname, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult FillPart(int filterid, string filtercat)
        {
            if (filtercat == "cabinet")
            {
                var results = db.PartInvs.Where(x => x.CabinetID == filterid).Select(x => new { x.PartID, PartName =  x.RefPartMasterPartInvID.PartName }).ToList();
                return Json(results, JsonRequestBehavior.AllowGet);
            }

            return Json(JsonRequestBehavior.AllowGet);
        }



        public ActionResult CheckDuplicate(string sectionname)
        {

            if (db.Sections.Where(x => x.SectionName == sectionname).Count() > 0)
            {
                ViewBag.CanDelete = "No";
                return Json(new { success = false, message = "Duplicate Section : " }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GetSection(string search, int pageSize, int pageIndex, 
            string businessunit, string plant, string costcentre, string status)
        {
            try
            {
                var detailrec = (from a in db.Sections
                                 select new
                                 {
                                     SectionID = a.SectionID,
                                     SectionName = a.SectionName,
                                     PlantName = a.RefPlantID.PlantName,
                                     SAPCode = a.SAPCode,
                                     BUName = a.RefBUnitID.BusinessUnitName,
                                     CostCentreName = a.RefCostCentreID.CostCentreName,
                                     Description = a.Description,
                                     Status = a.Status,
                                     IsActive = a.IsActive,
                                 }).OrderByDescending(c => c.SectionID);

                int totalRow = 0;
                int totalPage = 0;
                int skip = pageIndex * pageSize;

                var models = detailrec.Where(x => (x.SectionName.Contains(search))
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
                                 a.SectionID,
                                 a.SAPCode,
                                 a.SectionName,
                                 a.Description,
                                 a.PlantName,
                                 a.BUName,
                                 a.CostCentreName,
                                 a.Status,
                                 a.IsActive,
                                 ShowButton = true,
                                 DetailURL = EncyptURLHelper.EncodedActionUrl("DetailSection", "Section", new { SectionID = a.SectionID, area = "Part" }),
                                 EditURL = EncyptURLHelper.EncodedActionUrl("EditSection", "Section", new { SectionID = a.SectionID, area = "Part" }),
                             });
                return Json(new { hocs = hocs, totalPage = totalPage,  totalRow = totalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Console.WriteLine("Page Error. Contact System Administrator");
                throw;
            }
        }

        public ActionResult CreateSection()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(x => x.Id == userId);
            ViewBag.PlantID = new SelectList(db.Plants, "PlantID", "PlantName");
            ViewBag.CostCentreID = new SelectList(db.CostCentres, "CostCentreID", "CostCentreName");
            ViewBag.BUID = new SelectList(db.BusinessUnits, "BUID", "BusinessUnitName");

            return View("CreateSection");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSection (Section section)
        {
            {
                section.IsActive = true;
                section.Status = "Draft";
                db.Sections.Add(section);
                db.SaveChanges();
            }
            return RedirectToAction("IndexSection");
        }

        [EncryptedActionParameter]
        public ActionResult EditSection(int? SectionID)
        {
            Section sectionrec = db.Sections.Find(SectionID);
            if (SectionID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (SectionID == null)
            {
                return HttpNotFound();
            }
            ViewBag.PlantID = new SelectList(db.Plants, "PlantID", "PlantName", sectionrec.PlantID);
            ViewBag.BUID = new SelectList(db.BusinessUnits, "BUID", "BusinessUnitName", sectionrec.BUID);
            ViewBag.CostCentreID = new SelectList(db.CostCentres, "CostCentreID", "CostCentreName", sectionrec.CostCentreID);

            return View("EditSection", sectionrec);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult EditSection([Bind(Include = "SectionID, SectionName, SAPCode, PlantID, BUID, CostCentreID, Description, Status, IsActive, DateCreated, DateModified, UserCreated, UserModified")] Section section)
        {
            if (ModelState.IsValid)
            {
                var original_value = db.Sections.AsNoTracking().Where(P => P.SectionID == section.SectionID).FirstOrDefault();
                section.Status = original_value.Status;
                section.UserCreated = original_value.UserCreated;
                section.DateCreated = original_value.DateCreated;
                db.Entry(section).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.PlantID = new SelectList(db.Plants, "PlantID", "PlantName", section.PlantID);
                ViewBag.BUID = new SelectList(db.BusinessUnits, "BUID", "BusinessUnitName", section.BUID);
                ViewBag.CostCentreID = new SelectList(db.CostCentres, "CostCentreID", "CostCentreName", section.CostCentreID);

                return RedirectToAction("IndexSection");
            }
            return View(section);
        }

            
        [EncryptedActionParameter]

        public ActionResult DetailSection(int? SectionID)
        {
            Section sectionrec = db.Sections.Find(SectionID);
            if (SectionID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (sectionrec == null)
            {
                return HttpNotFound();
            }
            ViewBag.PlantID = new SelectList(db.Plants, "PlantID", "PlantName", sectionrec.PlantID);
            ViewBag.BUID = new SelectList(db.BusinessUnits, "BUID", "BusinessUnitName", sectionrec.BUID);
            ViewBag.CostCentreID = new SelectList(db.CostCentres, "CostCentreID", "CostCentreName", sectionrec.CostCentreID);

            return View(sectionrec);
        }

        [EncryptedActionParameter]
        public ActionResult DeleteSection(int? SectionID)
        {
            Section sectionrec = db.Sections.Find(SectionID);
            if (SectionID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (sectionrec == null)
            {
                return HttpNotFound();
            }

            ViewBag.PlantID = new SelectList(db.Plants, "PlantID", "PlantName", sectionrec.PlantID);
            ViewBag.BUID = new SelectList(db.BusinessUnits, "BUID", "BusinessUnitName", sectionrec.BUID);
            ViewBag.CostCentreID = new SelectList(db.CostCentres, "CostCentreID", "CostCentreName", sectionrec.CostCentreID);

            if (db.Stores.Where(x => x.SectionID == SectionID).Count() > 0)
            {
                ViewBag.CanDelete = "No";
                return View(sectionrec);
            }

            return View(sectionrec);
        }

        [HttpPost, ActionName("DeleteSection")]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult DeleteSectionConfirmed(int SectionID)
        {
            {
                Section sectionrec = db.Sections.Find(SectionID);
                db.Sections.Remove(sectionrec);
                db.SaveChanges();
                ViewBag.PlantID = new SelectList(db.Plants, "PlantID", "PlantName", sectionrec.PlantID);
                ViewBag.BUID = new SelectList(db.BusinessUnits, "BUID", "BusinessUnitName", sectionrec.BUID);
                ViewBag.CostCentreID = new SelectList(db.CostCentres, "CostCentreID", "CostCentreName", sectionrec.CostCentreID);

                return RedirectToAction("IndexSection");
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

