using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Printers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Printers
{
    /// <summary> Model for create and Edit the Printer entity.
    /// </summary>
    public class PrinterEditionModel
    {
        [DebuggerStepThrough]
        public PrinterEditionModel()
        {

        }
        public Printer Printer { get; set; }
        public IEnumerable<PrinterSchedule> PrinterSchedule { get; set; }
        public IEnumerable<PrinterService> PrinterServices { get; set; }
    }
}
