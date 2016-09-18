using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Offers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Printers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobalPrint.ClientWeb.Models.PrinterController
{
    public class Printer_MyPrinters
    {
        public List<Printer> PrinterList { get; set; }
        public UserOfferExtended LatestPrinterOwnerOffer { get; set; }
    }
}