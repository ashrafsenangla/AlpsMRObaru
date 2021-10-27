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
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using OfficeOpenXml.Style;
using OfficeOpenXml;

namespace AspNetGroupBasedPermissions.Areas.Part.Controllers
{
    public class PartTransferChargeOutController : Controller
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
                              join e in db.PartMakers
                              on d.PartID equals e.PartID
                              join f in db.PartMasters
                              on d.PartID equals f.PartID
                              join g in db.Categorys
                              on f.CategoryID equals g.CategoryID
                              join h in db.PartTransferChargeOuts
                              on d.PartID equals h.PartID
                              join i in db.Users
                              on h.UserCreated equals i.UserName
                              where b.UserId == userid
                              //&&  (SqlFunctions.DateName("day", h.DateCreated) + "/" + 
                              //    SqlFunctions.DateName("month", h.DateCreated) + "/" + 
                              //    SqlFunctions.DateName("year", h.DateCreated)  ==
                              //    SqlFunctions.DateName("day", DateTime.Now) + "/" +
                              //    SqlFunctions.DateName("month", DateTime.Now) + "/" +
                              //    SqlFunctions.DateName("year", DateTime.Now))
                              select new
                              {
                                  PartTransferOutID = h.PartTransferChargeOutID,
                                  PartName = h.RefPartID.PartName,
                                  Quantity = h.Qty,
                                  Amount = h.Qty * h.Price,
                                  PartNo = h.RefPartID.PartNo,
                                  ItemNo = h.RefPartID.ItemNo,
                                  CostCentreName = h.RefCabinetID.RefStoreID.RefSectionID.RefCostCentreID.CostCentreName,
                                  SectionName = h.RefCabinetID.RefStoreID.RefSectionID.SectionName,
                                  CategoryName = h.RefPartID.RefCategoryID.CategoryName,
                                  CabinetName = h.RefCabinetID.CabinetName,
                                  Description = h.Description,
                                  Status = h.Status,
                                  Price = h.Price,
                                  CreatedBy = i.FirstName,
                                  CreatedDate = h.DateCreated,
                                  IsActive = h.IsActive == true ? "Yes" : "No",
                              }).OrderBy(c => c.SectionName).ToList();

            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("MROUsage");

            workSheet.TabColor = System.Drawing.Color.Black;
            workSheet.DefaultRowHeight = 12;

            //Header of table
            workSheet.Row(1).Height = 30;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;
            workSheet.Row(1).Style.Font.Italic = true;

            workSheet.Cells[1, 3].Value = "MRO Usage Report";
            workSheet.Cells[1, 5].Value = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");

            workSheet.Row(3).Height = 20;
            workSheet.Row(3).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(3).Style.Font.Bold = true;

            workSheet.Cells[3, 1].Value = "No.";
            workSheet.Cells[3, 2].Value = "Section";
            workSheet.Cells[3, 3].Value = "Cost Centre";
            workSheet.Cells[3, 4].Value = "Part Name";
            workSheet.Cells[3, 5].Value = "Part No";
            workSheet.Cells[3, 6].Value = "Item No";
            workSheet.Cells[3, 7].Value = "Category";
            workSheet.Cells[3, 8].Value = "Description";
            workSheet.Cells[3, 9].Value = "Cabinet";
            workSheet.Cells[3, 10].Value = "Transfer Qty";
            workSheet.Cells[3, 11].Value = "Price";
            workSheet.Cells[3, 12].Value = "Inventory Amount";
            workSheet.Cells[3, 13].Value = "Created By";
            workSheet.Cells[3, 14].Value = "Created Date";

            int recordIndex = 2;
            int recordIndex1 = 4;
            decimal TotalAmt = 0;

            foreach (var inventory in inventorys)
            {
                workSheet.Cells[recordIndex1, 1].Value = (recordIndex - 1).ToString();
                workSheet.Cells[recordIndex1, 2].Value = inventory.SectionName;
                workSheet.Cells[recordIndex1, 3].Value = inventory.CostCentreName;
                workSheet.Cells[recordIndex1, 4].Value = inventory.PartName;
                workSheet.Cells[recordIndex1, 5].Value = inventory.PartNo;
                workSheet.Cells[recordIndex1, 6].Value = inventory.ItemNo;
                workSheet.Cells[recordIndex1, 7].Value = inventory.CategoryName;
                workSheet.Cells[recordIndex1, 8].Value = inventory.Description;
                workSheet.Cells[recordIndex1, 9].Value = inventory.CabinetName;
                workSheet.Cells[recordIndex1, 10].Value = inventory.Quantity;
                workSheet.Cells[recordIndex1, 11].Value = inventory.Price;
                workSheet.Cells[recordIndex1, 12].Value = inventory.Quantity * inventory.Price;
                workSheet.Cells[recordIndex1, 13].Value = inventory.CreatedBy;
                workSheet.Cells[recordIndex1, 14].Value = inventory.CreatedDate;

                TotalAmt = TotalAmt + Convert.ToDecimal(inventory.Quantity * inventory.Price);
                recordIndex++;
                recordIndex1++;
            }

            workSheet.Cells[recordIndex1 + 1, 3].Style.Font.Bold = true;
            workSheet.Cells[recordIndex1 + 1, 3].Style.Font.Color.SetColor(System.Drawing.Color.Red);

            workSheet.Cells[recordIndex1 + 1, 14].Style.Font.Bold = true;
            workSheet.Cells[recordIndex1 + 1, 14].Style.Font.Color.SetColor(System.Drawing.Color.Black);
            workSheet.Cells[recordIndex1 + 1, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[recordIndex1 + 1, 14].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);

            workSheet.Cells[recordIndex1 + 1, 3].Value = "Total";
            workSheet.Cells[recordIndex1 + 1, 14].Value = Convert.ToDecimal(TotalAmt);

            workSheet.Column(1).AutoFit();
            workSheet.Column(2).AutoFit();
            workSheet.Column(3).AutoFit();
            workSheet.Column(4).AutoFit();
            workSheet.Column(5).AutoFit();
            workSheet.Column(6).AutoFit();

            string excelName = "MROUsageReport" + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");

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
        public JsonResult DeletePartCO(int id)
        {
            PartTransferChargeOut partco = db.PartTransferChargeOuts.Find(id);
            db.PartTransferChargeOuts.Remove(partco);
            db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }

        // GET: /RTMIncorporation/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public async Task<ActionResult> Delete(int? id)
        {

            PartTransferChargeOut partco = await db.PartTransferChargeOuts.FindAsync(id);

            db.PartTransferChargeOuts.Remove(partco);
            db.SaveChanges();
            await db.SaveChangesAsync();
            return RedirectToAction("IndexPartTransferChargeOut");


        }

        public ActionResult CreatePartTransferCO(int? PartTransferChargeOutID)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(x => x.Id == userId);

            PartInv partinv = db.PartInvs.Find(PartTransferChargeOutID);
            if (PartTransferChargeOutID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (partinv == null)
            {
                return HttpNotFound();
            }

            ViewBag.PartName = partinv.RefPartMasterPartInvID.PartName;
            ViewBag.PartNo = partinv.RefPartMasterPartInvID.PartNo;
            ViewBag.ItemNo = partinv.RefPartMasterPartInvID.ItemNo;
            ViewBag.CurrentQty = partinv.StockInv;
            ViewBag.PartCategory = partinv.RefPartMasterPartInvID.RefCategoryID.CategoryName;
            ViewBag.MakerID = partinv.MakerID;
            ViewBag.VendorID = partinv.VendorID;
            ViewBag.MakerName = partinv.RefMakerID.MakerName;
            ViewBag.VendorName = partinv.RefVendorID.VendorName;
            ViewBag.PartID = partinv.PartID;
            ViewBag.Price = partinv.Price;
            ViewBag.CabinetID = partinv.CabinetID;

            var DetailCD = db.PartInvs.Include(x => x.RefCabinetID);
            ViewBag.FromCabinetID = DetailCD.Select(x => x.RefCabinetID.CabinetName).Distinct();

            return View("CreatePartTransferCO");
        }

        [HttpPost]
        //  [ValidateAntiForgeryToken]
        public ActionResult CreatePartTransferCO(PartTransferChargeOut parttranferco, decimal price, string balance, string makerid, string vendorid, string partid, string cabinetid, FormCollection fc)
        {

            parttranferco.PartID = Convert.ToInt32(partid);
            parttranferco.FromCabinetID = Convert.ToInt32(cabinetid);
            parttranferco.MakerID = Convert.ToInt32(makerid);
            parttranferco.VendorID = Convert.ToInt32(vendorid);
            parttranferco.TransferType = "In BU";
            parttranferco.Balance = Convert.ToInt32(balance);
            parttranferco.Price = price;
            parttranferco.IsActive = true;
            parttranferco.Status = "Draft";
            db.PartTransferChargeOuts.Add(parttranferco);
            db.SaveChanges();

            return RedirectToAction("IndexPartStoreInv");
        }


        [EncryptedActionParameter]
        public ActionResult IndexPartTransferChargeOutLog()
        {
            var DetailCD = db.PartTransferChargeOuts.Include(x => x.RefPartID).Include(y => y.RefCabinetID);
            ViewBag.Part = DetailCD.Select(x => x.RefPartID.PartName).Distinct();
            ViewBag.FromCabinet = DetailCD.Select(x => x.RefCabinetID.CabinetName).Distinct();
            ViewBag.Category = DetailCD.Select(x => x.RefPartID.RefCategoryID.CategoryName).Distinct();
            ViewBag.Status = DetailCD.Select(x => x.Status).Distinct();
            ViewBag.IsActive = DetailCD.Select(x => x.IsActive == true ? "Yes" : "No").Distinct();
            return View();
        }

        [HttpPost]
        public ActionResult GetPartTransferChargeOutLog(string search, int pageSize, int pageIndex,
              string cabinetname, string categoryname, string status, string isactive)
        {
            try
            {
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
                                 join h in db.PartTransferChargeOuts
                                 on d.PartID equals h.PartID
                                 join i in db.Users
                                 on h.UserCreated equals i.UserName
                                 where b.UserId == userid
                                 select new
                                 {
                                     PartTransferOutID = h.PartTransferChargeOutID,
                                     PartName = h.RefPartID.PartName,
                                     MakerName = h.RefMakerID.RefMakerID.MakerName,
                                     VendorName = h.RefVendorID.RefVendorID.VendorName,
                                     Quantity = h.Qty,
                                     Amount = h.Qty * h.Price,
                                     PartNo = h.RefPartID.PartNo,
                                     ItemNo = h.RefPartID.ItemNo,
                                     CategoryName = h.RefPartID.RefCategoryID.CategoryName,
                                     CabinetName = h.RefCabinetID.CabinetName,
                                     Description = h.Description,
                                     Status = h.Status,
                                     Price = h.Price,
                                     CreatedBy = i.FirstName,
                                     CreatedDate = h.DateCreated,
                                     IsActive = h.IsActive == true ? "Yes" : "No",
                                 }).OrderByDescending(c => c.PartTransferOutID);

                int totalRow = 0;
                int totalPage = 0;
                int totalCash = 0;
                decimal totalAmount = 0;
                int skip = pageIndex * pageSize;

                var models = detailrec.Where(x => (x.PartName.Contains(search))
                             && (x.Status == status || "All" == status)
                             && (x.CategoryName == categoryname || "All" == categoryname)
                             && (x.IsActive == isactive || "All" == isactive)
                             && (x.CabinetName == cabinetname || "All" == cabinetname))
                            .Skip(skip)
                            .Take(pageSize)
                            .ToList();
                totalRow = detailrec.Where(x => (x.Status.Contains(search))
                             && (x.Status == status || "All" == status)
                             && (x.IsActive == isactive || "All" == isactive)
                             && (x.CabinetName == cabinetname || "All" == cabinetname)).Count();

                totalCash = detailrec.Where(x => (x.Status.Contains(search))
                             && (x.Status == status || "All" == status)
                             && (x.IsActive == isactive || "All" == isactive)
                             && (x.CabinetName == cabinetname || "All" == cabinetname)) //.Sum(x => x.TotalAmount);
                          .Sum(uf => (int?)uf.Quantity) ?? 0;

                totalAmount = detailrec.Where(x => (x.Status.Contains(search))
                             && (x.Status == status || "All" == status)
                             && (x.IsActive == isactive || "All" == isactive)
                             && (x.CabinetName == cabinetname || "All" == cabinetname))
                           .Sum(uf => (decimal?)uf.Amount) ?? 0;


                totalPage = (totalRow + pageSize - 1) / pageSize;
                totalPage = totalPage == 0 ? 1 : totalPage;

                var hocs = models == null ? null :
                            models.Select(a =>
                             new
                             {
                                 a.PartTransferOutID,
                                 a.PartName,
                                 a.MakerName,
                                 a.VendorName,
                                 a.Quantity,
                                 a.Amount,
                                 a.PartNo,
                                 a.ItemNo,
                                 a.CabinetName,
                                 a.Description,
                                 a.CategoryName,
                                 a.Status,
                                 a.Price,
                                 a.CreatedBy,
                                 CreatedDate = Convert.ToString(a.CreatedDate),
                                 // CreatedDate = a.CreatedDate.ToString("dd/MM/yyyy"),
                                 a.IsActive,
                                 ShowButton = true,
                                 DetailURL = EncyptURLHelper.EncodedActionUrl("DetailPartTransferOut", "PartTransferOut", new { PartTransferOutID = a.PartTransferOutID, area = "PartTransferOut" }),
                                 EditURL = EncyptURLHelper.EncodedActionUrl("EditPartTransferOut", "PartTransferOut", new { PartTransferOutID = a.PartTransferOutID, area = "PartTransferOut" }),
                             });
                return Json(new { hocs = hocs, totalPage = totalPage, totalRow = totalRow, totalCash = totalCash, totalAmount = totalAmount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Console.WriteLine("Page Error. Contact System Administrator");
                throw;
            }
        }



        public ActionResult CreatePartTransferChargeOutX()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(x => x.Id == userId);

            var username = User.Identity.GetUserName();
            var query = (from a in db.UserWareHouses
                         join b in db.Stores
                         on a.WhouseID equals b.WhouseID
                         where a.UserId == userId
                         && b.IsActive == true
                         select new
                         {
                             b.WhouseID,
                             b.WhouseName
                         }).ToList();

            //            ViewBag.SectionID = query.Select(x => x.SectionName).Distinct();

            ViewBag.CategoryID = new SelectList(db.Categorys.Where(x => x.IsActive == true), "CategoryID", "CategoryName");
            ViewBag.FromCabinetID = new SelectList(db.Cabinets.Where(x => x.IsActive == true), "CabinetID", "CabinetName");

            ViewBag.PartID = new SelectList(db.PartMasters.Where(x => x.IsActive == true), "PartID", "PartName");

            return View("CreatePartTransferChargeOut");
        }


        [EncryptedActionParameter]
        public async Task<ActionResult> CreatePartTransferChargeOut(int? PartTransferChargeOutID)
        {
            PartInv partinv = await db.PartInvs.FindAsync(PartTransferChargeOutID);
            if (PartTransferChargeOutID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (partinv == null)
            {
                return HttpNotFound();
            }

            ViewBag.CategoryID = new SelectList(db.Categorys.Where(x => x.IsActive == true), "CategoryID", "CategoryName");
            ViewBag.PartName = new SelectList(db.PartMasters.Where(x => x.IsActive == true), "PartID", "PartName", partinv.RefPartMasterPartInvID.PartName);
            ViewBag.PartNo = new SelectList(db.PartMasters.Where(x => x.IsActive == true), "PartID", "PartNo", partinv.RefPartMasterPartInvID.PartNo);
            ViewBag.ItemNo = new SelectList(db.PartMasters.Where(x => x.IsActive == true), "PartID", "ItemNo", partinv.RefPartMasterPartInvID.ItemNo);
            //            ViewBag.CabinetID = new SelectList(db.Cabinets.Where(x => x.IsActive == true), "CabinetID", "CabinetName", parttransferoutrec.FromCabinetID);

            // return PartialView(model);
            return PartialView("CreatePartTransferChargeOut", partinv);
        }

        //   [EncryptedActionParameter]


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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePartTransferChargeOut(PartTransferChargeOut PartTransferChargeOut)
        {
            PartTransferChargeOut.IsActive = true;
            PartTransferChargeOut.Status = "Draft";
            PartTransferChargeOut.TransferType = "Charge Out";
            db.PartTransferChargeOuts.Add(PartTransferChargeOut);
            db.SaveChanges();

            return RedirectToAction("IndexPartTransferChargeOut");
        }

        public ActionResult PendingTransferCO()
        {
            string username = User.Identity.GetUserName();
            var model = from a in db.PartTransferChargeOuts
                        where a.UserCreated == username
                         && a.Status == "Draft"
                        select a;

            return PartialView(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PendingTransferCO(PartTransferChargeOut PartTransferChargeOut)
        {
            var username = User.Identity.GetUserName();
            var pendingCO = db.PartTransferChargeOuts.Where(x => x.UserCreated == username && x.Status == "Draft");

            string conn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection con = new SqlConnection(conn);
            con.Open();

            foreach (var co in pendingCO)
            {
                string UpdatePendingCO = "Update PartTransferChargeOut Set Status = 'Confirm' , " +
                "DateModified = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', " +
                "UserModified = '" + username + "' " +
                " Where PartTransferChargeOutID = '" + co.PartTransferChargeOutID + "'" +
                " And Status = 'Draft'";

                string UpdateInventory = "Update PartInv Set StockInv = StockInv - " + co.Qty + ", " +
                 "DateModified = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', " +
                 "UserModified = '" + username + "' " +
                 " Where PartID = " + co.PartID + "" +
                 " And CabinetID = " + co.FromCabinetID + "";

                SqlCommand cmd1 = new SqlCommand(UpdatePendingCO, con);
                SqlCommand cmd2 = new SqlCommand(UpdateInventory, con);

                try
                {
                    cmd1.ExecuteNonQuery();
                    cmd2.ExecuteNonQuery();
                    ViewBag.ErrorMsg = "Batch Has Been Submitted Successfully";

                }
                catch (Exception)
                {
                    Console.WriteLine("Page Error. Contact System Administrator");
                    throw;
                }
            }

            con.Close();
            return RedirectToAction("IndexPartStoreInv");
        }



        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //[EncryptedActionParameter]
        //public async Task<ActionResult> DeleteConfirmed(int id)
        //{
        //    RTMIncorporation rtmincorporation = await db.RTMIncorporations.FindAsync(id);
        //    db.RTMIncorporations.Remove(rtmincorporation);
        //    db.SaveChanges();
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}

        [EncryptedActionParameter]
        public ActionResult DeleteTransferCO(int? PartTransferChargeOutID)
        {
            PartTransferChargeOut pendingtransferCO = db.PartTransferChargeOuts.Find(PartTransferChargeOutID);
            if (pendingtransferCO == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (pendingtransferCO == null)
            {
                return HttpNotFound();
            }

            return View(pendingtransferCO);
        }


        [HttpPost, ActionName("DeleteTransferCO")]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult DeleteTransferCOConfirmed(int PartTransferChargeOutID)
        {
            {
                PartTransferChargeOut pendingtransferCO = db.PartTransferChargeOuts.Find(PartTransferChargeOutID);
                db.PartTransferChargeOuts.Remove(pendingtransferCO);
                db.SaveChanges();
                return RedirectToAction("PendingtranferCO");
            }
        }


        public ActionResult IndexPartStoreInv()
        {
            var userid = User.Identity.GetUserId();

            var DetailCD = db.PartInvs.Include(x => x.RefPartMasterPartInvID).Include(a => a.RefCabinetID.RefStoreID.RefSectionID.RefBUnitID).Include(b => b.RefCabinetID.RefStoreID.RefSectionID.RefBUnitID);
            ViewBag.Part = DetailCD.Select(x => x.RefPartMasterPartInvID.PartName).Distinct();
            ViewBag.Status = DetailCD.Select(x => x.Status).Distinct();
            ViewBag.BizUnit = DetailCD.Select(x => x.RefCabinetID.RefStoreID.RefSectionID.RefBUnitID.BusinessUnitName).Distinct();
            ViewBag.SectionName = DetailCD.Select(x => x.RefCabinetID.RefStoreID.RefSectionID.SectionName).Distinct();
            ViewBag.CategoryID = new SelectList(db.Categorys.Where(x => x.IsActive == true), "CategoryID", "CategoryName");

            var partinfo = (from a in db.Stores
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
                                CabinetName = d.RefCabinetID.CabinetName,
                                CategoryName = g.CategoryName,

                            }).ToList();

            ViewBag.Category = partinfo.Select(x => x.CategoryName).Distinct();
            ViewBag.Cabinet = partinfo.Select(x => x.CabinetName).Distinct();

            return View();
        }


        [HttpPost]
        public ActionResult GetPartStoreInv(string search, string SearchItemNo, string searchpartname, string searchpartmaker, string searchpartvendor, int pageSize, int pageIndex, string status,
                    string category, string partname, string cabinetname, string bizunit, string sectionname)
        {
            var userid = User.Identity.GetUserId();

            try
            {
                var detailrec = (from a in db.Stores
                                 join b in db.UserWareHouses
                                 on a.WhouseID equals b.WhouseID
                                 join c in db.Cabinets
                                 on a.WhouseID equals c.WhouseID
                                 join d in db.PartInvs
                                 on c.CabinetID equals d.CabinetID
                                 //join e in db.PartMakers
                                 //on d.PartID equals e.PartID
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
                                     Price = d.Price,
                                     MinQty = d.MinQty,
                                     MaxQty = d.MaxQty,
                                     ReOrderLevel = d.LowQty,
                                     LeadTime = d.LeadTime,
                                     BizUnitName = d.RefCabinetID.RefStoreID.RefSectionID.RefBUnitID.BusinessUnitName,
                                     SectionName = d.RefCabinetID.RefStoreID.RefSectionID.SectionName,
                                     Description = a.Description,
                                     Status = a.Status,
                                     IsActive = a.IsActive,
                                 }).OrderByDescending(c => c.PartInvID);

                int totalRow = 0;
                int totalPage = 0;
                int totalCash = 0;

                int skip = pageIndex * pageSize;

                var models = detailrec.Where(x => (x.PartName.Contains(searchpartname) && x.PartNo.Contains(search) && x.ItemNo.Contains(SearchItemNo) && x.MakerName.Contains(searchpartmaker) && x.VendorName.Contains(searchpartvendor))
                            //   && (x.PartName == partname || "All" == partname)
                            && (x.PartCategory == category || "All" == category)
                            && (x.CabinetName == cabinetname || "All" == cabinetname)
                            && (x.BizUnitName == bizunit || "All" == bizunit)
                            && (x.SectionName == sectionname || "All" == sectionname)
                            && (x.Status == status || "All" == status))
                            .Skip(skip)
                            .Take(pageSize)
                            .ToList();
                totalRow = detailrec.Where(x => (x.PartName.Contains(searchpartname) && x.PartNo.Contains(search) && x.ItemNo.Contains(SearchItemNo) && x.MakerName.Contains(searchpartmaker) && x.VendorName.Contains(searchpartvendor))
                            //   && (x.PartName == partname || "All" == partname)
                            && (x.PartCategory == category || "All" == category)
                            && (x.CabinetName == cabinetname || "All" == cabinetname)
                            && (x.BizUnitName == bizunit || "All" == bizunit)
                            && (x.SectionName == sectionname || "All" == sectionname)
                            && (x.Status == status || "All" == status)).Count();


                totalCash = detailrec.Where(x => (x.PartName.Contains(searchpartname) && x.PartNo.Contains(search) && x.ItemNo.Contains(SearchItemNo) && x.MakerName.Contains(searchpartmaker) && x.VendorName.Contains(searchpartvendor))
                             //  && (x.PartName == partname || "All" == partname)
                             && (x.PartCategory == category || "All" == category)
                            && (x.CabinetName == cabinetname || "All" == cabinetname)
                             && (x.BizUnitName == bizunit || "All" == bizunit)
                             && (x.SectionName == sectionname || "All" == sectionname)
                             && (status == "All" ? x.Status != "" : x.Status == status)) //.Sum(x => x.TotalAmount);
                            .Sum(uf => (int?)uf.Qty) ?? 0;

                totalPage = (totalRow + pageSize - 1) / pageSize;
                totalPage = totalPage == 0 ? 1 : totalPage;

                var hocs = models == null ? null :
                            models.Select(a =>
                             new
                             {
                                 a.PartInvID,
                                 a.PartNo,
                                 a.ItemNo,
                                 a.PartName,
                                 a.PartCategory,
                                 a.MakerName,
                                 a.VendorName,
                                 a.CabinetName,
                                 a.Qty,
                                 a.Price,
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
                                 DetailURL = EncyptURLHelper.EncodedActionUrl("DetailPartMaster", "PartTransferChargeOut", new { PartID = a.PartInvID, area = "Part" }),
                                 EditURL = EncyptURLHelper.EncodedActionUrl("EditPartInv", "PartInv", new { PartInvID = a.PartInvID, area = "PartInv" }),
                             });
                return Json(new { hocs = hocs, totalPage = totalPage, totalRow = totalRow, totalCash = totalCash }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Console.WriteLine("Page Error. Contact System Administrator");
                throw;
            }
        }



        [EncryptedActionParameter]
        public async Task<ActionResult> EditPartTransferChargeOut(int? PartTransferOutID)
        {
            PartTransferOut parttransferoutrec = await db.PartTransferOuts.FindAsync(PartTransferOutID);
            if (PartTransferOutID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (parttransferoutrec == null)
            {
                return HttpNotFound();
            }
            ViewBag.ToStoreID = new SelectList(db.Stores.Where(x => x.IsActive == true), "WhouseID", "WhouseName", parttransferoutrec.ToStoreID);
            ViewBag.PartID = new SelectList(db.PartMasters.Where(x => x.IsActive == true), "PartID", "PartName", parttransferoutrec.PartID);
            ViewBag.CabinetID = new SelectList(db.Cabinets.Where(x => x.IsActive == true), "CabinetID", "CabinetName", parttransferoutrec.FromCabinetID);

            return View("EditPartTransferChargeOut", parttransferoutrec);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult EditPartTransferChargeOut([Bind(Include = "PartTransferOutID, PartID, ToStoreID, Qty , FromCabinetID, Description, Status, IsActive, DateCreated, DateModified, UserCreated, UserModified")] PartTransferOut PartTransferOut)
        {
            if (ModelState.IsValid)
            {
                var original_value = db.PartTransferOuts.AsNoTracking().Where(P => P.PartTransferOutID == PartTransferOut.PartTransferOutID).FirstOrDefault();
                PartTransferOut.Status = original_value.Status;
                PartTransferOut.TransferType = original_value.TransferType;
                PartTransferOut.UserCreated = original_value.UserCreated;
                PartTransferOut.DateCreated = original_value.DateCreated;
                db.Entry(PartTransferOut).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("IndexPartTransferOut");
            }
            return View(PartTransferOut);
        }


        [EncryptedActionParameter]
        public async Task<ActionResult> EditPartTransferOutInternal(int? PartTransferOutID)
        {
            PartTransferOut parttransferoutrec = await db.PartTransferOuts.FindAsync(PartTransferOutID);
            if (PartTransferOutID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (parttransferoutrec == null)
            {
                return HttpNotFound();
            }
            ViewBag.ToStoreID = new SelectList(db.Stores.Where(x => x.IsActive == true), "WhouseID", "WhouseName", parttransferoutrec.ToStoreID);
            ViewBag.PartID = new SelectList(db.PartMasters.Where(x => x.IsActive == true), "PartID", "PartName", parttransferoutrec.PartID);
            ViewBag.FromCabinetID = new SelectList(db.Cabinets.Where(x => x.IsActive == true), "CabinetID", "CabinetName", parttransferoutrec.FromCabinetID);

            return View("EditPartTransferOut", parttransferoutrec);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult EditPartTransferOutInternal([Bind(Include = "PartTransferOutID, PartID, ToStoreID, Qty , FromCabinetID, Description, Status, IsActive, DateCreated, DateModified, UserCreated, UserModified")] PartTransferOut PartTransferOut)
        {
            if (ModelState.IsValid)
            {
                var original_value = db.PartTransferOuts.AsNoTracking().Where(P => P.PartTransferOutID == PartTransferOut.PartTransferOutID).FirstOrDefault();
                PartTransferOut.Status = original_value.Status;
                PartTransferOut.TransferType = original_value.TransferType;
                PartTransferOut.UserCreated = original_value.UserCreated;
                PartTransferOut.DateCreated = original_value.DateCreated;
                db.Entry(PartTransferOut).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("IndexPartTransferOut");
            }
            return View(PartTransferOut);
        }


        [EncryptedActionParameter]

        public ActionResult DetailPartTransferOut(int? PartTransferOutID)
        {
            PartTransferOut parttransferoutrec = db.PartTransferOuts.Find(PartTransferOutID);
            if (PartTransferOutID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (parttransferoutrec == null)
            {
                return HttpNotFound();
            }
            ViewBag.ToStoreID = new SelectList(db.Stores, "WhouseID", "WhouseName", parttransferoutrec.ToStoreID);
            ViewBag.PartID = new SelectList(db.PartMasters, "PartID", "PartName", parttransferoutrec.PartID);
            ViewBag.FromCabinetID = new SelectList(db.Cabinets, "CabinetID", "CabinetName", parttransferoutrec.FromCabinetID);

            return View(parttransferoutrec);
        }

        [EncryptedActionParameter]

        public ActionResult DetailPartTransferOutInternal(int? PartTransferOutID)
        {
            PartTransferOut parttransferoutrec = db.PartTransferOuts.Find(PartTransferOutID);
            if (PartTransferOutID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (parttransferoutrec == null)
            {
                return HttpNotFound();
            }
            ViewBag.ToStoreID = new SelectList(db.Stores, "WhouseID", "WhouseName", parttransferoutrec.ToStoreID);
            ViewBag.PartID = new SelectList(db.PartMasters, "PartID", "PartName", parttransferoutrec.PartID);
            ViewBag.FromCabinetID = new SelectList(db.Cabinets, "CabinetID", "CabinetName", parttransferoutrec.FromCabinetID);

            return View(parttransferoutrec);
        }

        [EncryptedActionParameter]
        public ActionResult DeletePartTransferOut(int? PartTransferOutID)
        {
            PartTransferOut parttransferoutrec = db.PartTransferOuts.Find(PartTransferOutID);
            if (PartTransferOutID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (parttransferoutrec == null)
            {
                return HttpNotFound();
            }
            ViewBag.ToStoreID = new SelectList(db.Stores, "WhouseID", "WhouseName", parttransferoutrec.ToStoreID);
            ViewBag.PartID = new SelectList(db.PartMasters, "PartID", "PartName", parttransferoutrec.PartID);
            ViewBag.FromCabinetID = new SelectList(db.Cabinets, "CabinetID", "CabinetName", parttransferoutrec.FromCabinetID);

            return View(parttransferoutrec);
        }

        [HttpPost, ActionName("DeletePartTransferOut")]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult DeletePartTransferOutConfirmed(int PartTransferOutID)
        {
            {
                PartTransferOut parttransferoutrec = db.PartTransferOuts.Find(PartTransferOutID);
                db.PartTransferOuts.Remove(parttransferoutrec);
                db.SaveChanges();
                return RedirectToAction("IndexPartTransferOut");
            }
        }

        [EncryptedActionParameter]
        public ActionResult DeletePartTransferOutInternal(int? PartTransferOutID)
        {
            PartTransferOut parttransferoutrec = db.PartTransferOuts.Find(PartTransferOutID);
            if (PartTransferOutID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (parttransferoutrec == null)
            {
                return HttpNotFound();
            }
            ViewBag.ToStoreID = new SelectList(db.Stores, "WhouseID", "WhouseName", parttransferoutrec.ToStoreID);
            ViewBag.PartID = new SelectList(db.PartMasters, "PartID", "PartName", parttransferoutrec.PartID);
            ViewBag.FromCabinetID = new SelectList(db.Cabinets, "CabinetID", "CabinetName", parttransferoutrec.FromCabinetID);

            return View(parttransferoutrec);
        }

        [HttpPost, ActionName("DeletePartTransferOut")]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult DeletePartTransferOutInternalConfirmed(int PartTransferOutID)
        {
            {
                PartTransferOut parttransferoutrec = db.PartTransferOuts.Find(PartTransferOutID);
                db.PartTransferOuts.Remove(parttransferoutrec);
                db.SaveChanges();
                return RedirectToAction("IndexPartTransferOut");
            }
        }


        public ActionResult GetPartInfo(int strSelected)
        {
            List<PartMaster> listPM = (from j in db.PartMasters
                                       where j.PartID == strSelected
                                       select j).ToList();
            PartMaster itemList = listPM.FirstOrDefault();
            var partNo = itemList.PartNo;

            List<PartMaker> listPMaker = (from j in db.PartMakers
                                          where j.PartID == strSelected
                                          select j).ToList();
            PartMaker itemListmaker = listPMaker.FirstOrDefault();
            var makername = itemListmaker.RefMakerID.MakerName;

            List<PartInv> listPInv = (from j in db.PartInvs
                                      where j.PartID == strSelected
                                      select j).ToList();
            PartInv itemInv = listPInv.FirstOrDefault();
            var qty = itemInv.StockInv;

            return Json(new { success = true, qty = qty, makername = makername, partno = partNo, message = "Success : " + strSelected }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult GetPartInfo2(string strSelected)
        {
            List<PartMaker> listPMaker = (from j in db.PartMakers
                                          where j.RefPartMasterID.PartNo == strSelected
                                          select j).ToList();
            PartMaker itemListmaker = listPMaker.FirstOrDefault();
            var makername = itemListmaker.RefMakerID.MakerName;

            List<PartInv> listPInv = (from j in db.PartInvs
                                      where j.RefPartMasterPartInvID.PartNo == strSelected
                                      select j).ToList();
            PartInv itemInv = listPInv.FirstOrDefault();
            var qty = itemInv.StockInv;

            return Json(new { success = true, qty = qty, makername = makername, message = "Success : " + strSelected }, JsonRequestBehavior.AllowGet);

        }



        [HttpPost]
        public ActionResult GetPartName(string partname, string partno, string itemno, string filterby)
        {
            if (filterby == "partname")
            {
                var pcname = db.PartMasters.Where(x => (x.PartName.ToUpper().Contains(partname.Trim().ToUpper()) || partname == "") && x.IsActive == true).Select(x => new { x.PartName, x.PartID, x.PartNo, x.ItemNo }).ToList();
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

                var pcname = db.PartMasters.Where(x => x.PartNo == partno && x.IsActive == true).Select(x => new { x.PartName, x.PartID, x.PartNo, x.ItemNo }).ToList();
                return Json(pcname, JsonRequestBehavior.AllowGet);

            }
            if (filterby == "itemno")
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

                var pcname = db.PartMasters.Where(x => x.ItemNo == itemno && x.IsActive == true).Select(x => new { x.PartName, x.PartID, x.PartNo, x.ItemNo }).ToList();
                return Json(pcname, JsonRequestBehavior.AllowGet);

            }

            return Json(JsonRequestBehavior.AllowGet);
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

