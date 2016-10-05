using GlobalPrint.ServerBusinessLogic.Models.Domain.Printers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using System.Collections.Generic;
using System.Diagnostics;

namespace GlobalPrint.ServerBusinessLogic.Models.Business.Printers
{
    /// <summary> Model for printer and it's operator.
    /// </summary>
    public class PrinterOperatorModel
    {
        [DebuggerStepThrough]
        public PrinterOperatorModel()
        {

        }
        public Printer Printer { get; set; }
        public User PrinterOperator { get; set; }
    }
}
