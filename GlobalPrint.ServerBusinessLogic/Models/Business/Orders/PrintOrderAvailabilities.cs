using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.Models.Business.Orders
{
    /// <summary>
    /// Cases with availability new order with certain price for current user.
    /// </summary>
    public enum PrintOrderAvailabilities
    {
        /// <summary>
        /// Totally available.
        /// </summary>
        Available = 1,
        /// <summary>
        /// Available but user will have to take a debt.
        /// </summary>
        AvailableWithDebt = 2,
        /// <summary>
        /// Totally unavailable.
        /// </summary>
        Unavailable = 3,
    }
}
