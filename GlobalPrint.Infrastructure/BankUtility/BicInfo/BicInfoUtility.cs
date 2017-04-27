using System.Net;

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
                string jsonString = wc.DownloadString($"{_webService}&bik={bicCode}");
                return new BicInfoAdapter().GetBankInfo(jsonString);
            }
        }
    }
}
