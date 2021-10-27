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
    public class StatusRequestController : Controller
    {
        private Infrastructures.Data.ApplicationDbContext db = new Infrastructures.Data.ApplicationDbContext();

        [EncryptedActionParameter]

        public ActionResult IndexStatusRequest()
        {
            var DetailCD = db.StatusRequests;
            ViewBag.Status = DetailCD.Select(x => x.Status).Distinct();
            return View();
        }


        [HttpPost]
        public ActionResult GetStatusRequest(string search, int pageSize, int pageIndex, string status)
        {
            try
            {
                var detailrec = (from a in db.StatusRequests
                                 select new
                                 {
                                     StatusID = a.StatusID,
                                     StatusName = a.StatusName,
                                     Description = a.Description,
                                     Status = a.Status,
                                     IsActive = a.IsActive,
                                 }).OrderByDescending(c => c.StatusID);

                int totalRow = 0;
                int totalPage = 0;
                int skip = pageIndex * pageSize;

                var models = detailrec.Where(x => (x.StatusName.Contains(search))
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
                                 a.StatusID,
                                 a.StatusName,
                                 a.Description,
                                 a.Status,
                                 a.IsActive,
                                 ShowButton = true,
                                 DetailURL = EncyptURLHelper.EncodedActionUrl("DetailStatusRequest", "StatusRequest", new { StatusID = a.StatusID, area = "Part" }),
                                 EditURL = EncyptURLHelper.EncodedActionUrl("EditStatusRequest", "StatusRequest", new { StatusID = a.StatusID, area = "Part" }),
                             });
                return Json(new { hocs = hocs, totalPage = totalPage,  totalRow = totalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Console.WriteLine("Page Error. Contact System Administrator");
                throw;
            }
        }

        public ActionResult CreateStatusRequest()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(x => x.Id == userId);
            return View("CreateStatusRequest");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateStatusRequest (StatusRequest statusrequest)
        {
            {
                statusrequest.IsActive = true;
                statusrequest.Status = "Draft";
                db.StatusRequests.Add(statusrequest);
                db.SaveChanges();
            }
            return RedirectToAction("IndexStatusRequest");
        }

        [EncryptedActionParameter]
        public async Task<ActionResult> EditStatusRequest(int? StatusID)
        {
            StatusRequest statusrequest = await db.StatusRequests.FindAsync(StatusID);
            if (StatusID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (StatusID == null)
            {
                return HttpNotFound();
            }
            return View("EditStatusRequest", statusrequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult EditStatusRequest([Bind(Include = "StatusID, StatusName, Description, Status, IsActive, DateCreated, DateModified, UserCreated, UserModified")] StatusRequest statusrequest)
        {
            if (ModelState.IsValid)
            {
                var original_value = db.StatusRequests.AsNoTracking().Where(P => P.StatusID == statusrequest.StatusID).FirstOrDefault();
                statusrequest.Status = original_value.Status;
                statusrequest.UserCreated = original_value.UserCreated;
                statusrequest.DateCreated = original_value.DateCreated;
                db.Entry(statusrequest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexStatusRequest");
            }
            return View(statusrequest);
        }

            
        [EncryptedActionParameter]

        public ActionResult DetailStatusRequest(int? StatusID)
        {
            StatusRequest statusrequestrec = db.StatusRequests.Find(StatusID);
            if (StatusID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (statusrequestrec == null)
            {
                return HttpNotFound();
            }
            return View(statusrequestrec);
        }

        [EncryptedActionParameter]
        public ActionResult DeleteStatusRequest(int? StatusID)
        {
            StatusRequest statusrequestrecs = db.StatusRequests.Find(StatusID);
            if (StatusID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (statusrequestrecs == null)
            {
                return HttpNotFound();
            }

            return View(statusrequestrecs);
        }

        [HttpPost, ActionName("DeleteStatusRequest")]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult DeleteStatusRequestConfirmed(int StatusID)
        {
            {
                StatusRequest statusrequestrecs = db.StatusRequests.Find(StatusID);
                db.StatusRequests.Remove(statusrequestrecs);
                db.SaveChanges();
                return RedirectToAction("IndexStatusRequest");
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

