using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.CommonUtils
{
    public class WeekUtility
    {
        public string DayName(DayOfWeek day)
        {
            return CultureInfo.GetCultureInfo("RU-ru").DateTimeFormat.GetDayName(day);
        }
        public string ShortDayName(DayOfWeek day)
        {
            return CultureInfo.GetCultureInfo("RU-ru").DateTimeFormat.GetShortestDayName(day);
        }
    }
}
