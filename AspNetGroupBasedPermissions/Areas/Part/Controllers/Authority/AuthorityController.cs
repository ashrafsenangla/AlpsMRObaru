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
    public class AuthorityController : Controller
    {
        private Infrastructures.Data.ApplicationDbContext db = new Infrastructures.Data.ApplicationDbContext();

        [EncryptedActionParameter]

        public ActionResult IndexAuthority()
        {
            var DetailCD = db.Authoritys;
            ViewBag.Status = DetailCD.Select(x => x.Status).Distinct();
            return View();
        }


        [HttpPost]
        public ActionResult GetAuthority(string search, int pageSize, int pageIndex, string status)
        {
            try
            {
                var detailrec = (from a in db.Authoritys
                                 select new
                                 {
                                     AuthorityID = a.AuthorityID,
                                     AuthorityName = a.AuthorityName,
                                     Description = a.Description,
                                     Status = a.Status,
                                     IsActive = a.IsActive,
                                 }).OrderByDescending(c => c.AuthorityID);

                int totalRow = 0;
                int totalPage = 0;
                int skip = pageIndex * pageSize;

                var models = detailrec.Where(x => (x.AuthorityName.Contains(search))
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
                                 a.AuthorityID,
                                 a.AuthorityName,
                                 a.Description,
                                 a.Status,
                                 a.IsActive,
                                 ShowButton = true,
                                 DetailURL = EncyptURLHelper.EncodedActionUrl("DetailAuthority", "Authority", new { AuthorityID = a.AuthorityID, area = "Part" }),
                                 EditURL = EncyptURLHelper.EncodedActionUrl("EditAuthority", "Authority", new { AuthorityID = a.AuthorityID, area = "Part" }),
                             });
                return Json(new { hocs = hocs, totalPage = totalPage,  totalRow = totalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Console.WriteLine("Page Error. Contact System Administrator");
                throw;
            }
        }

        public ActionResult CreateAuthority()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(x => x.Id == userId);
            return View("CreateAuthority");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAuthority (Authority authority)
        {
            {
                authority.IsActive = true;
                authority.Status = "Draft";
                db.Authoritys.Add(authority);
                db.SaveChanges();
            }
            return RedirectToAction("IndexAuthority");
        }

        [EncryptedActionParameter]
        public async Task<ActionResult> EditAuthority(int? AuthorityID)
        {
            Authority authority = await db.Authoritys.FindAsync(AuthorityID);
            if (AuthorityID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (AuthorityID == null)
            {
                return HttpNotFound();
            }
            return View("EditAuthority", authority);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult EditAuthority([Bind(Include = "AuthorityID, AuthorityName, Description, Status, IsActive, DateCreated, DateModified, UserCreated, UserModified")] Authority authority)
        {
            if (ModelState.IsValid)
            {
                var original_value = db.Authoritys.AsNoTracking().Where(P => P.AuthorityID == authority.AuthorityID).FirstOrDefault();
                authority.Status = original_value.Status;
                authority.UserCreated = original_value.UserCreated;
                authority.DateCreated = original_value.DateCreated;
                db.Entry(authority).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexAuthority");
            }
            return View(authority);
        }

            
        [EncryptedActionParameter]

        public ActionResult DetailAuthority(int? AuthorityID)
        {
            Authority authorityrec = db.Authoritys.Find(AuthorityID);
            if (AuthorityID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (authorityrec == null)
            {
                return HttpNotFound();
            }
            return View(authorityrec);
        }

        [EncryptedActionParameter]
        public ActionResult DeleteAuthority(int? AuthorityID)
        {
            Authority authorityrecs = db.Authoritys.Find(AuthorityID);
            if (AuthorityID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (authorityrecs == null)
            {
                return HttpNotFound();
            }

            return View(authorityrecs);
        }

        [HttpPost, ActionName("DeleteAuthority")]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult DeleteAuthorityConfirmed(int AuthorityID)
        {
            {
                Authority authorityrecs = db.Authoritys.Find(AuthorityID);
                db.Authoritys.Remove(authorityrecs);
                db.SaveChanges();
                return RedirectToAction("IndexAuthority");
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

