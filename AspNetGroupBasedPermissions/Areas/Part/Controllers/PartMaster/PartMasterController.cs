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
    public class PartMasterController : Controller
    {
        private Infrastructures.Data.ApplicationDbContext db = new Infrastructures.Data.ApplicationDbContext();

        [EncryptedActionParameter]

        public ActionResult IndexPartMaster()
        {
            var DetailCD = db.PartMasters.Include(x => x.RefCategoryID);
            ViewBag.Category = DetailCD.Select(x => x.RefCategoryID.CategoryName).Distinct();
            ViewBag.Status = DetailCD.Select(x => x.Status).Distinct();
            return View();
        }


        [HttpPost]
        public ActionResult GetPartMaster(string search, string searchpartno ,int pageSize, int pageIndex, 
            string category, string status)
        {
            try
            {
                var detailrec = (from a in db.PartMasters
                                 select new
                                 {
                                     PartID = a.PartID,
                                     PartName = a.PartName,
                                     PartNo = a.PartNo,
                                     ItemNo = a.ItemNo,
                                     Category = a.RefCategoryID.CategoryName,
                                     Description = a.Description,
                                     Status = a.Status,
                                     IsActive = a.IsActive,
                                 }).OrderByDescending(c => c.PartID);

                int totalRow = 0;
                int totalPage = 0;
                int skip = pageIndex * pageSize;

                var models = detailrec.Where(x => (x.PartName.Contains(search) && x.PartNo.Contains(searchpartno))
                             && (x.Category == category || "All" == category)
                             && (x.Status == status ||  "All" == status))
                            .Skip(skip)
                            .Take(pageSize)
                            .ToList();
                totalRow = detailrec.Where(x => (x.PartName.Contains(search) && x.PartNo.Contains(searchpartno))
                            && (x.Category == category || "All" == category)
                            && (x.Status == status || "All" == status)).Count();
           
                totalPage = (totalRow + pageSize - 1) / pageSize;
                totalPage = totalPage == 0 ? 1 : totalPage;

                var hocs = models == null ? null :
                            models.Select(a =>
                             new
                             {
                                 a.PartID,
                                 a.PartNo,
                                 a.ItemNo,
                                 a.PartName,
                                 a.Description,
                                 a.Category,
                                 a.Status,
                                 a.IsActive,
                                 ShowButton = true,
                                 DetailURL = EncyptURLHelper.EncodedActionUrl("DetailPartMaster", "PartMaster", new { PartID = a.PartID, area = "PartMaster" }),
                                 EditURL = EncyptURLHelper.EncodedActionUrl("EditPartMaster", "PartMaster", new { PartID = a.PartID, area = "PartMaster" }),
                             });
                return Json(new { hocs = hocs, totalPage = totalPage,  totalRow = totalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Console.WriteLine("Page Error. Contact System Administrator");
                throw;
            }
        }

        public ActionResult CreatePartMaster()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(x => x.Id == userId);
            ViewBag.CategoryID = new SelectList(db.Categorys, "CategoryID", "CategoryName");

            return View("CreatePartMaster");
        }

        [HttpPost]
      //  [ValidateAntiForgeryToken]
        public ActionResult CreatePartMaster (PartMaster PartMaster, HttpPostedFileBase File1, HttpPostedFileBase File2)
        {
            if (File1 != null)
            {
                string ImageFileExt = System.IO.Path.GetExtension(Request.Files["File1"].FileName);
                PartMaster.PartImage = new byte[File1.ContentLength];
                File1.InputStream.Read(PartMaster.PartImage, 0, File1.ContentLength);
                string ImageName1 = System.IO.Path.GetFileName(File1.FileName);
                string physicalPath1 = Server.MapPath("~/FileUpload/PartMaster/Image/" + ImageName1);
                File1.SaveAs(physicalPath1);
                PartMaster.PartImagePath = "FileUpload/PartMaster/Image/" + ImageName1;

                if (ImageFileExt.ToUpper() == ".PDF")
                { PartMaster.PartImageFileType = "PDF"; }
                else
                { PartMaster.PartImageFileType = "Image"; }

            }

            if (File2 != null)
            {
                string DrawingfileExt = System.IO.Path.GetExtension(Request.Files["File2"].FileName);
                PartMaster.PartDrawingImage = new byte[File2.ContentLength];
                File2.InputStream.Read(PartMaster.PartDrawingImage, 0, File2.ContentLength);
                string ImageName2 = System.IO.Path.GetFileName(File2.FileName);
                string physicalPath2 = Server.MapPath("~/FileUpload/PartMaster/Drawing/" + ImageName2);
                File2.SaveAs(physicalPath2);
                PartMaster.PartDrawingPath = "FileUpload/PartMaster/Drawing/" + ImageName2;

                if (DrawingfileExt.ToUpper() == ".PDF")
                { PartMaster.PartDrawingFileType = "PDF"; }
                else
                { PartMaster.PartDrawingFileType = "Image"; }

            }

            var existingRec = db.PartMasters.FirstOrDefault(x => x.PartNo == PartMaster.PartNo);
            var isDuplicateRec = existingRec != null;
            if (isDuplicateRec)
            {
                ModelState.AddModelError("", "Part No. " + PartMaster.PartNo + " Already Exists !!!");
            }
            else
            {
                PartMaster.IsActive = true;
                PartMaster.Status = "Draft";
                db.PartMasters.Add(PartMaster);
                db.SaveChanges();
                return RedirectToAction("IndexPartMaster");
            }
          
            return RedirectToAction("IndexPartMaster");
        }


        [HttpPost]
        public ActionResult GetCategoryName(string categoryname)
        {
                var pcname = db.Categorys.Where(x => (x.CategoryName.ToUpper().Contains(categoryname.Trim().ToUpper()) || categoryname == "") && x.IsActive == true).Select(x => new { x.CategoryName, x.CategoryID }).ToList();
                return Json(pcname, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CheckDuplicate(string partno)
        {

            if (db.PartMasters.Where(x => x.PartNo == partno).Count() > 0)
            {
                ViewBag.CanDelete = "No";

                return Json(new { success = false, message = "Duplicate Part No : " }, JsonRequestBehavior.AllowGet);
            }
           
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }


        [EncryptedActionParameter]
        public async Task<ActionResult> EditPartMaster(int? PartID)
        {
            PartMaster partmasterrec = await db.PartMasters.FindAsync(PartID);
            if (PartID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (partmasterrec == null)
            {
                return HttpNotFound();
            }

            ViewData["Image"] = partmasterrec.PartImage;
            ViewData["Drawing"] = partmasterrec.PartDrawingImage;

            ViewBag.CategoryID = new SelectList(db.Categorys, "CategoryID", "CategoryName", partmasterrec.CategoryID);
            ViewBag.PartImageFileType = partmasterrec.PartImageFileType;
            ViewBag.PartDrawingFileType = partmasterrec.PartDrawingFileType;

            return View("EditPartMaster", partmasterrec);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult EditPartMaster([Bind(Include = "PartID, PartName, ItemNo, PartNo , PartImageFileType, PartDrawingFileType, CategoryID, Description, Status, IsActive, DateCreated, DateModified, UserCreated, UserModified")] PartMaster partmaster,
             HttpPostedFileBase File1, HttpPostedFileBase File2)
        {
           

                if (File1 != null ) 
                {
                    partmaster.PartImage = new byte[File1.ContentLength];
                    string ImageFileExt = System.IO.Path.GetExtension(Request.Files["File1"].FileName);
                    File1.InputStream.Read(partmaster.PartImage, 0, File1.ContentLength);
                    string ImageName1 = System.IO.Path.GetFileName(File1.FileName);
                    string physicalPath1 = Server.MapPath("~/FileUpload/PartMaster/Image/" + ImageName1);
                    File1.SaveAs(physicalPath1);
                    partmaster.PartImagePath = "FileUpload/PartMaster/Image/" + ImageName1;

                    if (ImageFileExt.ToUpper() == ".PDF")
                    { partmaster.PartImageFileType = "PDF"; }
                    else
                    { partmaster.PartImageFileType = "Image"; }

                }

                if (File2 != null)
                {
                    partmaster.PartDrawingImage = new byte[File2.ContentLength];
                    string DrawingfileExt = System.IO.Path.GetExtension(Request.Files["File2"].FileName);
                    File2.InputStream.Read(partmaster.PartDrawingImage, 0, File2.ContentLength);
                    string ImageName2 = System.IO.Path.GetFileName(File2.FileName);
                    string physicalPath2 = Server.MapPath("~/FileUpload/PartMaster/Drawing/" + ImageName2);
                    File2.SaveAs(physicalPath2);
                    partmaster.PartDrawingPath = "FileUpload/PartMaster/Drawing/" + ImageName2;

                    if (DrawingfileExt.ToUpper() == ".PDF")
                    {partmaster.PartDrawingFileType = "PDF";}
                    else
                    {partmaster.PartDrawingFileType = "Image";}
                }

                var original_value = db.PartMasters.AsNoTracking().Where(P => P.PartID == partmaster.PartID).FirstOrDefault();
                if (File1 == null || File1.ContentLength < 0)
                {
                    partmaster.PartImage = original_value.PartDrawingImage;
                    partmaster.PartImageFileType = original_value.PartImageFileType;
                    partmaster.PartImagePath = original_value.PartImagePath;
                }

                if (File2 == null || File2.ContentLength < 0)
                {
                    partmaster.PartDrawingImage = original_value.PartDrawingImage;
                    partmaster.PartDrawingFileType = original_value.PartDrawingFileType;
                    partmaster.PartDrawingPath = original_value.PartDrawingPath;
                }

            
                partmaster.Status = original_value.Status;
                partmaster.UserCreated = original_value.UserCreated;
                partmaster.DateCreated = original_value.DateCreated;
                db.Entry(partmaster).State = EntityState.Modified;
                db.SaveChanges();

                ViewBag.CategoryID = new SelectList(db.Categorys, "CategoryID", "CategoryName", partmaster.CategoryID);
                return RedirectToAction("IndexPartMaster");
        }

            
        [EncryptedActionParameter]

        public ActionResult DetailPartMaster(int? PartID)
        {
            PartMaster partmasterrec = db.PartMasters.Find(PartID);
            if (PartID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (partmasterrec == null)
            {
                return HttpNotFound();
            }

            ViewData["Image"] = partmasterrec.PartImage;
            ViewData["Drawing"] = partmasterrec.PartDrawingImage;

            ViewBag.CategoryID = new SelectList(db.Categorys, "CategoryID", "CategoryName", partmasterrec.CategoryID);

            return PartialView(partmasterrec);
        }

        [EncryptedActionParameter]
        public ActionResult DeletePartMaster(int? PartID)
        {
            PartMaster partmasterrec = db.PartMasters.Find(PartID);
            if (PartID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (partmasterrec == null)
            {
                return HttpNotFound();
            }


            ViewData["Image"] = partmasterrec.PartImage;
            ViewData["Drawing"] = partmasterrec.PartDrawingImage;

            ViewBag.CategoryID = new SelectList(db.Categorys, "CategoryID", "CategoryName", partmasterrec.CategoryID);

            if (db.PartMakers.Where(x => x.PartID == PartID).Count() > 0)
            {
                ViewBag.CanDelete = "No";
                return View(partmasterrec);
            }
            if (db.PartInvs.Where(x => x.PartID == PartID).Count() > 0)
            {
                ViewBag.CanDelete = "No";
                return View(partmasterrec);
            }

            return View(partmasterrec);
        }

        [HttpPost, ActionName("DeletePartMaster")]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult DeletePartMasterConfirmed(int PartID)
        {
            {
                PartMaster partmasterrec = db.PartMasters.Find(PartID);
                db.PartMasters.Remove(partmasterrec);
                db.SaveChanges();
                ViewBag.CategoryID = new SelectList(db.Categorys, "CategoryID", "CategoryName", partmasterrec.CategoryID);

                return RedirectToAction("IndexPartMaster");
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

