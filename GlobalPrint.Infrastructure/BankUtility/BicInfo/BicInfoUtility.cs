using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GlobalPrint.Infrastructure.BankUtility.BicInfo
{
    public class BicInfoUtility : IBankUtility
    {
        private readonly string _webService = @"http://www.bik-info.ru/api.html?type=json";

        public IBankInfo GetBankInfo(string bicCode)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = System.Text.Encoding.UTF8;
                string fullServiceString = string.Format("{0}&bik={1}", _webService, bicCode);
                string jsonString = wc.DownloadString(fullServiceString);
                BicInfo result = new BicInfoAdapter().GetBankInfo(jsonString);
                return result;
            }
        }
    }
}
