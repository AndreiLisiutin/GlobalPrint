﻿using GlobalPrint.ServerBusinessLogic.Models.Domain.Printers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.Models.Business.Orders
{
    public class PrinterScheduled
    {
        [DebuggerStepThrough]
        public PrinterScheduled()
        {
        }
        public Printer Printer { get; set; }
        public List<PrinterSchedule> Schedule { get; set; }
    }
}
