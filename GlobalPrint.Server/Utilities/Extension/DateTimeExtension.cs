using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Server.Utilities
{
    public static class DateTimeExtension
    {
        public static bool IsMinValueOrNull(this DateTime? t)
        {
            return !t.HasValue || t == DateTime.MinValue;
        }

        public static string ToDMY(this DateTime? t)
        {
            return t.IsMinValueOrNull() ? null : t.Value.ToString("dd.MM.yyyy");
        }

        public static string ToDMYOrEmpty(this DateTime? t)
        {
            return t.ToDMY() ?? "";
        }

        public static string ToDMY(this DateTime t)
        {
            return ((DateTime?)t).ToDMY();
        }
    }
}
