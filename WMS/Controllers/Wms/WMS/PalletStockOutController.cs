﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Interfaces;
using THOK.WebUtil;
using THOK.Wms.DbModel;

namespace WMS.Controllers.Wms.WMS
{
    public class PalletStockOutController : Controller
    {
        //
        // GET: /PalletStockOut/

        [Dependency]
        public IWMSPalletMasterService PalletmasterService { get; set; }

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasAdd = true;
            ViewBag.hasEdit = true;
            ViewBag.hasDelete = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.hasTask = true;
            //ViewBag.hasAudit = true;
            //ViewBag.hasAntiTrial = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }

        public ActionResult GetDetail(int page, int rows, FormCollection collection)
        {
            string BILL_NO = collection["BILL_NO"] ?? "";
            string BILL_DATE = collection["BILL_DATE"] ?? "";
            string BTYPE_CODE = collection["BTYPE_CODE"] ?? "";
            string WAREHOUSE_CODE = collection["WAREHOUSE_CODE"] ?? "";
            string TARGET = collection["TARGET"] ?? "";
            string STATE = collection["STATE"] ?? "";
            string OPERATER = collection["OPERATER"] ?? "";
            string OPERATE_DATE = collection["OPERATE_DATE"] ?? "";
            string TASKER = collection["TASKER"] ?? "";
            string TASK_DATE = collection["TASK_DATE"] ?? "";
            var master = PalletmasterService.Details(page, rows, "0",BILL_NO ,BILL_DATE ,BTYPE_CODE ,WAREHOUSE_CODE ,TARGET ,STATE ,OPERATER ,OPERATE_DATE ,TASKER ,TASK_DATE );// 0表示出库类型
            return Json(master, "text", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSubDetail(int page, int rows, string BillNo)
        {
            var Billdetail = PalletmasterService.GetSubDetails(page, rows, BillNo);
            return Json(Billdetail, "text", JsonRequestBehavior.AllowGet);
        }
        //单据编号
        public ActionResult GetBillNo(System.DateTime dtime, string BILL_NO, string prefix)
        {
            string userName = this.GetCookieValue("username");
            var BillnoInfo = PalletmasterService.GetBillNo(userName, dtime, BILL_NO, prefix);

            return Json(BillnoInfo, "text", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add(WMS_PALLET_MASTER mast, object detail, string prefix)
        {
            bool bResult = PalletmasterService.Add(mast, detail, prefix);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(WMS_PALLET_MASTER mast, object detail)
        {
            bool bResult = PalletmasterService.Edit(mast, detail);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }

    }
}
