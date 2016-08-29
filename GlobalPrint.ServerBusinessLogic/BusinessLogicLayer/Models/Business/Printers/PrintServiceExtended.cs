using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Printers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Printers
{
    /// <summary> Extended model for print service.
    /// </summary>
    public class PrintServiceExtended
    {
        [DebuggerStepThrough]
        public PrintServiceExtended()
        {
        }
        [DebuggerStepThrough]
        public PrintServiceExtended(PrintService service, PrintSizePrintType sizeType, PrintSize size, PrintType type)
        {
            this.PrintService = service;
            this.PrintSizePrintType = sizeType;
            this.PrintSize = size;
            this.PrintType = type;
        }

        public PrintService PrintService { get; set; }
        public PrintSizePrintType PrintSizePrintType { get; set; }
        public PrintSize PrintSize { get; set; }
        public PrintType PrintType { get; set; }
    }


}
