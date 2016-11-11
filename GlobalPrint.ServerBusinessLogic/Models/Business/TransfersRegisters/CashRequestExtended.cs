using GlobalPrint.ServerBusinessLogic.Models.Domain.TransfersRegisters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.Models.Business.TransfersRegisters
{
    public class CashRequestExtended
    {
        [DebuggerStepThrough]
        public CashRequestExtended()
        {
        }

        public CashRequest CashRequest { get; set; } 
        public CashRequestStatus CashRequestStatus { get; set; } 
    }
}
