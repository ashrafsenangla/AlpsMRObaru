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
using Core.Entities.Data.PartRequest;
using OfficeOpenXml.Style;
using System.IO;
using OfficeOpenXml;
using System.Data.Entity.SqlServer;
using System.Windows.Media;

namespace AspNetGroupBasedPermissions.Areas.Part.Controllers
{
    public class PartTransferVendorInController : Controller
    {
        private Infrastructures.Data.ApplicationDbContext db = new Infrastructures.Data.ApplicationDbContext();


        public ActionResult ExportToExcel()
        {
            var currdate = DateTime.Now.ToString("dd/MMMM/yyyy");

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
                              join h in db.PartTransferVendorIns
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
                                  PartTransferVendorInID = h.PartTransferVendorInID,
                                  PartName = h.RefPartID.PartName,
                                  Quantity = h.Qty,
                                  NewPrice = h.NewPrice,
                                  Balance = h.Balance,
                                  PartNo = h.RefPartID.PartNo,
                                  ItemNo = h.RefPartID.ItemNo,
                                  CategoryName = h.RefPartID.RefCategoryID.CategoryName,
                                  SectionName = h.RefCabinetID.RefStoreID.RefSectionID.SectionName,
                                  CostCenterName = h.RefCabinetID.RefStoreID.RefSectionID.RefCostCentreID.CostCentreName,
                                  CabinetName = h.RefCabinetID.CabinetName,
                                  Description = a.Description,
                                  VendorName = h.RefVendorID.VendorName,
                                  Status = h.Status,
                                  Amount = h.Qty * h.NewPrice,
                                  CreatedBy = h.UserCreated,
                                  CreatedDate = h.DateCreated,
                                  IsActive = h.IsActive == true ? "Yes" : "No",
                              }).OrderBy(x => x.SectionName).ToList();

            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("MROPurchase");

            workSheet.TabColor = System.Drawing.Color.Black;
            workSheet.DefaultRowHeight = 12;

            //Header of table
            workSheet.Row(1).Height = 30;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;
            workSheet.Row(1).Style.Font.Italic = true;

            workSheet.Cells[1, 3].Value = "MRO Purchase Report";
            workSheet.Cells[1, 5].Value = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");

            workSheet.Row(3).Height = 20;
            workSheet.Row(3).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(3).Style.Font.Bold = true;

            workSheet.Cells[3, 1].Value = "No.";
            workSheet.Cells[3, 2].Value = "Section";
            workSheet.Cells[3, 3].Value = "Cost Centre";
            workSheet.Cells[3, 4].Value = "Part Name";
            workSheet.Cells[3, 5].Value = "Part No";
            workSheet.Cells[3, 6].Value = "Category";
            workSheet.Cells[3, 7].Value = "Vendor Name";
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
                workSheet.Cells[recordIndex1, 3].Value = inventory.CostCenterName;
                workSheet.Cells[recordIndex1, 4].Value = inventory.PartName;
                workSheet.Cells[recordIndex1, 5].Value = inventory.PartNo;
                workSheet.Cells[recordIndex1, 6].Value = inventory.ItemNo;
                workSheet.Cells[recordIndex1, 6].Value = inventory.CategoryName;
                workSheet.Cells[recordIndex1, 7].Value = inventory.VendorName;
                workSheet.Cells[recordIndex1, 8].Value = inventory.Description;
                workSheet.Cells[recordIndex1, 9].Value = inventory.CabinetName;
                workSheet.Cells[recordIndex1, 10].Value = inventory.Quantity;
                workSheet.Cells[recordIndex1, 11].Value = inventory.NewPrice;
                workSheet.Cells[recordIndex1, 12].Value = inventory.Quantity * inventory.NewPrice;
                workSheet.Cells[recordIndex1, 13].Value = inventory.CreatedBy;
                workSheet.Cells[recordIndex1, 14].Value = inventory.CreatedDate;

                TotalAmt = TotalAmt + Convert.ToDecimal(inventory.Quantity * inventory.NewPrice);
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

            string excelName = "MROPurchaseReport" + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");

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

        public JsonResult DeleteTransferVendorIn(int id)
        {
            PartTransferVendorIn partin = db.PartTransferVendorIns.Find(id);
            db.PartTransferVendorIns.Remove(partin);
            db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }

        // GET: /RTMIncorporation/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public async Task<ActionResult> Delete(int? id)
        {

            PartTransferVendorIn partin = await db.PartTransferVendorIns.FindAsync(id);

            db.PartTransferVendorIns.Remove(partin);
            db.SaveChanges();
            await db.SaveChangesAsync();
            return RedirectToAction("IndexPartTransferVendorIn");


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
        public ActionResult GetPartStoreInv(string search, string SearchItemNo, string searchpartname, string searchpartvendor, string searchpartmaker, int pageSize, int pageIndex, string status,
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
                                     PartID = d.PartID,
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
                                 a.PartID,
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
                                 DetailURL = EncyptURLHelper.EncodedActionUrl("DetailPartMaster", "PartTransferVendorIn", new { PartID = a.PartInvID, area = "Part" }),
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


        public ActionResult CreatePartTransferVendorIn(int? PartTransferChargeVendorID)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(x => x.Id == userId);

            PartInv partinv = db.PartInvs.Find(PartTransferChargeVendorID);

            if (PartTransferChargeVendorID == null)
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
            ViewBag.MakerName = partinv.RefMakerID.MakerName;
            ViewBag.VendorName = partinv.RefVendorID.VendorName;
            ViewBag.MakerID = partinv.MakerID;
            ViewBag.VendorID = partinv.VendorID;
            ViewBag.CabinetID = partinv.CabinetID;
            ViewBag.PartID = partinv.PartID;
            ViewBag.CabinetName = partinv.RefCabinetID.CabinetName;

            // var DetailCD = db.PartInvs.Include(x => x.RefCabinetID);
            // ViewBag.VendorID = new SelectList(db.Vendors.Where(x => x.IsActive == true), "VendorID", "VendorName");
            // ViewBag.PartID = new SelectList(db.PartMasters.Where(x => x.IsActive == true), "PartID", "PartName");

            return View("CreatePartTransferVendorIn");
        }


        [HttpPost]
        public ActionResult CreatePartTransferVendorIn(PartTransferVendorIn parttranfervendorin, string partid, string vendorid, string makerid, string cabinetid, FormCollection fc)
        {
            //PartInv partinv = db.PartInvs.Find(parttranfervendorin.PartID);
            //if (partinv == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //PartVendor partvendor = db.PartVendors.Find(parttranfervendorin.PartID);
            //if (partinv == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}


            parttranfervendorin.PartID = Convert.ToInt32(partid);
            parttranfervendorin.CabinetID = Convert.ToInt32(cabinetid);
            parttranfervendorin.MakerID = Convert.ToInt32(makerid);
            parttranfervendorin.VendorID = Convert.ToInt32(vendorid);
            parttranfervendorin.TransferType = "Vendor";
            parttranfervendorin.IsActive = true;
            parttranfervendorin.Status = "Draft";
            if (parttranfervendorin.Qty > 0 && parttranfervendorin.NewPrice > 0)
            {
                db.PartTransferVendorIns.Add(parttranfervendorin);
                db.SaveChanges();
            }

            return RedirectToAction("IndexPartStoreInv");
        }


        public ActionResult PendingTransferVendorIn()
        {
            string username = User.Identity.GetUserName();
            var model = from a in db.PartTransferVendorIns
                        where a.UserCreated == username
                         && a.Status == "Draft"
                        select a;

            return PartialView(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PendingTransferVendorIn(PartTransferVendorIn PartTransferVendorIn)
        {
            string conn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection con = new SqlConnection(conn);
            con.Open();

            var username = User.Identity.GetUserName();

            var pendingReceiving = db.PartTransferVendorIns.Where(x => x.UserCreated == username && x.Status == "Draft");

            foreach (var co in pendingReceiving)
            {

                string UpdatePendingReceiving = "Update PartTransferVendorIn Set Status = 'Confirm' , " +
                "DateModified = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', " +
                "UserModified = '" + username + "' " +
                " Where PartTransferVendorInID = '" + co.PartTransferVendorInID + "'" +
                " And Status = 'Draft'";

                string UpdateInventory = "Update PartInv Set StockInv = StockInv + " + co.Qty + ", " +
                 "DateModified = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', " +
                 "UserModified = '" + username + "', " +
                 "Price = " + co.NewPrice + " " +
                 " Where PartID = " + co.PartID + "" +
                 " And CabinetID = " + co.CabinetID +
                 " And VendorID = " + co.VendorID +
                 " And MakerID = " + co.MakerID + "";

                SqlCommand cmd1 = new SqlCommand(UpdatePendingReceiving, con);
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PendingTransferVendorInXXX(PartTransferVendorIn PartTransferVendorIn)
        {
            string conn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection con = new SqlConnection(conn);
            con.Open();

            var username = User.Identity.GetUserName();

            var pendingReceiving = db.PartTransferVendorIns.Where(x => x.UserCreated == username && x.Status == "Draft");

            foreach (var co in pendingReceiving)
            {
                PartVendor partvendor = db.PartVendors.Where(x => x.VendorID == co.VendorID && x.PartID == co.PartID).FirstOrDefault();
                if (partvendor == null)
                {
                    string InsertPartVendor = "Insert Into PartVendor (PartID, VendorID, Price, Status, Description, IsActive, UserCreated, UserModified, DateCreated, DateModified) values" +
                    "'" + co.PartID + "' " +
                    "'" + co.VendorID + "' " +
                    "'" + co.NewPrice + "' " +
                    "'" + co.Description + "' " +
                    "'" + co.Status + "' " +
                    "'" + co.IsActive + "' " +
                    "'" + username + "' " +
                    "'" + username + "' " +
                    "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                    "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ";

                }

                string UpdatePendingReceiving = "Update PartTransferVendorIn Set Status = 'Confirm' , " +
               "DateModified = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', " +
               "UserModified = '" + username + "' " +
               " Where PartTransferVendorInID = '" + co.PartTransferVendorInID + "'" +
               " And Status = 'Draft'";

                string UpdateInventory = "Update PartInv Set StockInv = StockInv + " + co.Qty + ", " +
                 "DateModified = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', " +
                 "UserModified = '" + username + "', " +
                 "Price = " + co.NewPrice + " " +
                 " Where PartID = " + co.PartID + "" +
                 " And CabinetID = " + co.CabinetID + "";

                string UpdateVendorPart = "Update PartVendor Set Price = " + co.NewPrice + ", " +
                "DateModified = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', " +
                "UserModified = '" + username + "' " +
                " Where PartID = " + co.PartID + "" +
                " And VendorId = " + co.VendorID + "";

                SqlCommand cmd1 = new SqlCommand(UpdatePendingReceiving, con);
                SqlCommand cmd2 = new SqlCommand(UpdateInventory, con);
                SqlCommand cmd3 = new SqlCommand(UpdateVendorPart, con);
                SqlCommand cmd4 = new SqlCommand(UpdateVendorPart, con);

                try
                {
                    cmd1.ExecuteNonQuery();
                    cmd2.ExecuteNonQuery();
                    cmd3.ExecuteNonQuery();
                    cmd4.ExecuteNonQuery();
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

        [EncryptedActionParameter]
        public ActionResult IndexPartTransferVendorInLog()
        {
            var DetailCD = db.PartTransferVendorIns.Include(x => x.RefPartID).Include(y => y.RefCabinetID);
            ViewBag.ToCabinet = DetailCD.Select(x => x.RefCabinetID.CabinetName).Distinct();
            ViewBag.Category = DetailCD.Select(x => x.RefPartID.RefCategoryID.CategoryName).Distinct();
            ViewBag.Status = DetailCD.Select(x => x.Status).Distinct();
            ViewBag.IsActive = DetailCD.Select(x => x.IsActive == true ? "Yes" : "No").Distinct();
            return View();
        }


        [HttpPost]
        public ActionResult GetPartTransferVendorInLog(string search, int pageSize, int pageIndex,
              string cabinetname, string categoryname, string partname, string status, string isactive)
        {
            try
            {
                var userid = User.Identity.GetUserId();

                //var detailrec = (from a in db.PartTransferVendorIns
                //                 join b in db.Users
                //                  on a.UserCreated equals b.UserName

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
                                 join h in db.PartTransferVendorIns
                                 on d.PartID equals h.PartID
                                 join i in db.Users
                                 on h.UserCreated equals i.UserName
                                 where b.UserId == userid
                                 && h.MakerID == d.MakerID
                                 && h.VendorID == d.VendorID
                                 select new
                                 {
                                     PartTransferVendorInID = h.PartTransferVendorInID,
                                     PartName = h.RefPartID.PartName,
                                     Quantity = h.Qty,
                                     NewPrice = h.NewPrice,
                                     Balance = h.Balance,
                                     PartNo = h.RefPartID.PartNo,
                                     ItemNo = h.RefPartID.ItemNo,
                                     CategoryName = h.RefPartID.RefCategoryID.CategoryName,
                                     CabinetName = h.RefCabinetID.CabinetName,
                                     Description = a.Description,
                                     VendorName = h.RefVendorID.VendorName,
                                     Status = h.Status,
                                     Amount = h.Qty * h.NewPrice,
                                     // CreatedBy = i.FirstName,
                                     CreatedBy = h.UserCreated,
                                     CreatedDate = h.DateCreated,
                                     IsActive = h.IsActive == true ? "Yes" : "No",
                                 }).OrderByDescending(x => x.PartTransferVendorInID);

                int totalRow = 0;
                int totalPage = 0;
                int skip = pageIndex * pageSize;
                int totalCash = 0;
                decimal totalAmount = 0;

                var models = detailrec.Where(x => (x.PartName.Contains(search))
                             && (x.Status == status || "All" == status)
                             && (x.CategoryName == categoryname || "All" == categoryname)
                             && (x.IsActive == isactive || "All" == isactive)
                             && (x.CabinetName == cabinetname || "All" == cabinetname))
                            .Skip(skip)
                            .Take(pageSize)
                            .ToList();
                totalRow = detailrec.Where(x => (x.PartName.Contains(search))
                             && (x.Status == status || "All" == status)
                             && (x.CategoryName == categoryname || "All" == categoryname)
                             && (x.IsActive == isactive || "All" == isactive)
                             && (x.CabinetName == cabinetname || "All" == cabinetname)).Count();

                totalCash = detailrec.Where(x => (x.PartName.Contains(search))
                          && (x.Status == status || "All" == status)
                          && (x.IsActive == isactive || "All" == isactive)
                          && (x.CabinetName == cabinetname || "All" == cabinetname)) //.Sum(x => x.TotalAmount);
                       .Sum(uf => (int?)uf.Quantity) ?? 0;

                totalAmount = detailrec.Where(x => (x.PartName.Contains(search))
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
                                 a.PartTransferVendorInID,
                                 a.PartName,
                                 a.Quantity,
                                 a.NewPrice,
                                 a.Amount,
                                 a.Balance,
                                 a.PartNo,
                                 a.ItemNo,
                                 a.CabinetName,
                                 a.VendorName,
                                 a.Description,
                                 a.Status,
                                 a.CategoryName,
                                 a.CreatedBy,
                                 CreatedDate = Convert.ToString(a.CreatedDate),
                                 a.IsActive,
                                 ShowButton = true,
                                 DetailURL = EncyptURLHelper.EncodedActionUrl("DetailPartTransferOut", "PartTransferOut", new { PartTransferOutID = a.PartTransferVendorInID, area = "PartTransferOut" }),
                                 EditURL = EncyptURLHelper.EncodedActionUrl("EditPartTransferOut", "PartTransferOut", new { PartTransferOutID = a.PartTransferVendorInID, area = "PartTransferOut" }),
                             });
                return Json(new { hocs = hocs, totalPage = totalPage, totalRow = totalRow, totalCash = totalCash, totalAmount = totalAmount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Console.WriteLine("Page Error. Contact System Administrator");
                throw;
            }
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
        public ActionResult DeleteTransferVendorIn(int? PartTransferVendorInID)
        {

            PartTransferVendorIn pendingtransfervendorin = db.PartTransferVendorIns.Find(PartTransferVendorInID);
            if (PartTransferVendorInID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (pendingtransfervendorin == null)
            {
                return HttpNotFound();
            }

            return View(pendingtransfervendorin);
        }


        [HttpPost, ActionName("DeleteTransferVendorIn")]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult DeleteTransferVendorInConfirmed(int PartTransferVendorInID)
        {
            {
                //Console.WriteLine("parameter=" + PartTransferVendorInID);
                PartTransferVendorIn pendingtransfersvendorin = db.PartTransferVendorIns.Find(PartTransferVendorInID);
                db.PartTransferVendorIns.Remove(pendingtransfersvendorin);
                db.SaveChanges();
                return RedirectToAction("PendingTransfersVendorIn");
            }
        }


        [HttpPost]
        public ActionResult GetVendorName(string vendorname)
        {
            var pcname = db.Vendors.Where(x => (x.VendorName.ToUpper().Contains(vendorname.Trim().ToUpper()) || vendorname == "") && x.IsActive == true).Select(x => new { x.VendorName }).ToList();
            return Json(pcname, JsonRequestBehavior.AllowGet);
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

