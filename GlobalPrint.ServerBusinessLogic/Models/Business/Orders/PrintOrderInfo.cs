using GlobalPrint.ServerBusinessLogic.Models.Domain.Orders;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Printers;
using System.Diagnostics;

namespace GlobalPrint.ServerBusinessLogic.Models.Business.Orders
{
    public class PrintOrderInfo
    {
        [DebuggerStepThrough]
        public PrintOrderInfo() { }

        [DebuggerStepThrough]
        public PrintOrderInfo(PrintOrder printOrder, Printer printer)
        {
            this.PrintOrder = printOrder;
            this.Printer = printer;
        }

        public PrintOrder PrintOrder { get; set; }
        public Printer Printer { get; set; }
        public PrintOrderStatus Status { get; set; }
    }
}