using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Payment
{
    public enum PaymentActionStatusEnum
    {
        Wait = 1,
        InProgress = 2,
        ExecutedSuccessfully = 3,
        Failed = 4,
    }
}
