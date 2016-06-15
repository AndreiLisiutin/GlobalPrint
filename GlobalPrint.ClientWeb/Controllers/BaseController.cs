using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb
{
    public class BaseController : Controller
    {
        public new JsonResult Json(object data)
        {
            return base.Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}