﻿using GlobalPrint.Server;
using GlobalPrint.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb
{
    public class HomeController : BaseController
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetPrinters()
        {
            List<Printer> printers = new HomeBll().GetPrinters();
            return Json(printers);
        }
    }
}
