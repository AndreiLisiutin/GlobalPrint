using GlobalPrint.ClientWeb.Models.PrinterController;
using GlobalPrint.ClientWeb.Models.PushNotifications;
using GlobalPrint.Infrastructure.CommonUtils;
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
        [HttpGet]
        public ActionResult New(int printerID)
        {
            Argument.Require(printerID > 0, "printerID не может быть меньше 0.");
            var model = this._CreatePrintViewModel(printerID, null);
            return View(model);
        }

        private Order_NewViewModel _CreatePrintViewModel(int printerID, NewOrder newOrder)
        {
            Printer printer = new PrinterUnit().GetPrinterByID(printerID);
            if (newOrder == null)
            {
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

        private PrintOrder OrderEditionModel(Order_NewPostModel viewModel, string appData)
        {
            if (viewModel == null || viewModel.Order == null)
            {
                throw new Exception("Не заполнены поля на форме нового заказа");
            }
            PrintOrder order = new PrintOrderUnit().New(viewModel.Order, appData);
            return order;
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

                string app_data = HttpContext.Server.MapPath("~/App_Data");
                PrintOrder order = OrderEditionModel(model, app_data);

                Session["Printer_PreparedOrder"] = order;
                //Session["Printer_PreparedOrderFile"] = serializedFile;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                var printerModel = this._CreatePrintViewModel(model.Order.PrinterID, model.Order);
                return View("New", printerModel);
            }
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

            order = new PrinterUnit().SavePrintOrder(file, order, this.GetSmsParams());

            // Push notification about new order
            User printerOperator = new PrinterUnit().GetPrinterOperator(order.PrinterID);
            string notificationMessage = string.Format(
                "{0}: поступил новый заказ № {1}." + Environment.NewLine +
                "Количество страниц: {2}, сумма заказа: {3}р.",
                order.OrderedOn.ToString("HH:mm:ss"),
                order.ID,
                order.PagesCount,
                order.FullPrice
            );
            new PushNotificationHub().NewIncomingOrder(notificationMessage, printerOperator.UserID);

            return RedirectToAction("OrderCompleted", new { printOrderID = order.ID });
        }

        [HttpGet]
        public ActionResult OrderCompleted(int printOrderID)
        {
            var order = new PrinterUnit().GetPrintOrderByID(printOrderID);
            return View(order);
        }
    }
}