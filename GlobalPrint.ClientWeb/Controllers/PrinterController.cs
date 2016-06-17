using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb.Controllers
{
    public class PrinterController : Controller
    {
        public ActionResult Print(int printerID)
        {

            return View();
        }
	}
}