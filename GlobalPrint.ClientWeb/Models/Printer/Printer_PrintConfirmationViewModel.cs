using GlobalPrint.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobalPrint.ClientWeb
{
    public class Printer_PrintConfirmationViewModel
    {
        public PrintOrder order { get; set; }
        public User user { get; set; }
    }
}