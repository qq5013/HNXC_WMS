﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Interfaces;
using THOK.WebUtil;
using FastReport;
using FastReport.Utils;
using FastReport.Web;

namespace WMS.Controllers.Wms.WMS
{
    public class StockInWorkController : Controller
    {
        //
        // GET: /StockInWork/
        [Dependency]
        public IWMSProductStateService ProductStateService { get; set; }
        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasBarcode = true;
            ViewBag.hasTask = true;
            //ViewBag.hasExit = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }
        //获取某条单据下的作业任务.
        public ActionResult GetSubdetail(int page, int rows,string billno)
        {
            var productstate = ProductStateService.Details(page, rows, billno);
            return Json(productstate, "text/html", JsonRequestBehavior.AllowGet);
        }
        //作业 函数
        public ActionResult Task(string BillNo, string btypecode)
        {
            string userName = this.GetCookieValue("userid");
             string error="";
             bool bResult = ProductStateService.Task(BillNo, btypecode, userName,out error );
             string msg = bResult ? "作业成功" : "作业失败"+error;
             var just = new
             {
                 success = msg
             };
             return Json(just, "text/html", JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取PDF路径
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPdfName()
        {
            string FileName = "";
            string Path = Server.MapPath("/");
            try
            {
                 
                using (Report report = new Report())
                {
                    report.Load(Path + @"ContentReport\Report\test.frx");

                    report.Prepare();

                    FileName = Path + @"ContentReport\PDF\text.pdf";
                    FastReport.Export.Pdf.PDFExport pdfExport = new FastReport.Export.Pdf.PDFExport();

                    report.Export(pdfExport, FileName);
                    FileName =  "text.pdf";
                }
            }
            catch(Exception ex)
            {

            }


            return Json(JsonMessageHelper.getJsonMessage(true, FileName, Path), "text", JsonRequestBehavior.AllowGet);

        }
    }
}
