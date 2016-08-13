using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Printers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Orders
{
    public class PrinterInfo
    {
        public Printer Printer { get; set; }
        public List<PrinterSchedule> Schedule { get; set; }
    }
}
