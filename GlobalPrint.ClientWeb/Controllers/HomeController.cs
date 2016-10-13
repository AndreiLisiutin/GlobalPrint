﻿using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Printers;
using GlobalPrint.ServerBusinessLogic.Models.Business.Printers;
using System.Collections.Generic;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb
{
    public class HomeController : BaseController
    {
        PrinterUnit _printerUnit;
        public HomeController()
            :this(new PrinterUnit())
        {
        }
        public HomeController(PrinterUnit printerUnit)
        {
            this._printerUnit = printerUnit;
        }

        // GET: Home
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetPrinters(PrinterSearchFilter filter)
        {
            filter = filter ?? new PrinterSearchFilter();
            IEnumerable<PrinterFullInfoModel> printers = this._printerUnit.GetPrinters(filter);
            return Json(printers);
        }

        [HttpGet]
        public ActionResult GetClosestPrinter(float latitude, float longtitude)
        {
            PrinterFullInfoModel printer = this._printerUnit.GetClosestPrinter(latitude, longtitude);
            return Json(printer);
        }
    }
}
