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
    public class ReportsController : BaseController
    {
        PrintOrderRegistersUnit _printOrderRegistersUnit;

        private IFileUtility _fileUtility;
        private IMimeTypeUtility _mimeTypeUtility;

        public ReportsController()
        {
            this._printOrderRegistersUnit = new PrintOrderRegistersUnit();
            this._fileUtility = IoC.Instance.Resolve<IFileUtility>();
            this._mimeTypeUtility = IoC.Instance.Resolve<IMimeTypeUtility>();
        }

        /// <summary>
        /// Show page with filters for the register of orders.
        /// </summary>
        /// <returns></returns>
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
            filter.OwnerUserID = this.GetCurrentUserID();
            DocumentBusinessInfo file = this._printOrderRegistersUnit.OrderRegisterExport(filter);
            string mimeType = this._mimeTypeUtility.ConvertExtensionToMimeType(file.Extension);
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
            filter.OwnerUserID = this.GetCurrentUserID();
            DocumentBusinessInfo file = this._printOrderRegistersUnit.TransfersRegisterExport(filter);
            string mimeType = this._mimeTypeUtility.ConvertExtensionToMimeType(file.Extension);
            return File(file.SerializedFile, mimeType, file.Name);
        }

        /// <summary>
        /// Create view PrintOrderRegister.
        /// </summary>
        /// <param name="filter">View model.</param>
        /// <returns></returns>
        private ViewResult _PRINT_ORDER_REGISTER(OrderRegisterFilter filter)
        {
            Dictionary<FileExporterEnum, string> exportTypes = this._fileUtility.GetFileExporterTypes();
            ViewBag.ExportTypesList = exportTypes
                .Select(e => new SelectListItem() { Text = e.Value, Value = ((int)e.Key).ToString() })
                .ToList();
            
            return this.View("PrintOrderRegister", filter);
        }
    }
}