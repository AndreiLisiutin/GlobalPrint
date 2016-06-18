using GlobalPrint.Server;
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

namespace GlobalPrint.ClientWeb
{
    public class UserPrintOrderListController : BaseController
    {
        // GET: UserPrintOrderList/UserPrintOrderList
        [HttpGet]
        public ActionResult UserPrintOrderList()
        {
            int userID = Request.RequestContext.HttpContext.User.Identity.GetUserId<int>();
            var printOrderList = new PrintOrderBll().GetUserPrintOrderList(userID);
            return View(printOrderList);
        }
    }
}
