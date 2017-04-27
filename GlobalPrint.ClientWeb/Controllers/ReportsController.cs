using GlobalPrint.Configuration.DI;
using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.Infrastructure.FileUtility;
using GlobalPrint.Infrastructure.FileUtility.FileExporters;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Orders;
using GlobalPrint.ServerBusinessLogic.Models.Business;
using GlobalPrint.ServerBusinessLogic.Models.Business.Orders;
using GlobalPrint.ServerBusinessLogic.Models.Business.TransfersRegisters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb.Controllers
{
    /// <summary>
    /// Контроллер реестров.
    /// </summary>
    public class ReportsController : BaseController
    {
        /// <summary>
        /// Модуль бизнес логики для реестров перечислений.
        /// </summary>
        private readonly PrintOrderRegistersUnit _printOrderRegistersUnit;

        /// <summary>
        /// Утилита для работы с файламию.
        /// </summary>
        private readonly IFileUtility _fileUtility;

        /// <summary>
        /// Утилита для работы с mime типами файлов.
        /// </summary>
        private readonly IMimeTypeUtility _mimeTypeUtility;

        public ReportsController(IMimeTypeUtility mimeTypeUtility, IFileUtility fileUtility)
        {
            _printOrderRegistersUnit = IoC.Instance.Resolve<PrintOrderRegistersUnit>();
            _fileUtility = fileUtility;
            _mimeTypeUtility = mimeTypeUtility;
        }


        /// <summary>
        /// Показать страницу с фильтрами для реестра заказов.
        /// </summary>
        /// <returns>Страница с фильтрами для реестра заказов.</returns>
        [HttpGet, Authorize]
        public ActionResult PrintOrderRegister()
        {
            OrderRegisterFilter filter = new OrderRegisterFilter()
            {
                OwnerUserID = this.GetCurrentUserID()
            };
            return _PRINT_ORDER_REGISTER(filter);
        }
        
        /// <summary>
        /// Perform calculating the register of orders for the certain period and for certain user as printer's owner.
        /// </summary>
        /// <param name="filter">Filter for the register.</param>
        /// <returns>Register file info.</returns>
        [HttpGet, Authorize]
        public ActionResult GetPrintOrderRegister(OrderRegisterFilter filter)
        {
            filter = filter ?? new OrderRegisterFilter();
            filter.OwnerUserID = GetCurrentUserID();

            DocumentBusinessInfo file = _printOrderRegistersUnit.OrderRegisterExport(filter);
            string mimeType = _mimeTypeUtility.ConvertExtensionToMimeType(file.Extension);
            return File(file.SerializedFile, mimeType, file.Name);
        }

        /// <summary>
        /// Perform calculating the register of transfers by its identifier.
        /// </summary>
        /// <param name="filter">Filter for the register.</param>
        /// <returns>Register file info.</returns>
        [HttpGet, Authorize]
        public ActionResult GetTransfersRegister(TransfersRegisterFilter filter)
        {
            filter = filter ?? new TransfersRegisterFilter();
            filter.OwnerUserID = GetCurrentUserID();

            DocumentBusinessInfo file = _printOrderRegistersUnit.TransfersRegisterExport(filter);
            string mimeType = _mimeTypeUtility.ConvertExtensionToMimeType(file.Extension);
            return File(file.SerializedFile, mimeType, file.Name);
        }


        /// <summary>
        /// Create view PrintOrderRegister.
        /// </summary>
        /// <param name="filter">View model.</param>
        /// <returns></returns>
        private ViewResult _PRINT_ORDER_REGISTER(OrderRegisterFilter filter)
        {
            Dictionary<FileExporterEnum, string> exportTypes = _fileUtility.GetFileExporterTypes();
            ViewBag.ExportTypesList = exportTypes
                .Select(e => new SelectListItem() { Text = e.Value, Value = ((int)e.Key).ToString() })
                .ToList();
            
            return View("PrintOrderRegister", filter);
        }
    }
}