using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Orders
{
    public class NewOrder
    {
        public int PrinterID { get; set; }
        public string SecretCode { get; set; }
        public int PrintTypeID { get; set; }
        public int PrintSizeID { get; set; }
        public bool IsColored { get; set; }
        public bool IsTwoSided { get; set; }
        public string FileToPrint { get; set; }
        public int CopiesCount { get; set; }
        public string Comment { get; set; }
        public int UserID { get; set; }
    }
}
