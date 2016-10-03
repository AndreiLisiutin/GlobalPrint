using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GlobalPrint.Infrastructure.LogUtility.Robokassa
{
    public static class Robokassa
    {

        public static string GetRedirectUrl(decimal price, int orderId, string email = "")
        {
            // ugly code, legacy from Robokassa website

            // your registration data
            string sMrchLogin = RobokassaConfig.Login;
            string sMrchPass1 = RobokassaConfig.Pass1;
            // order properties
            int nInvId = orderId;
            string sDesc = HttpUtility.UrlEncode($"Оплата заказа на распечатку заказа №{orderId} в Global print.");

            string sOutSum = price.ToString("0.00", CultureInfo.InvariantCulture);
            string sCrcBase = string.Format("{0}:{1}:{2}:{3}",
                                             sMrchLogin, sOutSum, nInvId, sMrchPass1);

            // build CRC value
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bSignature = md5.ComputeHash(Encoding.ASCII.GetBytes(sCrcBase));

            StringBuilder sbSignature = new StringBuilder();
            foreach (byte b in bSignature)
                sbSignature.AppendFormat("{0:x2}", b);

            string sCrc = sbSignature.ToString();

            return getBaseUrl() +
                "MrchLogin=" + sMrchLogin +
                "&OutSum=" + sOutSum +
                "&InvId=" + nInvId +
                "&Desc=" + sDesc +
                "&IsTest=" + (RobokassaConfig.Mode == RobokassaMode.Test ? "1" : "0") +
                "&SignatureValue=" + sCrc +
                (String.IsNullOrEmpty(email) ? "" : "&Email=" + email);
        }

        private static string getBaseUrl()
        {
            switch(RobokassaConfig.Mode)
            {
                case RobokassaMode.Test:
                case RobokassaMode.Production:
                    return "https://auth.robokassa.ru/Merchant/Index.aspx?";
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
