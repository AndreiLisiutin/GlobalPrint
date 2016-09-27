using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;

namespace GlobalPrint.ClientWeb.Helpers
{
    public class LocalizationCountry
    {
        public string FlagImage { get; set; }
        public string DisplayName { get; set; }
        public CultureInfo Culture { get; set; }
    }

    public static class LocalizationHelper
    {
        /// <summary>
        /// Default culture.
        /// </summary>
        private static readonly string _defaultCulture = "ru";

        /// <summary>
        /// Include ONLY cultures you are implementing
        /// </summary>
        private static readonly List<LocalizationCountry> _cultures = new List<LocalizationCountry> {
            new LocalizationCountry() // first culture is the DEFAULT
            {
                Culture = new CultureInfo("ru"),
                FlagImage = "ru_32_round.png",
                DisplayName = "Русский"
            },
            new LocalizationCountry() // English NEUTRAL culture        
            {
                Culture = new CultureInfo("en"),
                FlagImage = "en_32_round.png",
                DisplayName = "English"
            }
        };

        /// <summary>
        /// Returns a valid culture name based on "name" parameter. If "name" is not valid, it returns the default culture "en-US"
        /// </summary>
        /// <param name="name">Culture's name (e.g. en-US)</param>
        public static string GetImplementedCulture(string name)
        {
            // make sure it's not null
            if (string.IsNullOrEmpty(name))
                return GetDefaultCultureString();
            
            // if we can find it - cool, return found culture
            if (_cultures.Where(c => c.Culture.TwoLetterISOLanguageName.Equals(name, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return name; // accept it
                             
            // Find a close match. For example, if you have "en-US" defined and the user requests "en-GB", 
            // the function will return closes match that is "en-US" because at least the language is the same (i.e. English)  
            string neutralCulture = GetNeutralCulture(name);
            foreach (var c in _cultures)
            {
                if (c.Culture.TwoLetterISOLanguageName.StartsWith(neutralCulture, StringComparison.InvariantCultureIgnoreCase))
                {
                    return c.Culture.TwoLetterISOLanguageName;
                }                    
            }

            // It is not implemented return Default culture as no match found
            return GetDefaultCultureString();
        }

        /// <summary>
        /// Returns default culture name (e.g. ru)
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultCultureString()
        {
            return _defaultCulture; // return Default culture
        }
        public static CultureInfo GetDefaultCulture()
        {
            return new CultureInfo(_defaultCulture); // return Default culture
        }
        public static string GetCurrentCultureString()
        {
            return Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        }
        public static CultureInfo GetCurrentCultureInfo()
        {
            return Thread.CurrentThread.CurrentCulture;
        }
        public static LocalizationCountry GetCurrentCulture()
        {
            var currentCultureNeutralString = GetCurrentNeutralCultureString();
            return _cultures.Where(e => currentCultureNeutralString.Equals(e.Culture.TwoLetterISOLanguageName, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();
        }
        public static string GetNeutralCulture(string name)
        {
            if (IsNeutralCulture(name)) return name;
            return name.Split('-')[0]; // Read first part only. E.g. "en", "es"
        }
        public static string GetCurrentNeutralCultureString()
        {
            return GetNeutralCulture(GetCurrentCultureString());
        }
        public static bool IsNeutralCulture(string name)
        {
            return !name.Contains("-");
        }
        public static IEnumerable<string> GetCulturesStringList()
        {
            return _cultures.Select(e => e.Culture.TwoLetterISOLanguageName);
        }
        public static IEnumerable<CultureInfo> GetCultureInfoList()
        {
            foreach(var c in _cultures)
            {
                yield return c.Culture;
            }
        }
        public static IEnumerable<LocalizationCountry> GetCultureList()
        {
            return _cultures;
        }

        /// <summary>
        /// Returns true if the language is a right-to-left language. Otherwise, false.
        /// </summary>
        public static bool IsRighToLeft()
        {
            return Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft;
        }
    }
}