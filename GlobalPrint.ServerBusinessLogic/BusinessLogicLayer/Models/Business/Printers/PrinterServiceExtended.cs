using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Printers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Printers
{

    /// <summary> Extended model for printer service.
    /// </summary>
    public class PrinterServiceExtended
    {
        public PrintServiceExtended PrintService { get; set; }
        public PrinterService PrinterService { get; set; }
    }
}
