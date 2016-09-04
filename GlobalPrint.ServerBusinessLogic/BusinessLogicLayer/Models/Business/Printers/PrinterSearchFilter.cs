using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Printers
{
    public class PrinterSearchFilter
    {
        /// <summary> Any, operator or owner.
        /// </summary>
        public int? UserID { get; set; }
    }
}
