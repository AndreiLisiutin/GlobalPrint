using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Server.Utilities
{
    public static class StringExtension
    {
        public static decimal ConvertCurrentcyToDecimal(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new Exception("Не задана строка для перевода в денежный формат");
            }

            NumberFormatInfo format = new NumberFormatInfo();
            format.CurrencyDecimalSeparator = ".";
            format.CurrencyDecimalDigits = 99;
            return decimal.Parse(str.Replace(",", "."), NumberStyles.Currency | NumberStyles.AllowDecimalPoint, format);
        }
    }
}
