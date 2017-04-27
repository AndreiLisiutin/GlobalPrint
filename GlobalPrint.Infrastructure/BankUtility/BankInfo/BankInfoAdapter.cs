using System;
using System.Data;

namespace GlobalPrint.Infrastructure.BankUtility.BankInfo
{
    /// <summary>
    /// Адаптер для конвертирования ответа сервиса ЦБР в класс данных о банке.
    /// </summary>
    internal class BankInfoAdapter
    {
        /// <summary>
        /// Адаптер. Конвертирует полученный из веб сервиса DataSet в класс.
        /// </summary>
        /// <param name="ds">Полученный из веб сервиса DataSet с данными о банке.</param>
        /// <returns>Экземпляр класса BankInfo.</returns>
        public BankInfo GetBankInfo(DataSet ds)
        {
            if (ds != null && ds.Tables.Count > 0 && ds.Tables["CO"] != null)
            {
                DataTable dt = ds.Tables["CO"];
                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    return new BankInfo()
                    {
                        RegNumber = dr["RegNumber"] as string,
                        Bic = dr["BIC"].ToString(),
                        ShortName = dr["OrgName"].ToString(),
                        FullName = dr["OrgFullName"].ToString(),
                        Phone = dr["phones"].ToString(),
                        DateKGRRegistration = dr["DateKGRRegistration"] as DateTime?,
                        MainRegNumber = dr["MainRegNumber"].ToString(),
                        MainDateReg = dr["MainDateReg"] as DateTime?,
                        UstavAdr = dr["UstavAdr"].ToString(),
                        FactAdr = dr["FactAdr"].ToString(),
                        Director = dr["Director"].ToString(),
                        UstMoney = dr["UstMoney"] as string,
                        OrgStatus = dr["OrgStatus"].ToString(),
                        RegCode = dr["RegCode"] as string,
                        SSV_Date = dr["SSV_Date"] as DateTime?
                    };
                }
            }

            return null;
        }
    }
}
