using GlobalPrint.ClientWeb.Models;
using GlobalPrint.ClientWeb.Models.FilesRepository;
using GlobalPrint.Configuration.DI;
using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.Infrastructure.LogUtility;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Utilities;
using GlobalPrint.ServerBusinessLogic.Models.Business;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb
{
    public class BaseController : Controller
    {
        private const string _uploadedFilesDictionaryKey = "_UploadFiles";
        protected IUploadFilesRepository _uploadedFilesRepo;
        public BaseController()
        {
            Func<Dictionary<Guid, DocumentBusinessInfo>> uploadFilesDict = () =>
            {
                Dictionary<Guid, DocumentBusinessInfo> dict = this.Session[_uploadedFilesDictionaryKey] as Dictionary<Guid, DocumentBusinessInfo>;
                if (dict == null)
                {
                    this.Session[_uploadedFilesDictionaryKey] = dict = new Dictionary<Guid, DocumentBusinessInfo>();
                }
                return dict;
            };
            this._uploadedFilesRepo = new UploadFilesRepository(uploadFilesDict);
        }
        public BaseController(Func<Dictionary<Guid, DocumentBusinessInfo>> uploadFilesDict)
        {
            this._uploadedFilesRepo = new UploadFilesRepository(uploadFilesDict);
        }

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

        /// <summary>
        /// Upload file into session. File is storing inside the session one hour then it is to be removed.
        /// </summary>
        /// <returns></returns>
        [HttpPost, Authorize]
        public virtual ActionResult UploadFile()
        {
            int userID = this.GetCurrentUserID();
            HttpPostedFileBase file = Request.Files["gpUserFile"];
            bool isUploaded = false;
            string message = "Ошибка загрузки файла.";
            Guid? fileId = null;

            #region Для тестирования, удалить потом

            //ILoggerFactory factory = IoC.Instance.Resolve<ILoggerFactory>();
            //ILogger logger = factory.GetCurrentClassLogger();
            
            //string log = "File: " + Environment.NewLine;
            //log += "ContentLength: " + file.ContentLength + Environment.NewLine;
            //log += "ContentType: " + file.ContentType + Environment.NewLine;
            //log += "FileName: " + file.FileName + Environment.NewLine;

            //byte[] bytes = Encoding.Default.GetBytes(file.FileName);
            //string newFileName = Encoding.UTF8.GetString(bytes);
            //log += "DefaultNewFileName: " + newFileName + Environment.NewLine;

            //bytes = Encoding.ASCII.GetBytes(file.FileName);
            //newFileName = Encoding.UTF8.GetString(bytes);
            //log += "ASCIINewFileName: " + newFileName + Environment.NewLine;

            //bytes = Encoding.Unicode.GetBytes(file.FileName);
            //newFileName = Encoding.UTF8.GetString(bytes);
            //log += "UnicodeNewFileName: " + newFileName + Environment.NewLine;

            //newFileName = HttpUtility.UrlEncode(file.FileName, Encoding.UTF8);
            //log += "UrlEncodeNewFileName: " + newFileName + Environment.NewLine;
                        
            #endregion

            if (file != null && file.ContentLength != 0)
            {
                fileId = this._uploadedFilesRepo.Add(file, userID);
                isUploaded = true;
                message = "Файл успешно загружен.";
            }

            return Json(new { isUploaded = isUploaded, message = message, fileId = fileId }, "text/html;charset=utf-8");
        }
    }
}