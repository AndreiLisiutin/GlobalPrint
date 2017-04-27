using Newtonsoft.Json;
using System;

namespace GlobalPrint.Infrastructure.BankUtility.BicInfo
{
    /// <summary>
    /// Realizes Adapter pattern. For converting bank web service response to BicInfo model.
    /// </summary>
    internal class BicInfoAdapter
    {
        /// <summary>
        /// Get date or null from object.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <returns>DateTime or null.</returns>
        private DateTime? GetDateTime(object obj)
        {
            DateTime temp;

            if (obj != null)
            {
                bool successParse = DateTime.TryParse(obj.ToString(), out temp);
                if (successParse)
                {
                    return temp;
                }
            }

            return null;
        }

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
            result.DateAdd = GetDateTime(obj.dateadd);
            result.DateChange = GetDateTime(obj.datechange);
            result.DocumentsPeriod = obj.srok;
            result.FullName = obj.name == null || string.IsNullOrWhiteSpace(obj.name.ToString()) 
                ? null 
                : obj.name.ToString().Replace("&quot;", "\"");
            result.Index = obj.index;
            result.Okato = obj.okato;
            result.Okpo = obj.okpo;
            result.Phone = obj.phone;
            result.RegNumber = obj.regnum;
            result.ShortName = obj.namemini == null || string.IsNullOrWhiteSpace(obj.namemini.ToString()) 
                ? null 
                : obj.namemini.ToString().Replace("&quot;", "\"");

            return result;
        }
    }
}
