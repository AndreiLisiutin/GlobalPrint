using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Printers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb.Controllers
{
    public class UserPrintInfoController : BaseController
    {
        [ChildActionOnly]
        public ActionResult UserRecievedPrintOrder()
        { 
            // Number of waiting orders, not processed by current user
            int printOrdersCount = new PrinterUnit().GetWaitingIncomingOrdersCount(this.GetCurrentUserID());
            ViewData["UserRecievedPrintOrdersCount"] = printOrdersCount > 0 ? printOrdersCount.ToString() : null;
            return PartialView("_UserRecievedPrintOrder");
        }
    }
}