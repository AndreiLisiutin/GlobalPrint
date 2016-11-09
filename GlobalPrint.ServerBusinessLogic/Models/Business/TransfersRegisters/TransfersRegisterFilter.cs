using GlobalPrint.Infrastructure.FileUtility.FileExporters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.Models.Business.TransfersRegisters
{
    /// <summary>
    /// Filter for register report.
    /// </summary>
    public class TransfersRegisterFilter
    {
        [DebuggerStepThrough]
        public TransfersRegisterFilter()
        {
            this.FileExporter = FileExporterEnum.Excel;
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
        /// Identifier of the register to export.
        /// </summary>
        public int TransfersRegisterID { get; set; }
    }
}
