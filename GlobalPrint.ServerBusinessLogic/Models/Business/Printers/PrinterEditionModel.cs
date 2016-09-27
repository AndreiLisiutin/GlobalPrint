using GlobalPrint.ServerBusinessLogic.Models.Domain.Printers;
using System.Collections.Generic;
using System.Diagnostics;

namespace GlobalPrint.ServerBusinessLogic.Models.Business.Printers
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
