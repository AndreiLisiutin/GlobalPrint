using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Utilities;
using GlobalPrint.ServerBusinessLogic.Models.Business;
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

        protected int GetCurrentUserID()
        {
            return User.Identity.GetUserId<int>();
        }
        protected string GetCurrentUserName()
        {
            return User.Identity.GetUserName();
        }
        
        /// <summary>
        /// Upload file into session.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public virtual ActionResult UploadFile()
        {
            HttpPostedFileBase file = Request.Files["gpUserFile"];
            bool isUploaded = false;
            string message = "Ошибка загрузки файла.";
            Guid fileId = new Guid();

            if (file != null && file.ContentLength != 0)
            {
                fileId = Guid.NewGuid();
                DocumentBusinessInfo printFile = DocumentBusinessInfo.FromHttpPostedFileBase(file);
                this._Uploaded.Add(fileId, printFile);
                isUploaded = true;
                message = "Файл успешно загружен.";
            }

            return Json(new { isUploaded = isUploaded, message = message, fileId = fileId }, "text/html");
        }

        /// <summary> Uploaded files in memory. Will die if user will decide not to print them.
        /// </summary>
        protected Dictionary<Guid, DocumentBusinessInfo> _Uploaded
        {
            get
            {
                Dictionary<Guid, DocumentBusinessInfo> _uploaded = this.Session["UploadFiles"]
                    as Dictionary<Guid, DocumentBusinessInfo>;
                if (_uploaded == null)
                {
                    this.Session["UploadFiles"] = _uploaded = new Dictionary<Guid, DocumentBusinessInfo>();
                }
                return _uploaded;
            }
        }
    }
}