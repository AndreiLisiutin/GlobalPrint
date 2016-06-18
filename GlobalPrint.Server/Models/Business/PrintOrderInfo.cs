using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Server
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
    }
}