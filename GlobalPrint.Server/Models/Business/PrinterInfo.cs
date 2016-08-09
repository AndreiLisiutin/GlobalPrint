using GlobalPrint.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Server
{
    public class PrinterInfo
    {
        public Printer Printer { get; set; }
        public List<PrinterSchedule> Schedule { get; set; }
    }
}
