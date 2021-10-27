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
    public class MakerController : Controller
    {
        private Infrastructures.Data.ApplicationDbContext db = new Infrastructures.Data.ApplicationDbContext();

        [EncryptedActionParameter]

        public ActionResult IndexMaker()
        {
            var DetailCD = db.Makers;
            ViewBag.Status = DetailCD.Select(x => x.Status).Distinct();
            return View();
        }


        [HttpPost]
        public ActionResult GetMaker(string search, int pageSize, int pageIndex, string status)
        {
            try
            {
                var detailrec = (from a in db.Makers
                                 select new
                                 {
                                     MakerID = a.MakerID,
                                     MakerName = a.MakerName,
                                     Description = a.Description,
                                     Status = a.Status,
                                     IsActive = a.IsActive,
                                 }).OrderByDescending(c => c.MakerID);

                int totalRow = 0;
                int totalPage = 0;
                int skip = pageIndex * pageSize;

                var models = detailrec.Where(x => (x.MakerName.Contains(search))
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
                                 a.MakerID,
                                 a.MakerName,
                                 a.Description,
                                 a.Status,
                                 a.IsActive,
                                 ShowButton = true,
                                 DetailURL = EncyptURLHelper.EncodedActionUrl("DetailMaker", "Maker", new { MakerID = a.MakerID, area = "Part" }),
                                 EditURL = EncyptURLHelper.EncodedActionUrl("EditMaker", "Maker", new { MakerID = a.MakerID, area = "Part" }),
                             });
                return Json(new { hocs = hocs, totalPage = totalPage,  totalRow = totalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Console.WriteLine("Page Error. Contact System Administrator");
                throw;
            }
        }

        public ActionResult CreateMaker()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(x => x.Id == userId);
            return View("CreateMaker");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMaker (Maker maker)
        {
            {
                maker.IsActive = true;
                maker.Status = "Draft";
                db.Makers.Add(maker);
                db.SaveChanges();
            }
            return RedirectToAction("IndexMaker");
        }

        [EncryptedActionParameter]
        public async Task<ActionResult> EditMaker(int? MakerID)
        {
            Maker maker = await db.Makers.FindAsync(MakerID);
            if (MakerID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (MakerID == null)
            {
                return HttpNotFound();
            }
            return View("EditMaker", maker);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult EditMaker([Bind(Include = "MakerID, MakerName, Description, Status, IsActive, DateCreated, DateModified, UserCreated, UserModified")] Maker maker)
        {
            if (ModelState.IsValid)
            {
                var original_value = db.Makers.AsNoTracking().Where(P => P.MakerID == maker.MakerID).FirstOrDefault();
                maker.Status = original_value.Status;
                maker.UserCreated = original_value.UserCreated;
                maker.DateCreated = original_value.DateCreated;
                db.Entry(maker).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexMaker");
            }
            return View(maker);
        }

            
        [EncryptedActionParameter]

        public ActionResult DetailMaker(int? MakerID)
        {
            Maker makerrec = db.Makers.Find(MakerID);
            if (MakerID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (makerrec == null)
            {
                return HttpNotFound();
            }
            return View(makerrec);
        }

        [EncryptedActionParameter]
        public ActionResult DeleteMaker(int? MakerID)
        {
            Maker makerrecs = db.Makers.Find(MakerID);
            if (MakerID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (makerrecs == null)
            {
                return HttpNotFound();
            }

            //if (db.PartRequests.Where(x => x.MakerID == MakerID).Count() > 0)
            //{
            //    ViewBag.CanDelete = "No";
            //    return View(makerrecs);
            //}

            return View(makerrecs);
        }


        public ActionResult CheckDuplicate(string makername)
        {

            if (db.Makers.Where(x => x.MakerName == makername).Count() > 0)
            {
                ViewBag.CanDelete = "No";
                return Json(new { success = false, message = "Duplicate Part No : " }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("DeleteMaker")]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult DeleteMakerConfirmed(int MakerID)
        {
            {
                Maker makerrecs = db.Makers.Find(MakerID);
                db.Makers.Remove(makerrecs);
                db.SaveChanges();
                return RedirectToAction("IndexMaker");
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

