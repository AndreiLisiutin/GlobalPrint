using GlobalPrint.ServerBusinessLogic.Models.Domain.Printers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.Models.Business.Printers
{

    /// <summary> Extended model for printer service.
    /// </summary>
    public class PrinterServiceExtended
    {
        [DebuggerStepThrough]
        public PrinterServiceExtended()
        {
        }
        public PrintServiceExtended PrintService { get; set; }
        public PrinterService PrinterService { get; set; }
    }
}
