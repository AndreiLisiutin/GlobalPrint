using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Orders
{
    public enum PrintOrderStatusEnum
    {
        Waiting = 1,
        Accepted = 2,
        Printed = 3,
        Rejected = 4
    }
}
