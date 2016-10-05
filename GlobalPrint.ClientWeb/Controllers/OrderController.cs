using GlobalPrint.ClientWeb.Models.OrderController;
using GlobalPrint.ClientWeb.Models.PushNotifications;
using GlobalPrint.Configuration.DI;
using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Printers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.UnitsOfWork.Order;
using GlobalPrint.ServerBusinessLogic.Models.Business;
using GlobalPrint.ServerBusinessLogic.Models.Business.Orders;
using GlobalPrint.ServerBusinessLogic.Models.Business.Printers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Orders;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Printers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb
{
    public class OrderController : BaseController
    {
        PrintOrderUnit _printOrderUnit;
        Random _random;
        public OrderController()
        {
            this._printOrderUnit = IoC.Instance.Resolve<PrintOrderUnit>();
            this._random = new Random();
        }

        /// <summary>
        /// Order details with opportunity of its rating.
        /// </summary>
        /// <param name="printOrderID">Identifier of the order.</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult Details(int printOrderID)
        {
            Argument.Positive(printOrderID, "Ключ заказа пустой.");

            PrintOrderInfo orderInfo = this._printOrderUnit.GetPrintOrderInfoByID(printOrderID);
            return View("Details", orderInfo);
        }

        /// <summary>
        /// Rate the order action. Associate star rating and comment with order.
        /// </summary>
        /// <param name="rateModel">Info about order's rating.</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult Rate(Order_RateViewModel rateModel)
        {
            Argument.NotNull(rateModel, "Модель оценки заказа пустая.");
            Argument.Positive(rateModel.PrintOrderID, "Ключ заказа в модели оценки заказа пустой.");

            int userID = this.GetCurrentUserID();
            this._printOrderUnit.Rate(rateModel.PrintOrderID, rateModel.Rating, rateModel.Comment, userID);
            return RedirectToAction("MyOrders", "Order");
        }

        /// <summary>
        /// Returns a view with list of current user's orders. 
        /// </summary>
        /// <param name="printOrderID">Mask for order filtering. Optional.</param>
        /// <returns>View with list of current user's orders.</returns>
        [HttpGet]
        [Authorize]
        public ActionResult MyOrders(string printOrderID)
        {
            int userID = this.GetCurrentUserID();
            var printOrderList = this._printOrderUnit.GetUserPrintOrderList(userID, printOrderID);
            return View("MyOrders", printOrderList);
        }

        /// <summary>
        /// Create a new order using old document and printer where such an order was performed.
        /// </summary>
        /// <param name="printOrderID">Identifier of an order.</param>
        /// <returns>New order creation view.</returns>
        [HttpGet]
        [Authorize]
        public ActionResult FromExisting(int printOrderID)
        {
            Argument.Positive(printOrderID, "printOrderID не может быть меньше 0.");
            int userID = this.GetCurrentUserID();
            string app_data = HttpContext.Server.MapPath("~/App_Data");
            NewOrder newOrder = this._printOrderUnit.FromExisting(printOrderID, userID);
            DocumentBusinessInfo document = this._printOrderUnit.GetPrintOrderDocument(printOrderID, userID, app_data);
            this._Uploaded[newOrder.FileToPrint] = document;
            return this._ORDER_NEW(newOrder);
        }

        /// <summary>
        /// Place a new order for a certain printer.
        /// </summary>
        /// <param name="printerID">Identifier of the printer.</param>
        /// <returns>View with printer edition possibility.</returns>
        [HttpGet]
        [Authorize]
        public ActionResult New(int printerID)
        {
            Argument.Require(printerID > 0, "printerID не может быть меньше 0.");

            int userId = this.GetCurrentUserID();
            //creating an empty order
            NewOrder newOrder = new NewOrder()
            {
                PrinterID = printerID,
                SecretCode = new string(this._random.Next(1, 9).ToString()[0], 2)
                    + new string(this._random.Next(1, 9).ToString()[0], 2),
                CopiesCount = 1,
                UserID = userId
            };

            return this._ORDER_NEW(newOrder);
        }

        /// <summary>
        /// Accept new order and check it for validity.
        /// </summary>
        /// <param name="newOrder">New order model.</param>
        /// <returns>Redirect to order confirmation page.</returns>
        [HttpPost]
        public ActionResult New(NewOrder newOrder)
        {
            Argument.NotNull(newOrder, "Модель нового заказа не может быть пустой.");
            if (!ModelState.IsValid)
            {
                return this._ORDER_NEW(newOrder);
            }
            if (!this._Uploaded.ContainsKey(newOrder.FileToPrint))
            {
                //файл не найден
                ModelState.AddModelError("", "Файл для печати не найден.");
                return this._ORDER_NEW(newOrder);
            }
            DocumentBusinessInfo document = null;
            this._Uploaded.TryGetValue(newOrder.FileToPrint, out document);
            Validation validation = this._printOrderUnit.Validate(newOrder, document);
            if (!validation.IsValid)
            {
                validation.Errors.ForEach(e => ModelState.AddModelError("", e));
                return this._ORDER_NEW(newOrder);
            }

            return RedirectToAction("Confirm", newOrder);
        }

        /// <summary>
        /// Show confirmation page with new order details.
        /// </summary>
        /// <param name="newOrder">New order model.</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult Confirm(NewOrder newOrder)
        {
            Argument.NotNull(newOrder, "Подготовленный для печати заказ не может быть пустым.");
            if (!this._Uploaded.ContainsKey(newOrder.FileToPrint))
            {
                //файл не найден
                ModelState.AddModelError("", "Файл для печати не найден.");
                return RedirectToAction("New", newOrder);
            }
            return this._ORDER_CONFIRM(newOrder);
        }

        /// <summary>
        /// Creates exactly a new order. After successful creation redirects to Order.Complete page.
        /// </summary>
        /// <param name="newOrder"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult Create(NewOrder newOrder)
        {
            Argument.NotNull(newOrder, "Подготовленный для печати заказ не может быть пустым.");
            if (!this._Uploaded.ContainsKey(newOrder.FileToPrint))
            {
                //файл не найден
                ModelState.AddModelError("", "Файл для печати не найден.");
                return RedirectToAction("New", newOrder);
            }
            if (!ModelState.IsValid)
            {
                return this._ORDER_CONFIRM(newOrder);
            }

            Validation validation = this._printOrderUnit.Validate(newOrder, this._Uploaded[newOrder.FileToPrint]);
            if (!validation.IsValid)
            {
                validation.Errors.ForEach(e => ModelState.AddModelError("", e));
                return this._ORDER_CONFIRM(newOrder);
            }

            string app_data = HttpContext.Server.MapPath("~/App_Data");
            int userID = this.GetCurrentUserID();
            PrintOrder createdOrder = this._printOrderUnit.Create(newOrder, userID, app_data, this._Uploaded[newOrder.FileToPrint]);

            #region Notifications

            #warning remove it from here
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
            IoC.Instance.Resolve<PushNotificationHub>().NewIncomingOrder(notificationMessage, printerOperator.ID);

            #endregion Notifications

            this._Uploaded.Remove(newOrder.FileToPrint);
            return RedirectToAction("Complete", new { printOrderID = createdOrder.ID });
        }

        /// <summary>
        /// Shows page with congratulations to user for completeness of his new order.
        /// </summary>
        /// <param name="printOrderID">New order identifier.</param>
        /// <returns>Page with congratulations.</returns>
        [HttpGet]
        [Authorize]
        public ActionResult Complete(int printOrderID)
        {
            Argument.Positive(printOrderID, "Ключ заказа пустой.");

            PrintOrder order = this._printOrderUnit.GetByID(printOrderID);
            return View(order);
        }

        /// <summary>
        /// Download order from the App_data folder of the application.
        /// </summary>
        /// <param name="printOrderID">Identifier of the order.</param>
        /// <returns>File stram with order file.</returns>
        [HttpGet]
        [Authorize]
        public ActionResult DownloadOrder(int printOrderID)
        {
            string app_data = HttpContext.Server.MapPath("~/App_Data");
            int userID = this.GetCurrentUserID();
            DocumentBusinessInfo fileInfo = this._printOrderUnit.GetPrintOrderDocument(printOrderID, userID, app_data);
            return File(fileInfo.SerializedFile, System.Net.Mime.MediaTypeNames.Application.Octet, fileInfo.Name);
        }

        private ViewResult _ORDER_NEW(NewOrder newOrder)
        {
            Argument.NotNull(newOrder, "Модель для нового заказа не может быть пустой.");
            Argument.Positive(newOrder.PrinterID, "Ключ принтера в модели для нового заказа не может быть пустым.");

            Printer printer = new PrinterUnit().GetPrinterByID(newOrder.PrinterID);
            ViewBag.Printer = printer;
            return View("New", newOrder);
        }

        private ViewResult _ORDER_CONFIRM(NewOrder newOrder)
        {
            Argument.NotNull(newOrder, "Модель для нового заказа не может быть пустой.");
            Argument.Positive(newOrder.PrinterID, "Ключ принтера в модели для нового заказа не может быть пустым.");
            Argument.Require(this._Uploaded.ContainsKey(newOrder.FileToPrint), "Не найден файл для печати.");

            int pagesCount = this._printOrderUnit.CalculatePagesCount(this._Uploaded[newOrder.FileToPrint]);
            PrinterServiceExtended printService = this._printOrderUnit.GetPrintService(newOrder);
            decimal fullPrice = PrintOrderUnit.CALCULATE_FULL_PRICE(printService.PrinterService.PricePerPage, pagesCount, newOrder.CopiesCount);

            ViewBag.PagesCount = pagesCount;
            ViewBag.PrinterService = printService;
            ViewBag.FullPrice = fullPrice;
            return View("Confirm", newOrder);
        }
    }
}