﻿using GlobalPrint.ClientWeb.Models.PrinterController;
using GlobalPrint.ClientWeb.Models.PushNotifications;
using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Orders;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Printers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Orders;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Printers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Printers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Users;
using iTextSharp.text.pdf;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb
{
    public class PrinterController : BaseController
    {
        
        [HttpGet]
        public ActionResult GetPrinterServices(int printerID)
        {
            IEnumerable<PrinterServiceExtended> services = new PrintServicesUnit().GetPrinterServices(printerID);
            return Json(services);
        }

        /// <summary> Generate view model for printer edition action.
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

        /// <summary> Retreive printers which are owned orr operated by current user.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MyPrinters()
        {
            int UserID = this.GetCurrentUserID();
            var printerList = new PrinterUnit().GetUserPrinterList(UserID);
            return View("MyPrinters", printerList);
        }

        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                Printer_EditViewMoel viewModel = this._Printer_EditViewMoel();
                return View("Edit", viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Edit", null);
            }
        }

        [HttpGet]
        public ActionResult Edit(int PrinterID)
        {
            try
            {
                PrinterEditionModel model = new PrinterUnit().GetPrinterEditionModel(PrinterID);
                Printer_EditViewMoel viewModel = this._Printer_EditViewMoel(model);
                return View("Edit", viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Edit", null);
            }
        }

        [HttpPost]
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

            return RedirectToAction("MyPrinters", "Printer");
        }

        [HttpPost]
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

        /// <summary>
        /// Upload file into session
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public virtual ActionResult UploadFile()
        {
            HttpPostedFileBase file = Request.Files["gpUserFile"];
            bool isUploaded = false;
            string message = "Ошибка загрузки файла.";
            Guid fileId = new Guid();

            if (file != null && file.ContentLength != 0)
            {
                if (this.Session["UploadFiles"] == null)
                {
                    this.Session["UploadFiles"] = new Dictionary<Guid, HttpPostedFileBase>();
                }
                fileId = Guid.NewGuid();
                (this.Session["UploadFiles"] as Dictionary<Guid, HttpPostedFileBase>).Add(fileId, file);
                isUploaded = true;
                message = "Файл успешно загружен.";
            }

            return Json(new { isUploaded = isUploaded, message = message, fileId = fileId }, "text/html");
        }

    }
}