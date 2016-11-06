using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.BankUtility.BankInfo
{
    /// <summary>
    /// Realizes Adapter pattern. For converting bank web service response to BankInfo model.
    /// </summary>
    internal class BankInfoAdapter
    {
        /// <summary>
        /// Adapter. Converts dataset from web service to BankInfo object.
        /// </summary>
        /// <param name="ds">Dataset from web service with data about bank.</param>
        /// <returns>Returns BankInfo object.</returns>
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
