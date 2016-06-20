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
using System.IO;

namespace GlobalPrint.ClientWeb
{
    public class UserRecievedPrintOrderListController : BaseController
    {
        // GET: UserRecievedPrintOrderList/UserRecievedPrintOrderList
        [HttpGet]
        public ActionResult UserRecievedPrintOrderList()
        {
            int userID = Request.RequestContext.HttpContext.User.Identity.GetUserId<int>();
            var printOrderList = new PrintOrderBll().GetUserRecievedPrintOrderList(userID);
            return View(printOrderList);
        }

        [HttpPost]
        public ActionResult AcceptOrder(int printOrderID)
        {
            new PrintOrderBll().UpdateStatus(printOrderID, PrintOrderStatusEnum.Accepted, this.GetSmsParams());
            return RedirectToAction("UserRecievedPrintOrderList");
        }

        [HttpPost]
        public ActionResult RejectOrder(int printOrderID)
        {
            new PrintOrderBll().UpdateStatus(printOrderID, PrintOrderStatusEnum.Rejected, this.GetSmsParams());
            return RedirectToAction("UserRecievedPrintOrderList");
        }

        [HttpPost]
        public ActionResult PrintOrder(int printOrderID)
        {
            new PrintOrderBll().UpdateStatus(printOrderID, PrintOrderStatusEnum.Printed, this.GetSmsParams());
            return RedirectToAction("UserRecievedPrintOrderList");
        }

        [HttpGet]
        public ActionResult DownloadOrder(int printOrderID)
        {
            var order = new PrinterBll().GetPrintOrderByID(printOrderID);
            byte[] fileBytes = System.IO.File.ReadAllBytes(order.Document);
            string fileName = new FileInfo(order.Document).Name;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
}
