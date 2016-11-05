using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.BankUtility.BankInfo
{
    public class BankUtility : IBankUtility
    {
        /// <summary>
        /// Service http://www.cbr.ru/CreditInfoWebServ/CreditOrgInfo.asmx.
        /// </summary>
        private CreditOrgInfo.CreditOrgInfo _service { get; set; }

        public BankUtility()
        {
            _service = new CreditOrgInfo.CreditOrgInfo(); //ссылка на сервис
            _service.UseDefaultCredentials = true;
            _service.CookieContainer = new System.Net.CookieContainer();
        }

        /// <summary>
        /// Get bank info from BIC of bank. Calls web service of http://www.cbr.ru/.
        /// </summary>
        /// <param name="bicCode">BIC code of bank.</param>
        /// <returns>Returns object with bank info.</returns>
        public IBankInfo GetBankInfo(string bicCode)
        {
            double intCode = _service.BicToIntCode(bicCode);
            DataSet ds = _service.CreditInfoByIntCode(intCode);
            BankInfo bankInfo = new BankInfoAdapter().GetBankInfo(ds);
            return bankInfo;
        }
    }
}
