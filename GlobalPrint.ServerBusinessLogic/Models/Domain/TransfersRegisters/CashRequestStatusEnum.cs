using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.Models.Domain.TransfersRegisters
{
    public enum CashRequestStatusEnum
    {
        InProgress = 1,
        Committed = 2,
        RolledBack = 3,
    }
}
