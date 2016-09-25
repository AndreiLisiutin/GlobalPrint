using GlobalPrint.Configuration.DI;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Printers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Users;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using System;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb.Controllers
{
    public class UserPrintInfoController : BaseController
    {
        /// <summary>
        /// Number of waiting orders, not processed by current user
        /// </summary>
        /// <returns>Partial view with recieved print orders number</returns>
        [ChildActionOnly]
        [Authorize]
        public ActionResult UserRecievedPrintOrder()
        { 
            int printOrdersCount = new PrinterUnit().GetWaitingIncomingOrdersCount(this.GetCurrentUserID());
            ViewData["UserRecievedPrintOrdersCount"] = printOrdersCount > 0 ? printOrdersCount.ToString() : null;
            return PartialView("_UserRecievedPrintOrder");
        }

        /// <summary>
        /// Get user balance
        /// </summary>
        /// <returns>Partial view with user balance</returns>
        [ChildActionOnly]
        [Authorize]
        public ActionResult UserBalance()
        {
            UserUnit userUnit = IoC.Instance.Resolve<UserUnit>();
            User currentUser = userUnit.GetUserByID(this.GetCurrentUserID());
            if (currentUser != null)
            {
                ViewData["UserBalance"] = currentUser.AmountOfMoney.ToString("0.00");
                return PartialView("_UserBalance");
            }
            else
            {
                throw new Exception("Не найден текущий пользователь");
            }
        }
    }
}