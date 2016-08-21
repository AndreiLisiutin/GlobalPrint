using AberrantSMPP;
using AberrantSMPP.Packet;
using AberrantSMPP.Packet.Request;
using AberrantSMPP.Packet.Response;
using GlobalPrint.ClientWeb.Models.PrinterController;
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
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb
{
    public class PrinterController : BaseController
    {
        [HttpGet]
        public ActionResult Print(int printerID)
        {
            var model = this._CreatePrintViewModel(printerID, null, null);
            return View(model);
        }

        private Printer_PrintViewModel _CreatePrintViewModel(int printerID, HttpPostedFileBase fileToPrint, PrintOrder order)
        {
            PrinterScheduled printer = new PrinterUnit().GetPrinterInfoByID(printerID);
            int userID = Request.RequestContext.HttpContext.User.Identity.GetUserId<int>();
            User user = new UserUnit().GetUserByID(userID);
            if (user.AmountOfMoney <= 0)
            {
                ModelState.AddModelError("", "Нет средств на счете");
            }
            if (order == null)
            {
                var rnd = new Random();
                order = new PrintOrder()
                {
                    PrinterID = printerID,
                    Format = "A4",
                    SecretCode = new string(rnd.Next(1, 9).ToString()[0], 2)
                        + new string(rnd.Next(1, 9).ToString()[0], 2)
                };
            }
            List<string> formats = new List<string>()
            {
                "A1",
                "A2",
                "A3",
                "A4",
                "A5"
            };
            List<SelectListItem> formatStore = formats.Select(e => new SelectListItem()
            {
                Text = e,
                Value = e,
                Selected = order.Format == e
            }).ToList();

            var model = new Printer_PrintViewModel()
            {
                printer = printer,
                order = order,
                fileToPrint = fileToPrint,
                formatStore = formatStore,
                user = user
            };
            return model;
        }

        [HttpPost]
        public ActionResult Print(Printer_PrintPostModel model)
        {
            if (!ModelState.IsValid)
            {
                var printerModel = this._CreatePrintViewModel(model.order.PrinterID, model.fileToPrint, model.order);
                return View("Print", printerModel);
            }
            if (model.fileToPrint == null || model.fileToPrint.ContentLength == 0)
            {
                ModelState.AddModelError("", "Не выбран файл для печати");
                var printerModel = this._CreatePrintViewModel(model.order.PrinterID, model.fileToPrint, model.order);
                return View("Print", printerModel);
            }
            if (!model.fileToPrint.FileName.EndsWith(".pdf"))
            {
                ModelState.AddModelError("", "Неизвестный формат файла. Поддерживаемые форматы: PDF.");
                var printerModel = this._CreatePrintViewModel(model.order.PrinterID, model.fileToPrint, model.order);
                return View("Print", printerModel);
            }
            if (string.IsNullOrEmpty(model.order.SecretCode))
            {
                ModelState.AddModelError("", "Введите секретный код.");
                var printerModel = this._CreatePrintViewModel(model.order.PrinterID, model.fileToPrint, model.order);
                return View("Print", printerModel);
            }
            int userID = Request.RequestContext.HttpContext.User.Identity.GetUserId<int>();
            User user = new UserUnit().GetUserByID(userID);
            if (user.AmountOfMoney <= 0)
            {
                var printerModel = this._CreatePrintViewModel(model.order.PrinterID, model.fileToPrint, model.order);
                return View("Print", printerModel);
            }

            string app_data = HttpContext.Server.MapPath("~/App_Data");
            string usersFolder = Path.Combine(app_data, userID.ToString());
            string pathFoFile = Path.Combine(usersFolder, model.fileToPrint.FileName);

            byte[] serializedFile = null;
            using (MemoryStream ms = new MemoryStream())
            {
                model.fileToPrint.InputStream.Seek(0, SeekOrigin.Begin);
                model.fileToPrint.InputStream.CopyTo(ms);
                serializedFile = ms.ToArray();
            }
            PdfReader pdfReader = new PdfReader(serializedFile);
            int numberOfPages = pdfReader.NumberOfPages;

            var printer = new PrinterUnit().GetPrinterByID(model.order.PrinterID);
            var order = model.order;
            order.Document = pathFoFile;
            order.OrderedOn = DateTime.Now;
            order.PagesCount = numberOfPages;
            order.PrintedOn = null;
            order.UserID = userID;
            order.PrintOrderStatusID = (int)PrintOrderStatusEnum.Waiting;

            Session["Printer_PreparedOrder"] = order;
            Session["Printer_PreparedOrderFile"] = serializedFile;

            return RedirectToAction("PrintConfirmation");
        }

        private Printer_PrintConfirmationViewModel _CreatePrintConfirmationViewModel(PrintOrder order)
        {
            int userID = Request.RequestContext.HttpContext.User.Identity.GetUserId<int>();
            User user = new UserUnit().GetUserByID(userID);
            var model = new Printer_PrintConfirmationViewModel()
            {
                order = order,
                user = user
            };
            return model;
        }

        [HttpGet]
        public ActionResult PrintConfirmation()
        {
            PrintOrder order = Session["Printer_PreparedOrder"] as PrintOrder;
            var model = this._CreatePrintConfirmationViewModel(order);
            return View(model);
        }

        [HttpPost]
        public ActionResult ExecuteOrder()
        {
            PrintOrder order = Session["Printer_PreparedOrder"] as PrintOrder;
            var file = Session["Printer_PreparedOrderFile"] as byte[];

            int userID = Request.RequestContext.HttpContext.User.Identity.GetUserId<int>();
            User user = new UserUnit().GetUserByID(userID);
            if (user.AmountOfMoney <= order.PricePerPage)
            {
                ModelState.AddModelError("", "Недостаточно средств на счете. Пополните баланс");
                var confirmmodel = this._CreatePrintConfirmationViewModel(order);
                return View("PrintConfirmation", confirmmodel);
            }

            order = new PrinterUnit().SavePrintOrder(file, order, this.GetSmsParams());
            return RedirectToAction("OrderCompleted", new { printOrderID = order.PrintOrderID });
        }

        [HttpGet]
        public ActionResult OrderCompleted(int printOrderID)
        {
            var order = new PrinterUnit().GetPrintOrderByID(printOrderID);
            return View(order);
        }

        //--------------------------------------------------------CRUD-----------------------------------------------------

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
                PrinterSchedule fromDB = model?.PrinterSchedule
                    ?.FirstOrDefault(e => e.DayOfWeek == (int)day);

                string dayName = weekUtility.DayName(day);
                bool isOpened = fromDB != null;
                schedule.Add(new Printer_EditViewMoel._Schedule((int)day, dayName, isOpened, fromDB?.OpenTime, fromDB?.CloseTime, fromDB?.ID ?? 0));
            }
            viewModel.Schedule = schedule;

            var services = new List<Printer_EditViewMoel._Service>();
            IEnumerable<PrintServiceExtended> allServices = new PrintServicesUnit().GetPrintServices();
            foreach (PrintServiceExtended service in allServices)
            {
                PrinterService fromDB = model?.PrinterServices
                    ?.FirstOrDefault(e => e.PrintServiceID == service.PrintService.PrintServiceID);

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
                    PrinterID = model.Printer.PrinterID
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

        [HttpGet]
        public ActionResult CreatePrinter()
        {
            try
            {
                Printer_EditViewMoel viewModel = this._Printer_EditViewMoel();
                return View("EditPrinter", viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("EditPrinter", null);
            }
        }

        [HttpGet]
        public ActionResult EditPrinter(int PrinterID)
        {
            try
            {
                PrinterEditionModel model = new PrinterUnit().GetPrinterEditionModel(PrinterID);
                Printer_EditViewMoel viewModel = this._Printer_EditViewMoel(model);
                return View("EditPrinter", viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("EditPrinter", null);
            }
        }

        [HttpGet]
        public ActionResult DeletePrinter(int PrinterID)
        {
            try
            {
                new PrinterUnit().DeletePrinter(PrinterID);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return RedirectToAction("UserAccountPrinterList", "UserAccountPrinterList");
        }

        [HttpPost]
        public ActionResult SavePrinter(Printer_EditViewMoel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                PrinterEditionModel editionnModel = this._PrinterEditionModel(model);
                new PrinterUnit().SavePrinter(editionnModel);
                return RedirectToAction("UserAccountPrinterList", "UserAccountPrinterList");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("EditPrinter", model);
            }
        }

    }
}