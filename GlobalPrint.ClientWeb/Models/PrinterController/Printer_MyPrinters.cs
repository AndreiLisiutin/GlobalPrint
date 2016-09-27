using GlobalPrint.ServerBusinessLogic.Models.Business.Offers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Printers;
using System.Collections.Generic;

namespace GlobalPrint.ClientWeb.Models.PrinterController
{
    public class Printer_MyPrinters
    {
        public List<Printer> PrinterList { get; set; }
        public UserOfferExtended LatestPrinterOwnerOffer { get; set; }
    }
}