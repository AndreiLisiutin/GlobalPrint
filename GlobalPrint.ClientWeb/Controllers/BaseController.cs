using GlobalPrint.ClientWeb.Models.FilesRepository;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Utilities;
using GlobalPrint.ServerBusinessLogic.Models.Business;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb
{
    /// <summary>
    /// Базовый контроллер приложения.
    /// </summary>
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

        /// <summary>
        /// Получить параметры СМС оповещений.
        /// </summary>
        /// <returns>Параметры СМС оповещений.</returns>
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

        /// <summary>
        /// Получить идентификатор текущего пользователя.
        /// </summary>
        /// <returns>Идентификатор текущего пользователя.</returns>
        protected int GetCurrentUserID()
        {
            return User.Identity.GetUserId<int>();
        }

        /// <summary>
        /// Загрузить файл в сессию. Файл хранится в сессии на протяжении 1 часа, затем он должен быть удален.
        /// </summary>
        /// <returns>Результат загрузки файла.</returns>
        [HttpPost, Authorize]
        public virtual ActionResult UploadFile()
        {
            int userID = this.GetCurrentUserID();
            HttpPostedFileBase file = Request.Files["gpUserFile"];
            bool isUploaded = false;
            string message = "Ошибка загрузки файла.";
            Guid? fileId = null;

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