using GlobalPrint.ClientWeb.Models.PrinterController;
using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Printers;
using GlobalPrint.ServerBusinessLogic.Models.Business.Printers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Printers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GlobalPrint.ClientWeb.Filters;

namespace GlobalPrint.ClientWeb
{
    public class PrinterController : BaseController
    {
        PrinterUnit _printerUnit = null;
        public PrinterController(PrinterUnit printerUnit)
        {
            this._printerUnit = printerUnit;
        }
        public PrinterController()
            : this(new PrinterUnit())
        {
        }

        [Authorize, HttpGet]
        public ActionResult GetPrinterServices(int printerID)
        {
            IEnumerable<PrinterServiceExtended> services = new PrintServicesUnit().GetPrinterServices(printerID);
            return Json(services);
        }

        /// <summary> 
        /// Retrieves printers which are owned orr operated by current user.
        /// </summary>
        /// <returns></returns>
        [Authorize, HttpGet, ImportModelState]
        public ActionResult MyPrinters()
        {
            int userID = this.GetCurrentUserID();

            var printerList = this._printerUnit.GetUserPrinterList(userID);
            //var latestPrinterOwnerOffer = new UserOfferUnit().GetLatestUserOfferByUserID(userID, OfferTypeEnum.PrinterOwnerOffer);

            Printer_MyPrinters myPrinters = new Printer_MyPrinters()
            {
                PrinterList = printerList
                //LatestPrinterOwnerOffer = latestPrinterOwnerOffer
            };
            return View("MyPrinters", myPrinters);
        }

        [Authorize, HttpGet]
        public ActionResult Create()
        {
            PrinterEditionModel model = new PrinterEditionModel()
            {
                Printer = new Printer()
                {
                    OwnerUserID = this.GetCurrentUserID(),
                    OperatorUserID = this.GetCurrentUserID()
                },
                PrinterServices = new List<PrinterService>()
            };

            var schedule = new List<PrinterSchedule>();
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                schedule.Add(new PrinterSchedule()
                {
                    OpenTime = TimeSpan.FromHours(9),
                    CloseTime = TimeSpan.FromHours(18),
                    DayOfWeek = (int)day
                });
            }
            model.PrinterSchedule = schedule;

            return this._PRINTER_EDIT(model);
        }

        [Authorize, HttpGet]
        public ActionResult Clone(int PrinterID)
        {
            Argument.Positive(PrinterID, "Ключ принтера пустой.");

            int userID = this.GetCurrentUserID();
            PrinterEditionModel model = this._printerUnit.GetPrinterEditionModel(PrinterID, userID);
            model.Printer.ID = 0;
            model.Printer.IsDisabled = false;

            foreach (var schedule in model.PrinterSchedule)
            {
                schedule.ID = 0;
                schedule.PrinterID = 0;
            }
            foreach (var service in model.PrinterServices)
            {
                service.ID = 0;
                service.PrinterID = 0;
            }
            return this._PRINTER_EDIT(model);
        }

        [Authorize, HttpGet]
        public ActionResult Edit(int PrinterID)
        {
            Argument.Positive(PrinterID, "Ключ принтера пустой.");

            int userID = this.GetCurrentUserID();
            PrinterEditionModel model = this._printerUnit.GetPrinterEditionModel(PrinterID, userID);
            return this._PRINTER_EDIT(model);
        }

        [Authorize, HttpPost ExportModelState]
        public ActionResult Delete(int PrinterID)
        {
            try
            {
                this._printerUnit.DeletePrinter(PrinterID);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            if (Request.IsAjaxRequest())
            {
                return JavaScript("document.location.replace('" + Url.Action("MyPrinters", "Printer") + "');");
            }
            else
            {
                return RedirectToAction("MyPrinters", "Printer");
            }
        }

        [Authorize, HttpPost]
        public ActionResult Save(PrinterEditionModel model)
        {
            Argument.NotNull(model, "Модель редактирования принтера пустая.");
            model.PrinterServices = model.PrinterServices ?? new List<PrinterService>();
            model.PrinterServices = model.PrinterServices.Where(e => e.PricePerPage > 0);
            model.PrinterSchedule = model.PrinterSchedule ?? new List<PrinterSchedule>();
            model.PrinterSchedule = model.PrinterSchedule.Where(e => e.OpenTime != default(TimeSpan) || e.CloseTime != default(TimeSpan));

            this._printerUnit.SavePrinter(model);
            return RedirectToAction("MyPrinters", "Printer");
        }

        private ViewResult _PRINTER_EDIT(PrinterEditionModel model)
        {
            List<PrintServiceExtended> allServices = new PrintServicesUnit().GetPrintServices()
                .OrderBy(e => e.PrintType.Name)
                .ThenBy(e => e.PrintSize.Name)
                .ThenBy(e => e.PrintService.IsColored)
                .ThenBy(e => e.PrintService.IsTwoSided)
                .ToList();
            ViewBag.AllServices = allServices;
            ViewBag.WeekUtility = new WeekUtility();
            return this.View("Edit", model);
        }
    }
}