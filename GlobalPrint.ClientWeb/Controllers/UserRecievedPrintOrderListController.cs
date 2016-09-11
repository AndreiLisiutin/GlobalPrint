using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.IO;
using System.Collections.Generic;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Printers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Orders;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.UnitsOfWork.Order;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Orders;
using GlobalPrint.Configuration.DI;

namespace GlobalPrint.ClientWeb
{
    public class UserRecievedPrintOrderListController : BaseController
    {
        // GET: UserRecievedPrintOrderList/UserRecievedPrintOrderList
        [HttpGet]
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
        public ActionResult AcceptOrder(int printOrderID)
        {
            PrintOrderUnit printOrderUnit = IoC.Instance.Resolve<PrintOrderUnit>();
            printOrderUnit.UpdateStatus(printOrderID, PrintOrderStatusEnum.Accepted, this.GetSmsParams());
            return RedirectToAction("UserRecievedPrintOrderList");
        }

        [HttpPost]
        public ActionResult RejectOrder(int printOrderID)
        {
            PrintOrderUnit printOrderUnit = IoC.Instance.Resolve<PrintOrderUnit>();
            printOrderUnit.UpdateStatus(printOrderID, PrintOrderStatusEnum.Rejected, this.GetSmsParams());
            return RedirectToAction("UserRecievedPrintOrderList");
        }

        [HttpPost]
        public ActionResult PrintOrder(int printOrderID, string secretCode)
        {
            PrintOrderUnit printOrderUnit = IoC.Instance.Resolve<PrintOrderUnit>();
            var order = new PrinterUnit().GetPrintOrderByID(printOrderID);
            if (order.SecretCode.ToUpper() != (secretCode ?? "").ToUpper())
            {
                ModelState.AddModelError("", "Некорректный секретный код");
                var vm = ViewModelConfirmPrintOrder(printOrderID);
                return View("ConfirmPrintOrder", vm);
            }
            printOrderUnit.UpdateStatus(printOrderID, PrintOrderStatusEnum.Printed, this.GetSmsParams());
            return RedirectToAction("UserRecievedPrintOrderList");
        }
        private PrintOrderInfo ViewModelConfirmPrintOrder(int printOrderID)
        {
            PrintOrderUnit printOrderUnit = IoC.Instance.Resolve<PrintOrderUnit>();
            return printOrderUnit.GetPrintOrderInfoByID(printOrderID);
        }

        [HttpPost]
        public ActionResult ConfirmPrintOrder(int printOrderID)
        {
            var vm = ViewModelConfirmPrintOrder(printOrderID);
            return View(vm);
        }

        [HttpGet]
        public ActionResult DownloadOrder(int printOrderID)
        {
            var order = new PrinterUnit().GetPrintOrderByID(printOrderID);
            byte[] fileBytes = System.IO.File.ReadAllBytes(order.Document);
            string fileName = new FileInfo(order.Document).Name;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
}
