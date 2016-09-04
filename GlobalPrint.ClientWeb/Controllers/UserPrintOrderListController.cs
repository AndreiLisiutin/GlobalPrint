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
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.UnitsOfWork.Order;

namespace GlobalPrint.ClientWeb
{
    public class UserPrintOrderListController : BaseController
    {
        // GET: UserPrintOrderList/UserPrintOrderList
        [HttpGet]
        public ActionResult UserPrintOrderList(string printOrderID)
        {
            int userID = Request.RequestContext.HttpContext.User.Identity.GetUserId<int>();
            var printOrderList = new PrintOrderUnit().GetUserPrintOrderList(userID, printOrderID);
            return View(printOrderList);
        }
    }
}
