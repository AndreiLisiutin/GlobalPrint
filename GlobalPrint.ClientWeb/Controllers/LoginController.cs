using GlobalPrint.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb
{
    public class LoginController : BaseController
    {
        // GET: Home
        public ActionResult Login()
        {
            return View();
        }
    }
}
