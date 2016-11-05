using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.BankUtility
{
    public interface IBankUtility
    {
        /// <summary>
        /// Get bank info from BIC of bank.
        /// </summary>
        /// <param name="bicCode">BIC code of bank.</param>
        /// <returns>Returns object with bank info.</returns>
        IBankInfo GetBankInfo(string bicCode);
    }
}
