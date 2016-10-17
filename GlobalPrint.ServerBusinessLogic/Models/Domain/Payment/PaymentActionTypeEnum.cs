using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.Models.Domain.Payment
{
    /// <summary>
    /// Type of payment action.
    /// </summary>
    public enum PaymentActionTypeEnum
    {
        /// <summary>
        /// Refilling the balance.
        /// </summary>
        BalanceRefill = 1,
        /// <summary>
        /// Payment for the order.
        /// </summary>
        PaymentForOrder = 2,
        /// <summary>
        /// Sending money between the accounts.
        /// </summary>
        SendMoney = 3
    }
}
