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
    public class UserAccountPrinterListController : BaseController
    {
        // GET: UserAccountPrinterList/UserAccountPrinterList
        [HttpGet]
        public ActionResult UserAccountPrinterList()
        {
            int UserID = Request.RequestContext.HttpContext.User.Identity.GetUserId<int>();
            var printerList = new PrinterBll().GetUserPrinterList(UserID);
            return View(printerList);
        }
    }
}
