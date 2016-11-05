using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.BankUtility.BicInfo
{
    /// <summary>
    /// Realizes Adapter pattern. For converting bank web service response to BicInfo model.
    /// </summary>
    internal class BicInfoAdapter
    {
        /// <summary>
        /// Adapter. Converts dataset from web service to BicInfo object.
        /// </summary>
        /// <param name="jsonString">Json string with results of web service call.</param>
        /// <returns>Returns BicInfo object.</returns>
        public BicInfo GetBankInfo(string jsonString)
        {
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                return null;
            }

            dynamic obj = JsonConvert.DeserializeObject(jsonString);

            BicInfo result = new BicInfo();

            result.Address = obj.address;
            result.Bic = obj.bik;
            result.City = obj.city;
            result.CorrespondentAccount = obj.ks;
            result.DateAdd = obj.dateadd == null || string.IsNullOrWhiteSpace(obj.dateadd.ToString()) ? null : DateTime.Parse(obj.dateadd.ToString());
            result.DateChange = obj.datechange == null || string.IsNullOrWhiteSpace(obj.datechange.ToString()) ? null : DateTime.Parse(obj.datechange.ToString());
            result.DocumentsPeriod = obj.srok;
            result.FullName = obj.name;
            result.Index = obj.index;
            result.Okato = obj.okato;
            result.Okpo = obj.okpo;
            result.Phone = obj.phone;
            result.RegNumber = obj.regnum;
            result.ShortName = obj.namemini;

            return result;
        }
    }
}
