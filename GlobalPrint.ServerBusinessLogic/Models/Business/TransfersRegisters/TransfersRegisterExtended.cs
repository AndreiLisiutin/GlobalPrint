using GlobalPrint.ServerBusinessLogic.Models.Domain.TransfersRegisters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.Models.Business.TransfersRegisters
{
    public class TransfersRegisterExtended
    {
        [DebuggerStepThrough]
        public TransfersRegisterExtended()
        {
        }

        public TransfersRegister TransfersRegister { get; set; } 
        public int RequestsCount { get; set; } 
        public decimal AmountOfMoneySumm { get; set; } 
    }
}
