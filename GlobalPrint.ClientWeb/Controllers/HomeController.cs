using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Printers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Printers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Printers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb
{
    public class HomeController : BaseController
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetPrinters(PrinterSearchFilter filter)
        {
            filter = filter ?? new PrinterSearchFilter();
            IEnumerable<PrinterFullInfoModel> printers = new PrinterUnit().GetPrinters(filter);
            return Json(printers);
        }

        [HttpGet]
        public ActionResult GetClosestPrinter(float latitude, float longtitude)
        {
            PrinterFullInfoModel printer = new PrinterUnit().GetClosestPrinter(latitude, longtitude);
            return Json(printer);
        }
    }
}
