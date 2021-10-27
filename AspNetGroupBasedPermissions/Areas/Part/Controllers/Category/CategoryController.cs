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
    public class CategoryController : Controller
    {
        private Infrastructures.Data.ApplicationDbContext db = new Infrastructures.Data.ApplicationDbContext();

        [EncryptedActionParameter]

        public ActionResult IndexCategory()
        {
            var DetailCD = db.Categorys;
            ViewBag.Status = DetailCD.Select(x => x.Status).Distinct();
            return View();
        }


        [HttpPost]
        public ActionResult GetCategory(string search, int pageSize, int pageIndex, string status)
        {
            try
            {
                var detailrec = (from a in db.Categorys
                                 select new
                                 {
                                     CategoryID = a.CategoryID,
                                     CategoryName = a.CategoryName,
                                     Description = a.Description,
                                     Status = a.Status,
                                     IsActive = a.IsActive,
                                 }).OrderByDescending(c => c.CategoryID);

                int totalRow = 0;
                int totalPage = 0;
                int skip = pageIndex * pageSize;

                var models = detailrec.Where(x => (x.CategoryName.Contains(search))
                             && (x.Status == status ||  "All" == status))
                            .Skip(skip)
                            .Take(pageSize)
                            .ToList();
                totalRow = detailrec.Where(x => (x.CategoryName.Contains(search))
                            && (x.Status == status || "All" == status)).Count();
           
                totalPage = (totalRow + pageSize - 1) / pageSize;
                totalPage = totalPage == 0 ? 1 : totalPage;

                var hocs = models == null ? null :
                            models.Select(a =>
                             new
                             {
                                 a.CategoryID,
                                 a.CategoryName,
                                 a.Description,
                                 a.Status,
                                 a.IsActive,
                                 ShowButton = true,
                                 DetailURL = EncyptURLHelper.EncodedActionUrl("DetailCategory", "Category", new { MainCategoryID = a.CategoryID, area = "Part" }),
                                 EditURL = EncyptURLHelper.EncodedActionUrl("EditCategory", "Category", new { MainCategoryID = a.CategoryID, area = "Part" }),
                             });
                return Json(new { hocs = hocs, totalPage = totalPage,  totalRow = totalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Console.WriteLine("Page Error. Contact System Administrator");
                throw;
            }
        }


        public ActionResult CheckDuplicate(string categoryname)
        {

            if (db.Categorys.Where(x => x.CategoryName == categoryname).Count() > 0)
            {
                ViewBag.CanDelete = "No";
                return Json(new { success = false, message = "Duplicate Part No : " }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateCategory()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(x => x.Id == userId);
            return View("CreateCategory");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCategory (Category category)
        {
            {
                category.IsActive = true;
                category.Status = "Draft";
                db.Categorys.Add(category);
                db.SaveChanges();
            }
            return RedirectToAction("IndexCategory");
        }

        [EncryptedActionParameter]
        public async Task<ActionResult> EditCategory(int? CategoryID)
        {
            Category category = await db.Categorys.FindAsync(CategoryID);
            if (CategoryID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (CategoryID == null)
            {
                return HttpNotFound();
            }
            return View("EditCategory", category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult EditCategory([Bind(Include = "CategoryID, CategoryName, Description, Status, IsActive, DateCreated, DateModified, UserCreated, UserModified")] Category category)
        {
            if (ModelState.IsValid)
            {
                var original_value = db.Categorys.AsNoTracking().Where(P => P.CategoryID == category.CategoryID).FirstOrDefault();
                category.Status = original_value.Status;
                category.UserCreated = original_value.UserCreated;
                category.DateCreated = original_value.DateCreated;
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexCategory");
            }
            return View(category);
        }

            
        [EncryptedActionParameter]

        public ActionResult DetailCategory(int? CategoryID)
        {
            Category categoryrec = db.Categorys.Find(CategoryID);
            if (CategoryID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (categoryrec == null)
            {
                return HttpNotFound();
            }
            return View(categoryrec);
        }

        [EncryptedActionParameter]
        public ActionResult DeleteCategory(int? CategoryID)
        {
            Category categoryrecs = db.Categorys.Find(CategoryID);
            if (CategoryID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (categoryrecs == null)
            {
                return HttpNotFound();
            }

            if (db.PartMasters.Where(x => x.CategoryID == CategoryID).Count() > 0)
            {
                ViewBag.CanDelete = "No";
                return View(categoryrecs);
            }

            return View(categoryrecs);
        }

        [HttpPost, ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult DeleteCategoryConfirmed(int CategoryID)
        {
            {
                Category categoryrecs = db.Categorys.Find(CategoryID);
                db.Categorys.Remove(categoryrecs);
                db.SaveChanges();
                return RedirectToAction("IndexCategory");
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

