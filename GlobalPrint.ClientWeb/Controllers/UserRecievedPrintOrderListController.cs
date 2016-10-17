using GlobalPrint.ClientWeb.Filters;
using GlobalPrint.Configuration.DI;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.UnitsOfWork.Order;
using GlobalPrint.ServerBusinessLogic.Models.Business;
using GlobalPrint.ServerBusinessLogic.Models.Business.Orders;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Orders;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb
{
    public class UserRecievedPrintOrderListController : BaseController
    {
        /// <summary>
        /// Get received print order by ID.
        /// </summary>
        /// <param name="printOrderID">Identifier of print order.</param>
        /// <returns>Print order view.</returns>
        [Authorize, HttpGet/*, ImportModelState*/]
        public ActionResult UserRecievedPrintOrderList(string printOrderID)
        {
            var printOrderList = _GetViewModel(printOrderID);
            return View(printOrderList);
        }
        private List<PrintOrderInfo> _GetViewModel(string printOrderID)
        {
            PrintOrderUnit printOrderUnit = IoC.Instance.Resolve<PrintOrderUnit>();
            int userID = Request.RequestContext.HttpContext.User.Identity.GetUserId<int>();
            return printOrderUnit.GetUserRecievedPrintOrderList(userID, printOrderID);
        }

        /// <summary>
        /// Confirm print order.
        /// </summary>
        /// <param name="printOrderID">Identifier of print order to confirm.</param>
        /// <returns>Redirect to list of received order list.</returns>
        [Authorize, HttpPost/*, ExportModelState*/]
        public ActionResult AcceptOrder(int printOrderID)
        {
            PrintOrderUnit printOrderUnit = IoC.Instance.Resolve<PrintOrderUnit>();
            int userID = this.GetCurrentUserID();
            printOrderUnit.UpdateStatus(printOrderID, PrintOrderStatusEnum.Accepted, userID);

            if (Request.IsAjaxRequest())
            {
                return JavaScript("document.location.replace('" + Url.Action("UserRecievedPrintOrderList", "UserRecievedPrintOrderList") + "');");
            }
            else
            {
                return RedirectToAction("UserRecievedPrintOrderList");
            }
        }

        /// <summary>
        /// Reject print order.
        /// </summary>
        /// <param name="printOrderID">Identifier of print order to reject.</param>
        /// <returns>Redirect to list of received order list.</returns>
        [Authorize, HttpPost/*, ExportModelState*/]
        public ActionResult RejectOrder(int printOrderID)
        {
            PrintOrderUnit printOrderUnit = IoC.Instance.Resolve<PrintOrderUnit>();
            int userID = this.GetCurrentUserID();
            printOrderUnit.UpdateStatus(printOrderID, PrintOrderStatusEnum.Rejected, userID);

            if (Request.IsAjaxRequest())
            {
                return JavaScript("document.location.replace('" + Url.Action("UserRecievedPrintOrderList", "UserRecievedPrintOrderList") + "');");
            }
            else
            {
                return RedirectToAction("UserRecievedPrintOrderList");
            }
        }

        /// <summary>
        /// Realize order - real printing.
        /// </summary>
        /// <param name="printOrderID">Identifier of print order.</param>
        /// <param name="secretCode">Secret code of print order, entered by customer.</param>
        /// <returns>Redirects to orders list.</returns>
        [Authorize, HttpPost]
        public ActionResult PrintOrder(int printOrderID, string secretCode)
        {
            PrintOrderUnit printOrderUnit = IoC.Instance.Resolve<PrintOrderUnit>();
            PrintOrder order = printOrderUnit.GetByID(printOrderID);

            if (order.SecretCode.ToUpper() != (secretCode ?? "").ToUpper())
            {
                ModelState.AddModelError("", "Некорректный секретный код");
                var vm = ViewModelConfirmPrintOrder(printOrderID);
                return View("ConfirmPrintOrder", vm);
            }
            int userID = this.GetCurrentUserID();
            printOrderUnit.UpdateStatus(printOrderID, PrintOrderStatusEnum.Printed, userID);
            return RedirectToAction("UserRecievedPrintOrderList");
        }

        /// <summary>
        /// Get view model for print order confirmation.
        /// </summary>
        /// <param name="printOrderID">Identifier of print order.</param>
        /// <returns>Returns print order information.</returns>
        private PrintOrderInfo ViewModelConfirmPrintOrder(int printOrderID)
        {
            PrintOrderUnit printOrderUnit = IoC.Instance.Resolve<PrintOrderUnit>();
            return printOrderUnit.GetPrintOrderInfoByID(printOrderID);
        }

        /// <summary>
        /// Confirm print order. Your cap.
        /// </summary>
        /// <param name="printOrderID">Identifier of print order.</param>
        /// <returns>Redirects to ConfirmPrintOrder view.</returns>
        [Authorize, HttpGet]
        public ActionResult ConfirmPrintOrder(int printOrderID)
        {
            var vm = ViewModelConfirmPrintOrder(printOrderID);
            return View(vm);
        }
    }
}
