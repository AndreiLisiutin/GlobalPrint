using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Utilities;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb
{
    public class BaseController : Controller
    {
        protected new JsonResult Json(object data)
        {
            return base.Json(data, JsonRequestBehavior.AllowGet);
        }

        protected SmsUtility.Parameters GetSmsParams()
        {
            return new SmsUtility.Parameters()
            {
                Host = WebConfigurationManager.AppSettings["SmppHost"],
                Login = WebConfigurationManager.AppSettings["SmppLogin"],
                Password = WebConfigurationManager.AppSettings["SmppPassword"],
                Port = WebConfigurationManager.AppSettings["SmppPort"],
                Sender = WebConfigurationManager.AppSettings["SmppSender"],
                Enabled = WebConfigurationManager.AppSettings["SmppEnabled"] == "1",
            };
        }
        
    }
}