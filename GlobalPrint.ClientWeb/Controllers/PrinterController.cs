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
        [Authorize, HttpGet]
        public ActionResult GetPrinterServices(int printerID)
        {
            IEnumerable<PrinterServiceExtended> services = new PrintServicesUnit().GetPrinterServices(printerID);
            return Json(services);
        }

        /// <summary> 
        /// Generate view model for printer edition action.
        /// </summary>
        /// <param name="model">Business model for printer edition.</param>
        /// <returns></returns>
        private Printer_EditViewMoel _Printer_EditViewMoel(PrinterEditionModel model = null)
        {
            int userID = this.GetCurrentUserID();
            var weekUtility = new WeekUtility();

            Printer_EditViewMoel viewModel = new Printer_EditViewMoel();
            viewModel.Printer = model?.Printer ?? new Printer()
            {
                OwnerUserID = userID,
                OperatorUserID = userID,
            };

            var schedule = new List<Printer_EditViewMoel._Schedule>();
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                PrinterSchedule dayFromDB = model?.PrinterSchedule
                    ?.FirstOrDefault(e => e.DayOfWeek == (int)day);

                string dayName = weekUtility.DayName(day);
                bool isOpened = model?.Printer != null ? dayFromDB != null : true;
                //new printer is opened every day from 9:00 to 18:00 by default
                TimeSpan? openTime = model?.Printer != null ? dayFromDB?.OpenTime : TimeSpan.FromHours(9);
                TimeSpan? closeTime = model?.Printer != null ? dayFromDB?.CloseTime : TimeSpan.FromHours(18);
                schedule.Add(new Printer_EditViewMoel._Schedule((int)day, dayName, isOpened, openTime, closeTime, dayFromDB?.ID ?? 0));
            }
            viewModel.Schedule = schedule;

            var services = new List<Printer_EditViewMoel._Service>();
            IEnumerable<PrintServiceExtended> allServices = new PrintServicesUnit().GetPrintServices()
                .OrderBy(e => e.PrintType.Name)
                .ThenBy(e => e.PrintSize.Name)
                .ThenBy(e => e.PrintService.IsColored)
                .ThenBy(e => e.PrintService.IsTwoSided);
            foreach (PrintServiceExtended service in allServices)
            {
                PrinterService fromDB = model?.PrinterServices
                    ?.FirstOrDefault(e => e.PrintServiceID == service.PrintService.ID);

                bool isSupported = fromDB != null;
                services.Add(new Printer_EditViewMoel._Service(service, isSupported, fromDB?.PricePerPage, fromDB?.ID ?? 0));
            }

            viewModel.Services = services;

            return viewModel;
        }

        private PrinterEditionModel _PrinterEditionModel(Printer_EditViewMoel viewModel)
        {
            int userID = this.GetCurrentUserID();

            PrinterEditionModel model = new PrinterEditionModel();
            model.Printer = viewModel.Printer;
            model.Printer.OperatorUserID = model.Printer.OperatorUserID > 0 ? model.Printer.OperatorUserID : userID;
            model.Printer.OwnerUserID = model.Printer.OwnerUserID > 0 ? model.Printer.OwnerUserID : userID;

            model.PrinterSchedule = viewModel.Schedule
                .Where(e => e.isOpened)
                .Select(e => new PrinterSchedule()
                {
                    ID = e.PrinterScheduleID,
                    DayOfWeek = e.DayOfWeek,
                    CloseTime = e.CloseTime ?? TimeSpan.FromHours(-1),
                    OpenTime = e.OpenTime ?? TimeSpan.FromHours(-1),
                    PrinterID = model.Printer.ID
                })
                .ToList();

            model.PrinterServices = viewModel.Services
                .Where(e => e.IsSupported)
                .Select(e => new PrinterService()
                {
                    ID = e.PrinterServiceID,
                    PrintServiceID = e.PrintServiceID,
                    PricePerPage = e.Price ?? 0
                })
                .ToList();

            return model;
        }

        /// <summary> 
        /// Retrieves printers which are owned orr operated by current user.
        /// </summary>
        /// <returns></returns>
        [Authorize, HttpGet, ImportModelState]
        public ActionResult MyPrinters()
        {
            int userID = this.GetCurrentUserID();

            var printerList = new PrinterUnit().GetUserPrinterList(userID);
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
            Printer_EditViewMoel viewModel = this._Printer_EditViewMoel();

                //var latestPrinterOwnerOffer = new UserOfferUnit().GetLatestUserOfferByUserID(this.GetCurrentUserID(), OfferTypeEnum.PrinterOwnerOffer);
                //viewModel.NeedPrinterOwnerOffer = !latestPrinterOwnerOffer.HasUserOffer;

            return View("Edit", viewModel);
        }

        [Authorize, HttpGet]
        public ActionResult Edit(int PrinterID)
        {
            Argument.Positive(PrinterID, "Ключ принтера пустой.");

            int userID = this.GetCurrentUserID();
            PrinterEditionModel model = new PrinterUnit().GetPrinterEditionModel(PrinterID, userID);
            Printer_EditViewMoel viewModel = this._Printer_EditViewMoel(model);
            return View("Edit", viewModel);
        }

        [Authorize, HttpPost ExportModelState]
        public ActionResult Delete(int PrinterID)
        {
            try
            {
                new PrinterUnit().DeletePrinter(PrinterID);
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
        public ActionResult Save(Printer_EditViewMoel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                PrinterEditionModel editionModel = this._PrinterEditionModel(model);
                new PrinterUnit().SavePrinter(editionModel);
                return RedirectToAction("MyPrinters", "Printer");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Edit", model);
            }
        }
    }
}