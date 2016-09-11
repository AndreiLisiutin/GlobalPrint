using GlobalPrint.ClientWeb.Models.PrinterController;
using GlobalPrint.ClientWeb.Models.PushNotifications;
using GlobalPrint.Configuration.DI;
using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Orders;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Printers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Orders;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Printers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Printers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.UnitsOfWork.Order;
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
    public class OrderController : BaseController
    {

        private Order_NewViewModel _CreatePrintViewModel(int printerID, NewOrder newOrder)
        {
            Argument.Require(printerID > 0, "printerID не может быть меньше 0.");
            Printer printer = new PrinterUnit().GetPrinterByID(printerID);
            if (newOrder == null)
            {
                //placing a new order
                var rnd = new Random();
                newOrder = new NewOrder()
                {
                    PrinterID = printerID,
                    SecretCode = new string(rnd.Next(1, 9).ToString()[0], 2)
                        + new string(rnd.Next(1, 9).ToString()[0], 2),
                    CopiesCount = 1,
                    UserID = this.GetCurrentUserID()
                };
            }

            var model = new Order_NewViewModel(newOrder, printer);
            return model;
        }

        private Order_ConfirmViewModel _CreatePrintConfirmationViewModel(NewOrder order, PrintOrder prepared = null)
        {
            if (prepared == null)
            {
                prepared = _OrderEditionModel(order).Item1;
            }

            var model = new Order_ConfirmViewModel()
            {
                NewOrder = order,
                PreparedOrder = prepared
            };
            return model;
        }

        private Tuple<PrintOrder, PrintFile> _OrderEditionModel(NewOrder newOrder)
        {
            Argument.NotNull(newOrder, "Не заполнены поля на форме нового заказа");
            Argument.Require(this._Uploaded.ContainsKey(newOrder.FileToPrint), "Файл заказа не найден.");
            PrintOrderUnit printOrderUnit = IoC.Instance.Resolve<PrintOrderUnit>();

            string app_data = HttpContext.Server.MapPath("~/App_Data");
            PrintFile file = this._Uploaded[newOrder.FileToPrint];
            PrintOrder order = printOrderUnit.New(newOrder, app_data, file);
            return new Tuple<PrintOrder, PrintFile>(order, file);
        }


        [HttpGet]
        public ActionResult MyOrders(string printOrderID)
        {
            PrintOrderUnit printOrderUnit = IoC.Instance.Resolve<PrintOrderUnit>();

            int userID = Request.RequestContext.HttpContext.User.Identity.GetUserId<int>();
            var printOrderList = printOrderUnit.GetUserPrintOrderList(userID, printOrderID);
            return View("MyOrders", printOrderList);
        }

        [HttpGet]
        public ActionResult New(int printerID)
        {
            Argument.Require(printerID > 0, "printerID не может быть меньше 0.");
            var model = this._CreatePrintViewModel(printerID, null);
            return View(model);
        }

        [HttpPost]
        public ActionResult New(Order_NewPostModel model)
        {
            Argument.NotNull(model, "Модель не может быть пустой.");
            Argument.NotNull(model.Order, "Заказ не может быть пустым.");

            try
            {
                if (!ModelState.IsValid)
                {
                    var printerModel = this._CreatePrintViewModel(model.Order.PrinterID, model.Order);
                    return View("New", printerModel);
                }
                PrintOrder order = this._OrderEditionModel(model.Order).Item1;
                Guid guid = Guid.NewGuid();
                this._PreparedOrders.Add(guid, new Tuple<NewOrder, PrintOrder>(model.Order, order));
                return RedirectToAction("Confirm", new { preparedOrderID = guid });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                var printerModel = this._CreatePrintViewModel(model.Order.PrinterID, model.Order);
                return View("New", printerModel);
            }
        }

        [HttpGet]
        public ActionResult Confirm(Guid preparedOrderID)
        {
            Argument.Require(preparedOrderID != Guid.Empty, "Ключ подготовленного для печати заказа не может быть пустым.");
            Argument.Require(this._PreparedOrders.ContainsKey(preparedOrderID), "Подготовленный для печати заказ не найден. Создайте его заново.");

            Tuple<NewOrder, PrintOrder> preparedOrder = this._PreparedOrders[preparedOrderID];
            var model = this._CreatePrintConfirmationViewModel(preparedOrder.Item1, preparedOrder.Item2);
            return View(model);
        }

        [HttpPost]
        public ActionResult Confirm(NewOrder NewOrder)
        {
            Argument.NotNull(NewOrder, "Заказ не может быть пустым.");
            PrintOrderUnit printOrderUnit = IoC.Instance.Resolve<PrintOrderUnit>();

            try
            {
                string app_data = HttpContext.Server.MapPath("~/App_Data");
                PrintFile file = this._Uploaded[NewOrder.FileToPrint];
                PrintOrder createdOrder = printOrderUnit.Create(NewOrder, app_data, file);

                // Push notification about new order
                User printerOperator = new PrinterUnit().GetPrinterOperator(createdOrder.PrinterID);
                string notificationMessage = string.Format(
                    "{0}: поступил новый заказ № {1}." + Environment.NewLine +
                    "Количество страниц: {2}, сумма заказа: {3}р.",
                    createdOrder.OrderedOn.ToString("HH:mm:ss"),
                    createdOrder.ID,
                    createdOrder.PagesCount,
                    createdOrder.FullPrice
                );
                new PushNotificationHub().NewIncomingOrder(notificationMessage, printerOperator.UserID);

                this._Uploaded.Remove(NewOrder.FileToPrint);
                return RedirectToAction("Complete", new { printOrderID = createdOrder.ID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                var model = this._CreatePrintConfirmationViewModel(NewOrder);
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult Complete(int printOrderID)
        {
            var order = new PrinterUnit().GetPrintOrderByID(printOrderID);
            return View(order);
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
                fileId = Guid.NewGuid();
                PrintFile printFile = PrintFile.FromHttpPostedFileBase(file);
                this._Uploaded.Add(fileId, printFile);
                isUploaded = true;
                message = "Файл успешно загружен.";
            }

            return Json(new { isUploaded = isUploaded, message = message, fileId = fileId }, "text/html");
        }

        /// <summary> Uplioaded files in memory. Will die if user will decide not to print them.
        /// </summary>
        private Dictionary<Guid, PrintFile> _Uploaded
        {
            get
            {
                Dictionary<Guid, PrintFile> _uploaded = this.Session["UploadFiles"]
                    as Dictionary<Guid, PrintFile>;
                if (_uploaded == null)
                {
                    this.Session["UploadFiles"] = _uploaded = new Dictionary<Guid, PrintFile>();
                }
                return _uploaded;
            }
        }

        /// <summary> Prepared orders.
        /// </summary>
        private Dictionary<Guid, Tuple<NewOrder, PrintOrder>> _PreparedOrders
        {
            get
            {
                Dictionary<Guid, Tuple<NewOrder, PrintOrder>> _prepared = this.Session["PreparedOrders"]
                    as Dictionary<Guid, Tuple<NewOrder, PrintOrder>>;
                if (_prepared == null)
                {
                    this.Session["PreparedOrders"] = _prepared = new Dictionary<Guid, Tuple<NewOrder, PrintOrder>>();
                }
                return _prepared;
            }
        }
    }
}