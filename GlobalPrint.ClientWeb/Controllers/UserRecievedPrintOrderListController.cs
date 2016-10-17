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
        // GET: UserRecievedPrintOrderList/UserRecievedPrintOrderList
        [HttpGet]
        [Authorize]
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

        [HttpPost]
        [Authorize]
        public ActionResult AcceptOrder(int printOrderID)
        {
            PrintOrderUnit printOrderUnit = IoC.Instance.Resolve<PrintOrderUnit>();
            int userID = this.GetCurrentUserID();
            printOrderUnit.UpdateStatus(printOrderID, PrintOrderStatusEnum.Accepted, userID);
            return RedirectToAction("UserRecievedPrintOrderList");
        }

        [HttpPost]
        [Authorize]
        public ActionResult RejectOrder(int printOrderID)
        {
            PrintOrderUnit printOrderUnit = IoC.Instance.Resolve<PrintOrderUnit>();
            int userID = this.GetCurrentUserID();
            printOrderUnit.UpdateStatus(printOrderID, PrintOrderStatusEnum.Rejected, userID);
            return RedirectToAction("UserRecievedPrintOrderList");
        }

        [HttpPost]
        [Authorize]
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

        private PrintOrderInfo ViewModelConfirmPrintOrder(int printOrderID)
        {
            PrintOrderUnit printOrderUnit = IoC.Instance.Resolve<PrintOrderUnit>();
            return printOrderUnit.GetPrintOrderInfoByID(printOrderID);
        }

        [HttpPost]
        [Authorize]
        public ActionResult ConfirmPrintOrder(int printOrderID)
        {
            var vm = ViewModelConfirmPrintOrder(printOrderID);
            return View(vm);
        }
    }
}
