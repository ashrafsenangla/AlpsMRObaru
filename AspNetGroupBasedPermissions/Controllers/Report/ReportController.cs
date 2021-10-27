using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Net;
using AspNetGroupBasedPermissions.Filters;
using AspNetGroupBasedPermissions.Models;


namespace AspNetGroupBasedPermissions.Controllers.Report
{
    public class ReportController : Controller
    {
        private Infrastructures.Data.ApplicationDbContext db = new Infrastructures.Data.ApplicationDbContext();

      

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RFQ(FormCollection form)
        {
            string currentUserId = User.Identity.GetUserId();

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Remote;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);
            
            reportViewer.ServerReport.ReportPath = "/EPRReport/RFQ";
            reportViewer.ServerReport.ReportServerUrl = new Uri(GetReportURL()); //ConfigurationManager.AppSettings["ReportURL"];

            string xMSRNumber = form["MSRRFQNo"];
            string xVendorCode = form["VendorCode"];
            string xCompanyCode = form["CompanyCode"];
            reportViewer.ServerReport.SetParameters(RFQParameter(xMSRNumber, xVendorCode, xCompanyCode));

            //testing purpose//
            //reportViewer.ServerReport.SetParameters(RFQParameter("1611-0003-HR-001", "ASB001", "UOG"));

            Warning[] warnings;
            string[] streamids;
            string mimeType, encoding, extension;

            byte[] bytes = reportViewer.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);
            
            return File(bytes, mimeType, xMSRNumber+".pdf");
        }

        private ReportParameter[] RFQParameter(string MSRNumber, string VendorCode, string CompanyCode)
        {
            ReportParameter p1 = new ReportParameter("MSRsRFQsNumber", MSRNumber);
            ReportParameter p2 = new ReportParameter("VendorCode", VendorCode);
            ReportParameter p3 = new ReportParameter("CompanyCode", CompanyCode);

            return new ReportParameter[] { p1, p2, p3 };
        }
        private ReportParameter[] RFQParameter(string MSRNumber, string VendorCode)
        {
            ReportParameter p1 = new ReportParameter("MSRsRFQsNumber", MSRNumber);
            ReportParameter p2 = new ReportParameter("VendorCode", VendorCode);

            return new ReportParameter[] { p1, p2 };
        }
        [HttpGet]
        [EncryptedActionParameter]
        public ActionResult DownloadRFQ(string MSRRFQNo, string VendorCode, string CompanyCode)
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Remote;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);

            reportViewer.ServerReport.ReportPath = "/EPRReport/RFQ";
            reportViewer.ServerReport.ReportServerUrl = new Uri(GetReportURL());

            string xMSRNumber = MSRRFQNo;
            string xVendorCode = VendorCode.ToString();
            string xCompanyCode = CompanyCode;
            reportViewer.ServerReport.SetParameters(RFQParameter(xMSRNumber, xVendorCode, xCompanyCode));

            Warning[] warnings;
            string[] streamids;
            string mimeType, encoding, extension;

            byte[] bytes = reportViewer.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);

            return File(bytes, mimeType, xMSRNumber + ".pdf");
        }

        [HttpGet]
        [EncryptedActionParameter]
        public ActionResult DownloadRFQVA(string MSRRFQNo, int VendorCode)
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Remote;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);

            reportViewer.ServerReport.ReportPath = "/EPRReport/RFQ_VendorApp";
            reportViewer.ServerReport.ReportServerUrl = new Uri(GetReportURL());

            string xMSRNumber = MSRRFQNo;
            string xVendorCode = VendorCode.ToString();
            reportViewer.ServerReport.SetParameters(RFQParameter(xMSRNumber, xVendorCode));

            Warning[] warnings;
            string[] streamids;
            string mimeType, encoding, extension;

            byte[] bytes = reportViewer.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);

            return File(bytes, mimeType, xMSRNumber + ".pdf");
        }
        [HttpGet]
        [EncryptedActionParameter]
        public ActionResult DownloadRFQPreview(string MSRRFQNo, string CompanyCode)
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Remote;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);

            reportViewer.ServerReport.ReportPath = "/EPRReport/RFQPreview";
            reportViewer.ServerReport.ReportServerUrl = new Uri(GetReportURL());


            string xMSRNumber = MSRRFQNo;
            string xCompanyCode = CompanyCode;

            ReportParameter p1 = new ReportParameter("MSRsRFQsNumber", xMSRNumber);
            ReportParameter p2 = new ReportParameter("CompanyCode", xCompanyCode);

            var reportParam = new ReportParameter[] { p1, p2 };

            reportViewer.ServerReport.SetParameters(reportParam);

            Warning[] warnings;
            string[] streamids;
            string mimeType, encoding, extension;

            byte[] bytes = reportViewer.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);

            return File(bytes, mimeType, xMSRNumber + ".pdf");
        }
        private string GetReportURL()
        {
            var isProd = ConfigurationManager.AppSettings["IsProd"];
            if (isProd != "true")
                return ConfigurationManager.AppSettings["ReportURL"];
                //return ConfigurationManager.AppSettings["ReportURLPROD"];
            else
                return ConfigurationManager.AppSettings["ReportURLPROD"];
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MSRTracking(FormCollection collection)

        {

            DateTime StDateStr = Convert.ToDateTime(collection["StartDate"]);
            DateTime EndDateStr = Convert.ToDateTime(collection["EndDate"]);


            string StDate = StDateStr.ToString("yyyy-MM-dd");
            string EndDate = EndDateStr.ToString("yyyy-MM-dd"); 


            try
            {

                string xcostcentre = collection["CostCentreCode"] == "" ? "0" : collection["CostCentreCode"];
                string xcompanycode = collection["CompanyCode"] == "" ? "ALL" : collection["CompanyCode"];
                ReportViewer reportViewer = new ReportViewer();
                reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Remote;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);

                reportViewer.ServerReport.ReportPath = "/EPRReport/MSRTracking_Report"; //EPRTesting For Staging or EPRReport For Production
                
                reportViewer.ServerReport.ReportServerUrl = new Uri(GetReportURL());


                List<ReportParameter> paramList = new List<ReportParameter>();

                paramList.Add(new ReportParameter("CompanyCode", xcompanycode));
                paramList.Add(new ReportParameter("CostCenter", Convert.ToString(xcostcentre)));
                paramList.Add(new ReportParameter("StartDate", StDate));
                paramList.Add(new ReportParameter("EndDate", EndDate));

                reportViewer.ServerReport.SetParameters(paramList);

                Warning[] warnings;
                string[] streamids;
                string mimeType, encoding, extension;


                if (collection["documentType"] == "PDF")
                {
                    byte[] bytes = reportViewer.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);

                    return File(bytes, mimeType, "MSRTracking_Report.pdf");

                }
                else
                {
                    byte[] bytes = reportViewer.ServerReport.Render("EXCEL", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                    return File(bytes, mimeType, "MSRTracking_Report.xls");
                   

                }

               
                
            }
               
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        #region DailyHazardList
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<ActionResult> DailyHazardList([Bind(Include = "LocCode")] FormCollection form)
        {
            string currentUserId = User.Identity.GetUserId();

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Remote;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);


            reportViewer.ServerReport.ReportPath = "/EPRReport/QHSEDailyList";
            reportViewer.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["ReportURL"]); //ConfigurationManager.AppSettings["ReportURL"];


            //  string xLocCode = Convert.ToString(Request["txtLocCode"].ToString());

            string xLocCode = form["LocationCode"];
            string xhazardDate = form["txtHzdDate"];

            //           string xhazardDate = System.Convert.ToString(form["txtHzdDate"]);

            if (xLocCode == "")
            {
                xLocCode = "All";
            }

            reportViewer.ServerReport.SetParameters(QHSEDailyParameter(xLocCode, xhazardDate, currentUserId));

            Warning[] warnings;
            string[] streamids;
            string mimeType, encoding, extension;

            byte[] bytes = reportViewer.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);

            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "inline; VendorReport=myfile." + extension);
            Response.BinaryWrite(bytes);

            Response.Flush();
            Response.End();

            ViewBag.ReportViewer = reportViewer;

            return ViewBag.ReportViewer;
        }

        private ReportParameter[] QHSEDailyParameter(string LocCode, string hzdDate, string UsrID)
        {
            ReportParameter p1 = new ReportParameter("LocationCode", LocCode);
            ReportParameter p2 = new ReportParameter("hazdDate", hzdDate);
            ReportParameter p3 = new ReportParameter("UserID", UsrID);

            return new ReportParameter[] { p1, p2, p3 };
        }
        #endregion

        #region DailyQHSE01
  

   

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HSEMonthly(FormCollection collection)
        {

            try
            {

                string xlocatoncode = collection["LocationCode"];
                string xmonth = collection["Month"];
                string xyear = collection["Year"];
                ReportViewer reportViewer = new ReportViewer();
                reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Remote;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);

                reportViewer.ServerReport.ReportPath = "/EPRReport/HSEPerformanceV1"; //EPRTesting For Staging or EPRReport For Production

                reportViewer.ServerReport.ReportServerUrl = new Uri(GetReportURL());


                List<ReportParameter> paramList = new List<ReportParameter>();

                paramList.Add(new ReportParameter("LocationCd", xlocatoncode));
                paramList.Add(new ReportParameter("POBMonth", xmonth));
                paramList.Add(new ReportParameter("POBYear", xyear));

                reportViewer.ServerReport.SetParameters(paramList);

                Warning[] warnings;
                string[] streamids;
                string mimeType, encoding, extension;


                if (collection["documentType"] == "PDF")
                {
                    byte[] bytes = reportViewer.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);

                    return File(bytes, mimeType, "HSEMonthlyReport.pdf");

                }
                else
                {
                    byte[] bytes = reportViewer.ServerReport.Render("EXCEL", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                    return File(bytes, mimeType, "HSEMonthlyReport.xls");


                }



            }

            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<ActionResult> DailyQHSE01([Bind(Include = "LocCode")] FormCollection form)
        {
            string currentUserId = User.Identity.GetUserId();

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Remote;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);

            reportViewer.ServerReport.ReportPath = "/EPRReport/DailyHOCList";
            reportViewer.ServerReport.ReportServerUrl = new Uri(GetReportURL());

            string xLocCode = form["LocationCode"];
            string xstartDate = form["txtstartDate"];
            string xendDate = form["txtendDate"];
            string condition = form["LocationCode"];
            string status = form["txtstartDate"];

            if (xLocCode == "")
            {
                xLocCode = "All";
            }

            reportViewer.ServerReport.SetParameters(QHSEDaily01Parameter(xLocCode, xstartDate, xendDate));

            Warning[] warnings;
            string[] streamids;
            string mimeType, encoding, extension;

            byte[] bytes = reportViewer.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);

            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "inline; VendorReport=myfile." + extension);
            Response.BinaryWrite(bytes);

            Response.Flush();
            Response.End();

            ViewBag.ReportViewer = reportViewer;

            return ViewBag.ReportViewer;
        }

        private ReportParameter[] QHSEDaily01Parameter(string LocCode, string stardDate, string endDate)
        {
            ReportParameter p1 = new ReportParameter("loccode", LocCode);
            ReportParameter p2 = new ReportParameter("startDate", stardDate);
            ReportParameter p3 = new ReportParameter("endDate", endDate);
            //ReportParameter p4 = new ReportParameter("userID", UsrID);

            return new ReportParameter[] { p1, p2, p3 };
        }
        #endregion

        #region DailyHOCList
   

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DailyHOCList(FormCollection form)
        {
            string currentUserId = User.Identity.GetUserId();

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Remote;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);

            reportViewer.ServerReport.ReportPath = "/EPRReport/DailyHOCList";
            reportViewer.ServerReport.ReportServerUrl = new Uri(GetReportURL());

            string xLocCode = form["LocationCode"];
            string condition = form["ConditionName"];
            string company = form["Company"];
            string reporter = form["Reporter"];
            string status = form["Status"];
            DateTime StDateStr = Convert.ToDateTime(form["txtstartDate"]);
            DateTime EndDateStr = Convert.ToDateTime(form["txtendDate"]);


            string xstartDate = StDateStr.ToString("yyyy-MM-dd");
            string xendDate = EndDateStr.ToString("yyyy-MM-dd"); 

            if (xLocCode == "")
            {
                xLocCode = "All";
            }

            if (status == "")
            {
                status = "All";
            }

            if (condition == "")
            {
                condition = "0";
            }
            if (reporter == "")
            {
                reporter = "All";
            }
            if (company == "")
            {
                company = "All";
            }

            List<ReportParameter> paramList = new List<ReportParameter>();

            paramList.Add(new ReportParameter("startDate",xstartDate));
            paramList.Add(new ReportParameter("endDate", xendDate));
            paramList.Add(new ReportParameter("status",status));
            paramList.Add(new ReportParameter("loccode",xLocCode));
            paramList.Add(new ReportParameter("condition", condition));
            paramList.Add(new ReportParameter("reporterName", reporter));
            paramList.Add(new ReportParameter("company", company));

            reportViewer.ServerReport.SetParameters(paramList);

            Warning[] warnings;
            string[] streamids;
            string mimeType, encoding, extension;


            if (form["documentType"] == "PDF")
            {
                byte[] bytes = reportViewer.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);

                return File(bytes, mimeType, "HOCReport_" + xLocCode + "_" + xstartDate + ".pdf");
               
            }
            else
            {
                byte[] bytes = reportViewer.ServerReport.Render("EXCEL", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                return File(bytes, mimeType, "HOCReport_" + xLocCode + "_" + xstartDate + ".xls");
            }
        }

        private ReportParameter[] DailyHOCListParameter(string LocCode, string stardDate, string endDate, string condition)
        {
            ReportParameter p1 = new ReportParameter("loccode", LocCode);
            ReportParameter p2 = new ReportParameter("startDate", stardDate);
            ReportParameter p3 = new ReportParameter("endDate", endDate);
            //ReportParameter p4 = new ReportParameter("userID", UsrID);
            ReportParameter p5 = new ReportParameter("condition", condition);
            //ReportParameter p6 = new ReportParameter("Location", string.Empty);
            //ReportParameter p7 = new ReportParameter("status", status);

            return new ReportParameter[] { p1, p2, p3, p5 };
        }

       
        #endregion

        #region HSEPerformance02
   

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<ActionResult> HSEPerformance02([Bind(Include = "LocCode")] FormCollection form)
        {
            string currentUserId = User.Identity.GetUserId();

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Remote;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);
            reportViewer.ShowParameterPrompts = true;

            reportViewer.ServerReport.ReportPath = "/EPRReport/HSEPerformance2";
            reportViewer.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["ReportURL"]);

            string xLocCode = form["LocationCode"];
            string xstartDate = form["txtstartDate"];
            string xendDate = form["txtendDate"];

            if (xLocCode == "")
            {
                xLocCode = "All";
            }

            reportViewer.ServerReport.SetParameters(HSEPerformance02Parameter(xLocCode, xstartDate, xendDate, currentUserId));

            Warning[] warnings;
            string[] streamids;
            string mimeType, encoding, extension;

            byte[] bytes = reportViewer.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);

            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "inline; VendorReport=myfile." + extension);
            Response.BinaryWrite(bytes);

            Response.Flush();
            Response.End();

            ViewBag.ReportViewer = reportViewer;

            return ViewBag.ReportViewer;
        }

        private ReportParameter[] HSEPerformance02Parameter(string LocCode, string stardDate, string endDate, string UsrID)
        {
            ReportParameter p1 = new ReportParameter("loccode", LocCode);
            ReportParameter p2 = new ReportParameter("startDate", stardDate);
            ReportParameter p3 = new ReportParameter("endDate", endDate);
            ReportParameter p4 = new ReportParameter("userID", UsrID);

            return new ReportParameter[] { p1, p2, p3, p4 };
        }
        #endregion

        #region HOCAnalysis03
  

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<ActionResult> HOCAnalysis03([Bind(Include = "LocCode")] FormCollection form)
        {
            string currentUserId = User.Identity.GetUserId();

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Remote;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);
            reportViewer.ShowParameterPrompts = true;

            reportViewer.ServerReport.ReportPath = "/EPRReport/HOCAnalysis";
            reportViewer.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["ReportURL"]);

            string xLocCode = form["LocationCode"];
            string xstartDate = form["txtstartDate"];
            string xendDate = form["txtendDate"];

            if (xLocCode == "")
            {
                xLocCode = "All";
            }

            reportViewer.ServerReport.SetParameters(HOCAnalysis03Parameter(xLocCode, xstartDate, xendDate, currentUserId));

            Warning[] warnings;
            string[] streamids;
            string mimeType, encoding, extension;

            byte[] bytes = reportViewer.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);

            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "inline; VendorReport=myfile." + extension);
            Response.BinaryWrite(bytes);

            Response.Flush();
            Response.End();

            ViewBag.ReportViewer = reportViewer;

            return ViewBag.ReportViewer;
        }

        private ReportParameter[] HOCAnalysis03Parameter(string LocCode, string stardDate, string endDate, string UsrID)
        {
            ReportParameter p1 = new ReportParameter("loccode", LocCode);
            ReportParameter p2 = new ReportParameter("startDate", stardDate);
            ReportParameter p3 = new ReportParameter("endDate", endDate);
            ReportParameter p4 = new ReportParameter("userID", UsrID);

            return new ReportParameter[] { p1, p2, p3, p4 };
        }
        #endregion

        #region HOCMonthlyDtl04
  

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<ActionResult> HOCMonthlyDtl04([Bind(Include = "LocCode")] FormCollection form)
        {
            string currentUserId = User.Identity.GetUserId();

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Remote;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);
            reportViewer.ShowParameterPrompts = true;

            reportViewer.ServerReport.ReportPath = "/EPRReport/HOCMonthlyDetail";
            reportViewer.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["ReportURL"]);

            string xLocCode = form["LocationCode"];
            string xstartDate = form["txtstartDate"];
            string xendDate = form["txtendDate"];

            if (xLocCode == "")
            {
                xLocCode = "All";
            }

            reportViewer.ServerReport.SetParameters(HOCMonthlyDtl04Parameter(xLocCode, xstartDate, xendDate, currentUserId));

            Warning[] warnings;
            string[] streamids;
            string mimeType, encoding, extension;

            byte[] bytes = reportViewer.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);

            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "inline; VendorReport=myfile." + extension);
            Response.BinaryWrite(bytes);

            Response.Flush();
            Response.End();

            ViewBag.ReportViewer = reportViewer;

            return ViewBag.ReportViewer;
        }

        private ReportParameter[] HOCMonthlyDtl04Parameter(string LocCode, string stardDate, string endDate, string UsrID)
        {
            ReportParameter p1 = new ReportParameter("loccode", LocCode);
            ReportParameter p2 = new ReportParameter("startDate", stardDate);
            ReportParameter p3 = new ReportParameter("endDate", endDate);
            ReportParameter p4 = new ReportParameter("userID", UsrID);

            return new ReportParameter[] { p1, p2, p3, p4 };
        }
        #endregion

        #region HOCMonthlyTotal05
   

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<ActionResult> HOCMonthlyTotal05([Bind(Include = "LocCode")] FormCollection form)
        {
            string currentUserId = User.Identity.GetUserId();

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Remote;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);
            reportViewer.ShowParameterPrompts = true;

            reportViewer.ServerReport.ReportPath = "/EPRReport/HOCMonthlyTotal";
            reportViewer.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["ReportURL"]);

            string xLocCode = form["LocationCode"];
            string xstartDate = form["txtstartDate"];
            string xendDate = form["txtendDate"];

            if (xLocCode == "")
            {
                xLocCode = "All";
            }

            reportViewer.ServerReport.SetParameters(HOCMonthlyTotal05Parameter(xLocCode, xstartDate, xendDate, currentUserId));

            Warning[] warnings;
            string[] streamids;
            string mimeType, encoding, extension;

            byte[] bytes = reportViewer.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);

            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "inline; VendorReport=myfile." + extension);
            Response.BinaryWrite(bytes);

            Response.Flush();
            Response.End();

            ViewBag.ReportViewer = reportViewer;

            return ViewBag.ReportViewer;
        }

        private ReportParameter[] HOCMonthlyTotal05Parameter(string LocCode, string stardDate, string endDate, string UsrID)
        {
            ReportParameter p1 = new ReportParameter("loccode", LocCode);
            ReportParameter p2 = new ReportParameter("startDate", stardDate);
            ReportParameter p3 = new ReportParameter("endDate", stardDate);
            ReportParameter p4 = new ReportParameter("userID", UsrID);

            return new ReportParameter[] { p1, p2, p3, p4 };
        }
        #endregion

        #region SAILListing06
 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SAILListing06(FormCollection form)
        {
            string currentUserId = User.Identity.GetUserId();

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Remote;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);
            reportViewer.ShowParameterPrompts = true;

            reportViewer.ServerReport.ReportPath = "/EPRReport/SAIL_Report";
            reportViewer.ServerReport.ReportServerUrl = new Uri(GetReportURL());

            string xLocCode = form["LocationCode"];
            string company = form["Company"];
            string reporter = form["Reporter"];
            DateTime StDateStr = Convert.ToDateTime(form["txtstartDate"]);
            DateTime EndDateStr = Convert.ToDateTime(form["txtendDate"]);


            string xstartDate = StDateStr.ToString("yyyy-MM-dd");
            string xendDate = EndDateStr.ToString("yyyy-MM-dd");

            if (xLocCode == "")
            {
                xLocCode = "All";
            }
            if (reporter == "")
            {
                reporter = "All";
            }
            if (company == "")
            {
                company = "All";
            }
           
            List<ReportParameter> paramList = new List<ReportParameter>();

            paramList.Add(new ReportParameter("loccode",xLocCode));
            paramList.Add(new ReportParameter("startdate", xstartDate));
            paramList.Add(new ReportParameter("enddate", xendDate));
            paramList.Add(new ReportParameter("userID", currentUserId));

            paramList.Add(new ReportParameter("reporterName", reporter));
            paramList.Add(new ReportParameter("company", company));

            reportViewer.ServerReport.SetParameters(paramList);

            Warning[] warnings;
            string[] streamids;
            string mimeType, encoding, extension;


            if (form["documentType"] == "PDF")
            {
                byte[] bytes = reportViewer.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);

                return File(bytes, mimeType, "SAILReport_" + xLocCode + "-" + xstartDate + ".pdf");

            }
            else
            {
                byte[] bytes = reportViewer.ServerReport.Render("EXCEL", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                return File(bytes, mimeType, "SAILReport_" + xLocCode + "-" + xstartDate + ".xls");


            }
        }

        private ReportParameter[] SAILListing06Parameter(string LocCode, string stardDate, string endDate, string UsrID)
        {
            ReportParameter p1 = new ReportParameter("loccode", LocCode);
            ReportParameter p2 = new ReportParameter("startDate", stardDate);
            ReportParameter p3 = new ReportParameter("endDate", endDate);
            ReportParameter p4 = new ReportParameter("userID", UsrID);

            return new ReportParameter[] { p1, p2, p3, p4 };
        }
        #endregion

        #region ListINR
  

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<ActionResult> ListINR([Bind(Include = "LocCode")] FormCollection form)
        {
            string currentUserId = User.Identity.GetUserId();

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Remote;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);
            reportViewer.ShowParameterPrompts = true;

            reportViewer.ServerReport.ReportPath = "/EPRReport/ListINR";
            reportViewer.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["ReportURL"]);

            string xLocCode = form["LocationCode"];
            string xstartDate = form["txtstartDate"];
            string xendDate = form["txtendDate"];

            if (xLocCode == "")
            {
                xLocCode = "All";
            }

            reportViewer.ServerReport.SetParameters(ListINRParameter(xLocCode, xstartDate, xendDate, currentUserId));

            Warning[] warnings;
            string[] streamids;
            string mimeType, encoding, extension;

            byte[] bytes = reportViewer.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);

            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "inline; VendorReport=myfile." + extension);
            Response.BinaryWrite(bytes);

            Response.Flush();
            Response.End();

            ViewBag.ReportViewer = reportViewer;

            return ViewBag.ReportViewer;
        }

        private ReportParameter[] ListINRParameter(string LocCode, string stardDate, string endDate, string UsrID)
        {
            ReportParameter p1 = new ReportParameter("loccode", LocCode);
            ReportParameter p2 = new ReportParameter("startDate", stardDate);
            ReportParameter p3 = new ReportParameter("endDate", endDate);
            ReportParameter p4 = new ReportParameter("userID", UsrID);

            return new ReportParameter[] { p1, p2, p3, p4 };
        }
        #endregion


        #region ListIIR
  

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<ActionResult> ListIIR([Bind(Include = "LocCode")] FormCollection form)
        {
            string currentUserId = User.Identity.GetUserId();

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Remote;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);
            reportViewer.ShowParameterPrompts = true;

            reportViewer.ServerReport.ReportPath = "/EPRReport/ListIIR";
            reportViewer.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["ReportURL"]);

            string xLocCode = form["LocationCode"];
            string xstartDate = form["txtstartDate"];
            string xendDate = form["txtendDate"];

            if (xLocCode == "")
            {
                xLocCode = "All";
            }

            reportViewer.ServerReport.SetParameters(ListIIRParameter(xLocCode, xstartDate, xendDate, currentUserId));

            Warning[] warnings;
            string[] streamids;
            string mimeType, encoding, extension;

            byte[] bytes = reportViewer.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);

            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "inline; VendorReport=myfile." + extension);
            Response.BinaryWrite(bytes);

            Response.Flush();
            Response.End();

            ViewBag.ReportViewer = reportViewer;

            return ViewBag.ReportViewer;
        }

        private ReportParameter[] ListIIRParameter(string LocCode, string stardDate, string endDate, string UsrID)
        {
            ReportParameter p1 = new ReportParameter("loccode", LocCode);
            ReportParameter p2 = new ReportParameter("startDate", stardDate);
            ReportParameter p3 = new ReportParameter("endDate", endDate);
            ReportParameter p4 = new ReportParameter("userID", UsrID);

            return new ReportParameter[] { p1, p2, p3, p4 };
        }
        #endregion

        #region INRReport11
   

  

        private ReportParameter[] INRReport11Parameter(string loccode, string INRReportNO, string IncidentDate, string userID)
        {
            ReportParameter p1 = new ReportParameter("loccode", loccode);
            ReportParameter p2 = new ReportParameter("INRReportNO", INRReportNO);
            ReportParameter p3 = new ReportParameter("IncidentDate", IncidentDate);
            ReportParameter p4 = new ReportParameter("userID", userID);

            return new ReportParameter[] { p1, p2, p3 };
        }
        #endregion


  

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DailyQHSEReport(FormCollection collection)
        {

            DateTime hazarddate = Convert.ToDateTime(collection["HazardDate"]);


            string HDate = hazarddate.ToString("yyyy-MM-dd");


            try
            {

                string xqhselocation =  collection["QHSELocation"];
                ReportViewer reportViewer = new ReportViewer();
                reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Remote;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);

                reportViewer.ServerReport.ReportPath = "/EPRReport/DailyQHSE1"; //EPRTesting For Staging or EPRReport For Production

                reportViewer.ServerReport.ReportServerUrl = new Uri(GetReportURL());


                List<ReportParameter> paramList = new List<ReportParameter>();

                paramList.Add(new ReportParameter("LocationCode", xqhselocation));
                paramList.Add(new ReportParameter("HazardDate", HDate));

                reportViewer.ServerReport.SetParameters(paramList);

                Warning[] warnings;
                string[] streamids;
                string mimeType, encoding, extension;


                if (collection["documentType"] == "PDF")
                {
                    byte[] bytes = reportViewer.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);

                    return File(bytes, mimeType, "QHSEReport_" + xqhselocation + "_" + HDate + ".pdf");

                }
                else
                {
                    byte[] bytes = reportViewer.ServerReport.Render("EXCEL", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                    return File(bytes, mimeType, "QHSEReport_" + xqhselocation + "_" + HDate + ".xls");


                }



            }

            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

   

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult POBListingReport(FormCollection form)
        {
           // string currentUserId = User.Identity.GetUserId();

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Remote;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);

            reportViewer.ServerReport.ReportPath = "/EPRReport/POBLISTING_Report";
            reportViewer.ServerReport.ReportServerUrl = new Uri(GetReportURL());

            string xLocCode = form["LocationCode"];
            DateTime pobDate = Convert.ToDateTime(form["txtstartDate"]);


            string xpobDate = pobDate.ToString("yyyy-MM-dd");

            if (xLocCode == "")
            {
                xLocCode = "All";
            }


            List<ReportParameter> paramList = new List<ReportParameter>();

            paramList.Add(new ReportParameter("POBDate", xpobDate));
            paramList.Add(new ReportParameter("LocationCode", xLocCode));

            reportViewer.ServerReport.SetParameters(paramList);

            Warning[] warnings;
            string[] streamids;
            string mimeType, encoding, extension;


            if (form["documentType"] == "PDF")
            {
                byte[] bytes = reportViewer.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);

                return File(bytes, mimeType, "POBReport_" + xLocCode + "_" + xpobDate + ".pdf");

            }
            else
            {
                byte[] bytes = reportViewer.ServerReport.Render("EXCEL", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                return File(bytes, mimeType, "POBReport_" + xLocCode + "_" + xpobDate + ".xls");


            }
        }

        
   

 
        public ActionResult IIRReport(FormCollection form, string Report,string StartDate)
        {
            // string currentUserId = User.Identity.GetUserId();

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Remote;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);

            reportViewer.ServerReport.ReportPath = "/EPRReport/IIR_Report";
            reportViewer.ServerReport.ReportServerUrl = new Uri(GetReportURL());
            string irno = form["Report"];
            DateTime IRDate = Convert.ToDateTime(form["StartDate"]);


            string xIRDate = IRDate.ToString("yyyy-MM-dd");
            List<ReportParameter> paramList = new List<ReportParameter>();

            paramList.Add(new ReportParameter("IIRReportNo", irno));

            reportViewer.ServerReport.SetParameters(paramList);

            Warning[] warnings;
            string[] streamids;
            string mimeType, encoding, extension;


            if (form["documentType"] == "PDF")
            {
                byte[] bytes = reportViewer.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);

                return File(bytes, mimeType, irno + "_" + IRDate + ".pdf");

            }
            else
            {
                byte[] bytes = reportViewer.ServerReport.Render("EXCEL", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                return File(bytes, mimeType, irno + "_" + IRDate + ".xls");


            }
        }

 
   

    
        [HttpGet]
        [EncryptedActionParameter]
        public ActionResult DownloadInvoice(int? id)
        {
            var value = id;
            
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Remote;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);

            reportViewer.ServerReport.ReportPath = "/EPRReport/InvoiceAcknowledgement";
            reportViewer.ServerReport.ReportServerUrl = new Uri(GetReportURL());
            List<ReportParameter> paramList = new List<ReportParameter>();

            paramList.Add(new ReportParameter("IHID", value.ToString()));

            reportViewer.ServerReport.SetParameters(paramList);

            Warning[] warnings;
            string[] streamids;
            string mimeType, encoding, extension;

            byte[] bytes = reportViewer.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);

            return File(bytes, mimeType, "InvoiceApprovalTracking.pdf");
        }

  

        [HttpPost]
        [EncryptedActionParameter]
        public ActionResult HazardsReport(FormCollection collection)
        {
            var location = collection["LocationCode"];
            DateTime FDate = Convert.ToDateTime(collection["txtHazardsReportFromDatePicker"]);
            DateTime TDate = Convert.ToDateTime(collection["txtHazardsReportToDatePicker"]);
            var FromDate = FDate.ToString("yyyy-MM-dd");
            var ToDate = TDate.ToString("yyyy-MM-dd");
            try
            {
                ReportViewer reportViewer = new ReportViewer();
                reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Remote;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);

                reportViewer.ServerReport.ReportPath = "/EPRReport/HazardReport"; //EPRTesting For Staging or EPRReport For Production

                reportViewer.ServerReport.ReportServerUrl = new Uri(GetReportURL());

                List<ReportParameter> paramList = new List<ReportParameter>();

                paramList.Add(new ReportParameter("LocationCode", location));
                paramList.Add(new ReportParameter("From", FromDate));
                paramList.Add(new ReportParameter("To", ToDate));

                reportViewer.ServerReport.SetParameters(paramList);

                Warning[] warnings;
                string[] streamids;
                string mimeType, encoding, extension;
                if (collection["documentType"] == "PDF")
                {
                    byte[] bytes = reportViewer.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);

                    return File(bytes, mimeType, "HazardReport.pdf");
                }
                else
                {
                    byte[] bytes = reportViewer.ServerReport.Render("EXCEL", null, out mimeType, out encoding, out extension, out streamids, out warnings);

                    return File(bytes, mimeType, "HazardReport.xls");
                }

            }

            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

  

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InvoiceTracking(FormCollection collection)
        {

            try
            {
                DateTime StDateStr = Convert.ToDateTime(collection["StartDateId"]);
                DateTime EndDateStr = Convert.ToDateTime(collection["EndDateId"]);


                string StDate = StDateStr.ToString("yyyy-MM-dd");
                string EndDate = EndDateStr.ToString("yyyy-MM-dd"); 
                string xcostcentre = collection["CostCentreCode"] == "" ? "0" : collection["CostCentreCode"];
                string xduration = collection["Duration"] == "" ? "0" : collection["Duration"];
                string xcompanycode = collection["CompanyCode"] == "" ? "ALL" : collection["CompanyCode"];
                string xDateOption = collection["DateOption"];
                ReportViewer reportViewer = new ReportViewer();
                reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Remote;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);

                reportViewer.ServerReport.ReportPath = "/EPRReport/InvoiceApprovalDuration"; //EPRTesting For Staging or EPRReport For Production

                reportViewer.ServerReport.ReportServerUrl = new Uri(GetReportURL());


                List<ReportParameter> paramList = new List<ReportParameter>();
                paramList.Add(new ReportParameter("CompanyCode", Convert.ToString(xcompanycode)));
                paramList.Add(new ReportParameter("CostCenter", Convert.ToString(xcostcentre)));
                paramList.Add(new ReportParameter("Duration", Convert.ToString(xduration)));
                paramList.Add(new ReportParameter("StartDate", StDate));
                paramList.Add(new ReportParameter("EndDate", EndDate));
                paramList.Add(new ReportParameter("DateOption", xDateOption));
                reportViewer.ServerReport.SetParameters(paramList);

                Warning[] warnings;
                string[] streamids;
                string mimeType, encoding, extension;


                if (collection["documentType"] == "PDF")
                {
                    byte[] bytes = reportViewer.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);

                    return File(bytes, mimeType, "InvoiceTracking_Action.pdf");

                }
                else
                {
                    byte[] bytes = reportViewer.ServerReport.Render("EXCEL", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                    return File(bytes, mimeType, "InvoiceTracking_Action.xls");


                }



            }

            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}