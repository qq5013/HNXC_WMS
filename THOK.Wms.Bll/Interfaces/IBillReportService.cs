﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
   public   interface IBillReportService : IService<BILLREPORT>
    {
        //入库单报表打印
       bool   StockinPrint(string Path, string username, string PrintCount, string BILLNO, string BILLDATEFROM, string BILLDATETO, string BTYPECODE, string BILLMETHOD, string STATE, string CIGARETTECODE, string FORMULACODE);
    }
}