using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Payment
{
    public enum PaymentTransactionStatusEnum
    {
        InProgress = 1,
        Committed = 2,
        RolledBack = 3,
    }
}
