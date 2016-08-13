
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Orders;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Printers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Orders
{
    public class PrintOrderInfo
    {
        public PrintOrderInfo() { }

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