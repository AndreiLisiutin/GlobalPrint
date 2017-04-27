using System.Data;

namespace GlobalPrint.Infrastructure.BankUtility.BankInfo
{
    /// <summary>
    /// Утилита для получения данных о банке по его БИК. Использует сервис ЦБР.
    /// </summary>
    public class BankUtility : IBankUtility
    {
        /// <summary>
        /// Сервис ЦБР. 
        /// http://www.cbr.ru/CreditInfoWebServ/CreditOrgInfo.asmx.
        /// </summary>
        private readonly CreditOrgInfo.CreditOrgInfo _service
            = new CreditOrgInfo.CreditOrgInfo()
            {
                UseDefaultCredentials = true,
                CookieContainer = new System.Net.CookieContainer()
            };

        /// <summary>
        /// Получить данные о банке по его БИК.
        /// </summary>
        /// <param name="bicCode">БИК банка.</param>
        /// <returns>Информация о банке.</returns>
        public IBankInfo GetBankInfo(string bicCode)
        {
            double intCode = _service.BicToIntCode(bicCode);
            DataSet ds = _service.CreditInfoByIntCode(intCode);
            var bankInfo = new BankInfoAdapter().GetBankInfo(ds);
            return bankInfo;
        }
    }
}
