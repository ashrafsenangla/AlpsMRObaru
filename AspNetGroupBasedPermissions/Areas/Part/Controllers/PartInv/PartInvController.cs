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
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.IO;

namespace AspNetGroupBasedPermissions.Areas.Part.Controllers
{
    public class PartInvController : Controller
    {
        private Infrastructures.Data.ApplicationDbContext db = new Infrastructures.Data.ApplicationDbContext();


        public ActionResult ExportToExcel()
        {

            var userid = User.Identity.GetUserId();
            var inventorys = (from a in db.Stores
                              join b in db.UserWareHouses
                              on a.WhouseID equals b.WhouseID
                              join c in db.Cabinets
                              on a.WhouseID equals c.WhouseID
                              join d in db.PartInvs
                              on c.CabinetID equals d.CabinetID
                              //join e in db.PartMakers
                              //on d.PartID equals e.PartID
                              join f in db.PartMasters
                              on d.PartID equals f.PartID
                              join g in db.Categorys
                              on f.CategoryID equals g.CategoryID
                              where b.UserId == userid
                              select new
                              {
                                  PartInvID = d.PartInvID,
                                  PartNo = d.RefPartMasterPartInvID.PartNo,
                                  ItemNo = d.RefPartMasterPartInvID.ItemNo,
                                  PartName = d.RefPartMasterPartInvID.PartName,
                                  MakerName = d.RefMakerID.MakerName,
                                  VendorName = d.RefVendorID.VendorName,
                                  CabinetName = d.RefCabinetID.CabinetName,
                                  PartCategory = d.RefPartMasterPartInvID.RefCategoryID.CategoryName,
                                  Qty = d.StockInv,
                                  MinQty = d.MinQty,
                                  MaxQty = d.MaxQty,
                                  ReOrderLevel = d.LowQty,
                                  LeadTime = d.LeadTime,
                                  BizUnitName = d.RefCabinetID.RefStoreID.RefSectionID.RefBUnitID.BusinessUnitName,
                                  SectionName = d.RefCabinetID.RefStoreID.RefSectionID.SectionName,
                                  CostCenterName = d.RefCabinetID.RefStoreID.RefSectionID.RefCostCentreID.CostCentreName,
                                  Description = a.Description,
                                  Status = a.Status,
                                  Price = d.Price,
                                  Amount = d.Price * d.StockInv,
                                  IsActive = a.IsActive,
                              }).OrderBy(c => c.SectionName).ToList();

            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("MROInventory");

            workSheet.TabColor = System.Drawing.Color.Black;
            workSheet.DefaultRowHeight = 12;

            //Header of table
            workSheet.Row(1).Height = 30;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;
            workSheet.Row(1).Style.Font.Italic = true;

            workSheet.Cells[1, 3].Value = "MRO Inventory Report";
            workSheet.Cells[1, 5].Value = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");

            workSheet.Row(3).Height = 20;
            workSheet.Row(3).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(3).Style.Font.Bold = true;

            workSheet.Cells[3, 1].Value = "No.";
            workSheet.Cells[3, 2].Value = "Section";
            workSheet.Cells[3, 3].Value = "Cost Centre";
            workSheet.Cells[3, 4].Value = "Part No";
            workSheet.Cells[3, 5].Value = "Part Name";
            workSheet.Cells[3, 6].Value = "Stock Qty";
            workSheet.Cells[3, 7].Value = "Cabinet";
            workSheet.Cells[3, 8].Value = "Min Qty";
            workSheet.Cells[3, 9].Value = "Max Qty";
            workSheet.Cells[3, 10].Value = "Reorder Qty";
            workSheet.Cells[3, 11].Value = "Price";
            workSheet.Cells[3, 12].Value = "Inventory Amount";

            int recordIndex = 2;
            int recordIndex1 = 4;
            decimal TotalAmt = 0;

            foreach (var inventory in inventorys)
            {
                workSheet.Cells[recordIndex1, 1].Value = (recordIndex - 1).ToString();
                workSheet.Cells[recordIndex1, 2].Value = inventory.SectionName;
                workSheet.Cells[recordIndex1, 3].Value = inventory.CostCenterName;
                workSheet.Cells[recordIndex1, 4].Value = inventory.PartNo;
                workSheet.Cells[recordIndex1, 5].Value = inventory.PartName;
                workSheet.Cells[recordIndex1, 6].Value = inventory.Qty;
                workSheet.Cells[recordIndex1, 7].Value = inventory.CabinetName;
                workSheet.Cells[recordIndex1, 8].Value = inventory.MinQty;
                workSheet.Cells[recordIndex1, 9].Value = inventory.MaxQty;
                workSheet.Cells[recordIndex1, 10].Value = inventory.ReOrderLevel;
                workSheet.Cells[recordIndex1, 11].Value = inventory.Price;
                workSheet.Cells[recordIndex1, 12].Value = inventory.Qty * inventory.Price;

                TotalAmt = TotalAmt + Convert.ToDecimal(inventory.Qty * inventory.Price);
                recordIndex++;
                recordIndex1++;
            }

            workSheet.Cells[recordIndex1 + 1, 3].Style.Font.Bold = true;
            workSheet.Cells[recordIndex1 + 1, 3].Style.Font.Color.SetColor(System.Drawing.Color.Red);

            workSheet.Cells[recordIndex1 + 1, 12].Style.Font.Bold = true;
            workSheet.Cells[recordIndex1 + 1, 12].Style.Font.Color.SetColor(System.Drawing.Color.Black);
            workSheet.Cells[recordIndex1 + 1, 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[recordIndex1 + 1, 12].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);

            workSheet.Cells[recordIndex1 + 1, 3].Value = "Total";
            workSheet.Cells[recordIndex1 + 1, 12].Value = Convert.ToDecimal(TotalAmt);

            workSheet.Column(1).AutoFit();
            workSheet.Column(2).AutoFit();
            workSheet.Column(3).AutoFit();
            workSheet.Column(4).AutoFit();
            workSheet.Column(5).AutoFit();
            workSheet.Column(6).AutoFit();

            string excelName = "MROInventoryReport";

            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=" + excelName + ".xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }

            return View();
        }

        [HttpPost]
        public ActionResult GetMakerName(string makername,  string filterby)
        {
            if (filterby == "partname")
            {
                var pcname = db.Makers.Where(x => (x.MakerName.ToUpper().Contains(makername.Trim().ToUpper()) || makername == "") && x.IsActive == true).Select(x => new { x.MakerName, x.MakerID }).ToList();
                return Json(pcname, JsonRequestBehavior.AllowGet);
            }

            return Json(JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult GetVendorName(string vendorname, string filterby)
        {
            if (filterby == "partname")
            {
                var pcname = db.Vendors.Where(x => (x.VendorName.ToUpper().Contains(vendorname.Trim().ToUpper()) || vendorname == "") && x.IsActive == true).Select(x => new { x.VendorName, x.VendorID }).ToList();
                return Json(pcname, JsonRequestBehavior.AllowGet);
            }

            return Json(JsonRequestBehavior.AllowGet);
        }


        [EncryptedActionParameter]
        public ActionResult IndexPartInv()
        {
            var DetailCD = db.PartInvs.Include(x => x.RefPartMasterPartInvID).Include(a => a.RefCabinetID.RefStoreID.RefSectionID.RefBUnitID).Include(b => b.RefCabinetID.RefStoreID.RefSectionID.RefBUnitID);
            ViewBag.Part = DetailCD.Select(x => x.RefPartMasterPartInvID.PartName).Distinct();
            ViewBag.Category = DetailCD.Select(x => x.RefPartMasterPartInvID.RefCategoryID.CategoryName).Distinct();
            ViewBag.Cabinet = DetailCD.Select(x => x.RefCabinetID.CabinetName).Distinct();
            ViewBag.Status = DetailCD.Select(x => x.Status).Distinct();
            ViewBag.CostCentre = DetailCD.Select(x => x.RefCabinetID.RefStoreID.RefSectionID.RefCostCentreID.CostCentreName).Distinct();
            ViewBag.BizUnit = DetailCD.Select(x => x.RefCabinetID.RefStoreID.RefSectionID.RefBUnitID.BusinessUnitName).Distinct();
            ViewBag.SectionName = DetailCD.Select(x => x.RefCabinetID.RefStoreID.RefSectionID.SectionName).Distinct();

            return View();
        }


        [HttpPost]
        public ActionResult GetPartInv(string search, string searchpartno, string searchitemno, int pageSize, int pageIndex, string status,
                    string category,string searchmaker, string searchvendor, string searchpartid, string partname, string costcentername ,string cabinetname, string bizunit, string sectionname)
        {
            try
            {
                 var partidsearch = 0;
                if (searchpartid == "")
                {
                    partidsearch = 0;
                    searchpartid = "0";
                }
                else
                {
                    partidsearch = Convert.ToInt32(searchpartid);
                }

                var userid = User.Identity.GetUserId();
                var detailrec = (from a in db.Stores
                                 join b in db.UserWareHouses
                                 on a.WhouseID equals b.WhouseID
                                 join c in db.Cabinets
                                 on a.WhouseID equals c.WhouseID
                                 join d in db.PartInvs
                                 on c.CabinetID equals d.CabinetID
                                 //join e in db.PartMakers
                                 //on d.PartID equals e.PartID
                                 join f in db.PartMasters
                                 on d.PartID equals f.PartID
                                 join g in db.Categorys
                                 on f.CategoryID equals g.CategoryID
                                 where b.UserId == userid
                                 select new
                                 {
                                     PartInvID = d.PartInvID,
                                     //PartInvID = d.PartInvID,
                                     PartID = d.PartID,
                                     PartNo = d.RefPartMasterPartInvID.PartNo,
                                     ItemNo = d.RefPartMasterPartInvID.ItemNo,
                                     PartName = d.RefPartMasterPartInvID.PartName,
                                     CabinetName = d.RefCabinetID.CabinetName,
                                     PartCategory = d.RefPartMasterPartInvID.RefCategoryID.CategoryName,
                                     Qty = d.StockInv,
                                     MakerName = d.RefMakerID.MakerName,
                                     VendorName = d.RefVendorID.VendorName,
                                     MinQty = d.MinQty,
                                     MaxQty = d.MaxQty,
                                     ReOrderLevel = d.LowQty,
                                     LeadTime = d.LeadTime,
                                     BizUnitName = d.RefCabinetID.RefStoreID.RefSectionID.RefBUnitID.BusinessUnitName,
                                     SectionName = d.RefCabinetID.RefStoreID.RefSectionID.SectionName,
                                     CostCenterName = d.RefCabinetID.RefStoreID.RefSectionID.RefCostCentreID.CostCentreName,
                                     Description = a.Description,
                                     Status = a.Status,
                                     Price = d.Price,
                                     Amount = d.Price*d.StockInv,
                                     IsActive = a.IsActive,
                                 }).OrderByDescending(c => c.PartInvID);

                int totalRow = 0;
                int totalPage = 0;
                int totalCash = 0;
                decimal totalAmount = 0;

                int skip = pageIndex * pageSize;

                var models = detailrec.Where(x => (x.PartName.Contains(search) && x.PartNo.Contains(searchpartno) && x.ItemNo.Contains(searchitemno) && x.MakerName.Contains(searchmaker) && x.VendorName.Contains(searchvendor)) 
                            && (x.CostCenterName == costcentername || "All" == costcentername)
                            && (x.PartID == partidsearch || "0" == searchpartid)
                            && (x.PartCategory == category || "All" == category)
                            && (x.CabinetName == cabinetname || "All" == cabinetname)
                            && (x.BizUnitName == bizunit || "All" == bizunit)
                            && (x.SectionName == sectionname || "All" == sectionname)
                            && (x.Status == status ||  "All" == status))
                            .Skip(skip)
                            .Take(pageSize)
                            .ToList();
                totalRow = detailrec.Where(x => (x.PartName.Contains(search) && x.PartNo.Contains(searchpartno) && x.ItemNo.Contains(searchitemno) && x.MakerName.Contains(searchmaker) && x.VendorName.Contains(searchvendor))
                            && (x.CostCenterName == costcentername || "All" == costcentername)     
                            && (x.PartID == partidsearch || "0" == searchpartid)
                            && (x.PartCategory == category || "All" == category)
                            && (x.CabinetName == cabinetname || "All" == cabinetname)
                            && (x.BizUnitName == bizunit || "All" == bizunit)
                            && (x.SectionName == sectionname || "All" == sectionname)
                            && (x.Status == status || "All" == status)).Count();


                totalCash = detailrec.Where(x => (x.PartName.Contains(search) && x.PartNo.Contains(searchpartno) && x.ItemNo.Contains(searchitemno) && x.MakerName.Contains(searchmaker) && x.VendorName.Contains(searchvendor))
                            && (x.CostCenterName == costcentername || "All" == costcentername)
                            && (x.PartID == partidsearch || "0" == searchpartid)
                             && (x.PartCategory == category || "All" == category)
                            && (x.CabinetName == cabinetname || "All" == cabinetname)
                             && (x.BizUnitName == bizunit || "All" == bizunit)
                             && (x.SectionName == sectionname || "All" == sectionname)
                             && (status == "All" ? x.Status != "" : x.Status == status)) //.Sum(x => x.TotalAmount);
                            .Sum(uf => (int?)uf.Qty) ?? 0;

                totalAmount = detailrec.Where(x => (x.PartName.Contains(search) && x.PartNo.Contains(searchpartno) && x.ItemNo.Contains(searchitemno) && x.MakerName.Contains(searchmaker) && x.VendorName.Contains(searchvendor))
                         && (x.CostCenterName == costcentername || "All" == costcentername)
                             && (x.PartID == partidsearch || "0" == searchpartid)
                          && (x.PartCategory == category || "All" == category)
                         && (x.CabinetName == cabinetname || "All" == cabinetname)
                          && (x.BizUnitName == bizunit || "All" == bizunit)
                          && (x.SectionName == sectionname || "All" == sectionname)
                          && (status == "All" ? x.Status != "" : x.Status == status)) //.Sum(x => x.TotalAmount);
                         //.Sum(uf => ((int?)uf.Qty)*((decimal ?)uf.TotalAmount)) ?? 0;
                         .Sum(uf => (decimal?)uf.Amount) ?? 0;


                totalPage = (totalRow + pageSize - 1) / pageSize;
                totalPage = totalPage == 0 ? 1 : totalPage;

                var hocs = models == null ? null :
                            models.Select(a =>
                             new
                             {
                                 a.PartInvID,
                                 a.PartID,
                                 a.PartNo,
                                 a.ItemNo,
                                 a.PartName,
                                 a.PartCategory,
                                 a.MakerName,
                                 a.VendorName,
                                 a.CabinetName,
                                 a.CostCenterName,
                                 a.Qty,
                                 a.Price,
                                  Amount = a.Qty*a.Price,
                                 a.MinQty,
                                 a.MaxQty,
                                 a.ReOrderLevel,
                                 a.LeadTime,
                                 a.BizUnitName,
                                 a.SectionName,
                                 a.Description,
                                 a.Status,
                                 a.IsActive,
                                 ShowButton = true,
                                 DetailURL = EncyptURLHelper.EncodedActionUrl("DetailPartInv", "PartInv", new { PartInvID = a.PartInvID, area = "PartInv" }),
                                 EditURL = EncyptURLHelper.EncodedActionUrl("EditPartInv", "PartInv", new { PartInvID = a.PartInvID, area = "PartInv" }),
                             });
                return Json(new { hocs = hocs, totalPage = totalPage, totalRow = totalRow, totalCash = totalCash , totalAmount = totalAmount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Console.WriteLine("Page Error. Contact System Administrator");
                throw;
            }
        }



        [HttpPost]
        public ActionResult GetPartName(string partname, string partno, string filterby)
        {
            if (filterby == "partname")
            {
                var pcname = db.PartMasters.Where(x => (x.PartNo.ToUpper().Contains(partname.Trim().ToUpper()) || partname == "") && x.IsActive == true).Select(x => new { x.PartName, x.PartID, x.PartNo }).ToList();
                return Json(pcname, JsonRequestBehavior.AllowGet);
            }
            if (filterby == "partno")
            {
                List<PartMaker> listPMaker = (from j in db.PartMakers
                                              where j.PartID == 2
                                              select j).ToList();
                PartMaker itemListmaker = listPMaker.FirstOrDefault();
                var makername = itemListmaker.RefMakerID.MakerName;

                List<PartInv> listPInv = (from j in db.PartInvs
                                          where j.PartID == 2
                                          select j).ToList();
                PartInv itemInv = listPInv.FirstOrDefault();
                var qty = itemInv.StockInv;

                var pcname = db.PartMasters.Where(x => x.PartNo == partno && x.IsActive == true).Select(x => new { x.PartName, x.PartID, x.PartNo }).ToList();
                return Json(pcname, JsonRequestBehavior.AllowGet);

            }

            return Json(JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GetCabinetName(string cabinetname)
        {
                var pcname = db.Cabinets.Where(x => (x.CabinetName.ToUpper().Contains(cabinetname.Trim().ToUpper()) || cabinetname == "") && x.IsActive == true).Select(x => new { x.CabinetName, x.CabinetID }).ToList();
                return Json(pcname, JsonRequestBehavior.AllowGet);
        }



        public ActionResult CreatePartInv()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(x => x.Id == userId);
            ViewBag.PartID = new SelectList(db.PartMasters, "PartID", "PartName");
            ViewBag.MakerID = new SelectList(db.Makers, "MakerID", "MakerName");
            ViewBag.VendorID = new SelectList(db.Vendors, "VendorID", "VendorName");
            ViewBag.CabinetID = new SelectList(db.Cabinets, "CabinetID", "CabinetName");

            return View("CreatePartInv");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePartInv(PartInv partinv)
        {
            partinv.IsActive = true;
            partinv.Status = "Draft";
            db.PartInvs.Add(partinv);
            db.SaveChanges();
              
            return RedirectToAction("IndexPartInv");
        }


        [EncryptedActionParameter]
        public async Task<ActionResult> EditPartInv(int? PartInvID)
        {
            PartInv partinvrec = await db.PartInvs.FindAsync(PartInvID);
            if (PartInvID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (partinvrec == null)
            {
                return HttpNotFound();
            }
            ViewBag.PartID = new SelectList(db.PartMasters, "PartID", "PartName", partinvrec.PartID);
            ViewBag.CabinetID = new SelectList(db.Cabinets, "CabinetID", "CabinetName", partinvrec.CabinetID);
            ViewBag.MakerID = new SelectList(db.Makers, "MakerID", "MakerName", partinvrec.MakerID);
            ViewBag.VendorID = new SelectList(db.Vendors, "VendorID", "VendorName", partinvrec.VendorID);

            return View("EditPartInv", partinvrec);
        }

        public ActionResult CheckDuplicate(int partid, int makerid, int vendorid, int cabinetid)
        {
            
            if (db.PartInvs.Where(x => (x.PartID == partid) && (x.MakerID == makerid) && (x.VendorID == vendorid) && (x.CabinetID == cabinetid)).Count() > 0)
            {
                ViewBag.CanDelete = "No";

                return Json(new { success = false, message = "Duplicate Part Inv : " }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult EditPartInv([Bind(Include = "PartInvID, PartID, StockInv, Price, MinQty, MaxQty, MakerID, VendorID ,LowQty, LeadTime, CabinetID, Description, Status, IsActive, DateCreated, DateModified, UserCreated, UserModified")] PartInv partinv)
        {
            if (ModelState.IsValid)
            {
                var original_value = db.PartInvs.AsNoTracking().Where(P => P.PartID == partinv.PartID).FirstOrDefault();
                partinv.Status = original_value.Status;
                partinv.UserCreated = original_value.UserCreated;
                partinv.DateCreated = original_value.DateCreated;
                db.Entry(partinv).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.PartID = new SelectList(db.PartMasters, "PartID", "PartName", partinv.PartID);
                ViewBag.CabinetID = new SelectList(db.Cabinets, "CabinetID", "CabinetName", partinv.CabinetID);
                ViewBag.MakerID = new SelectList(db.Makers, "MakerID", "MakerName", partinv.MakerID);
                ViewBag.VendorID = new SelectList(db.Vendors, "VendorID", "VendorName", partinv.VendorID);

                return RedirectToAction("IndexPartInv");
            }
            return View(partinv);
        }

            
        [EncryptedActionParameter]

        public ActionResult DetailPartInv(int? PartInvID)
        {
            PartInv partinvrec = db.PartInvs.Find(PartInvID);
            if (PartInvID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (partinvrec == null)
            {
                return HttpNotFound();
            }
            ViewBag.PartID = new SelectList(db.PartMasters, "PartID", "PartName", partinvrec.PartID);
            ViewBag.CabinetID = new SelectList(db.Cabinets, "CabinetID", "CabinetName", partinvrec.CabinetID);
            ViewBag.MakerID = new SelectList(db.Makers, "MakerID", "MakerName", partinvrec.MakerID);
            ViewBag.VendorID = new SelectList(db.Vendors, "VendorID", "VendorName", partinvrec.VendorID);

            return View(partinvrec);
        }

        [EncryptedActionParameter]
        public ActionResult DeletePartInv(int? PartInvID)
        {
            PartInv partinvrec = db.PartInvs.Find(PartInvID);
            if (PartInvID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (partinvrec == null)
            {
                return HttpNotFound();
            }

            if (db.PartTransferChargeOuts.Where(x => x.PartID == PartInvID).Count() > 0)
            {
                ViewBag.CanDelete = "No";
                return View(partinvrec);
            }

            ViewBag.PartID = new SelectList(db.PartMasters, "PartID", "PartName", partinvrec.PartID);
            ViewBag.CabinetID = new SelectList(db.Cabinets, "CabinetID", "CabinetName", partinvrec.CabinetID);
            ViewBag.MakerID = new SelectList(db.Makers, "MakerID", "MakerName", partinvrec.MakerID);
            ViewBag.VendorID = new SelectList(db.Vendors, "VendorID", "VendorName", partinvrec.VendorID);

            return View(partinvrec);
        }

        [HttpPost, ActionName("DeletePartInv")]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult DeletePartInvConfirmed(int PartInvID)
        {
            {
                PartInv partinvrec = db.PartInvs.Find(PartInvID);
                db.PartInvs.Remove(partinvrec);
                db.SaveChanges();
                ViewBag.PartID = new SelectList(db.PartMasters, "PartID", "PartName", partinvrec.PartID);
                ViewBag.CabinetID = new SelectList(db.Cabinets, "CabinetID", "CabinetName", partinvrec.CabinetID);
                ViewBag.MakerID = new SelectList(db.Makers, "MakerID", "MakerName", partinvrec.MakerID);
                ViewBag.VendorID = new SelectList(db.Vendors, "VendorID", "VendorName", partinvrec.VendorID);

                return RedirectToAction("IndexPartInv");
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

