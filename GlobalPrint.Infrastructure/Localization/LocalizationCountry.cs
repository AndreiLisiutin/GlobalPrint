using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.Localization
{
    public class LocalizationCountry
    {
        public string FlagImage { get; set; }
        public string DisplayName { get; set; }
        public CultureInfo Culture { get; set; }
    }
}
