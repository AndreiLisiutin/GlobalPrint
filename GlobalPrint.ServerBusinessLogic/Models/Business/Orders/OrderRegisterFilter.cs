using GlobalPrint.Infrastructure.FileUtility.FileExporters;
using System;
using System.Diagnostics;

namespace GlobalPrint.ServerBusinessLogic.Models.Business.Orders
{
    /// <summary>
    /// Filter for register report.
    /// </summary>
    public class OrderRegisterFilter
    {
        [DebuggerStepThrough]
        public OrderRegisterFilter()
        {
        }
        
        /// <summary>
        /// How to export resulting register.
        /// </summary>
        public FileExporterEnum FileExporter { get; set; }

        /// <summary>
        /// Printer's owner identifier.
        /// </summary>
        public int OwnerUserID { get; set; }

        /// <summary>
        /// Report and register begin date.
        /// </summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>
        /// Report and register end date.
        /// </summary>
        public DateTime? DateTo { get; set; }
    }
}
