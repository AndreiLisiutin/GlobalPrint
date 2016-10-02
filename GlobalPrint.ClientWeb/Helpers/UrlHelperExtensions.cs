using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb.Helpers
{
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// Extension method to handle localized URL using a dedicated, multi-language custom route.
        /// for additional info, read the following post:
        /// http://www.ryadel.com/en/setup-a-multi-language-website-using-asp-net-mvc/
        /// Using: @Url.Action("actionName", "Home", null, (CultureInfo)null)
        /// </summary>
        [Obsolete("Bad routing", true)]
        public static string Action(
            this UrlHelper helper,
            string actionName,
            string controllerName,
            object routeValues,
            CultureInfo cultureInfo)
        {
            // fallback if cultureInfo is NULL
            if (cultureInfo == null) cultureInfo = CultureInfo.CurrentCulture;

            // arrange a "localized" controllerName to be handled with a dedicated localization-aware route.
            string localizedControllerName = String.Format("{0}/{1}", cultureInfo.TwoLetterISOLanguageName, controllerName);

            // build the Action
            return helper.Action(
                actionName,
                controllerName,
                routeValues);
        }

        [Obsolete("Bad routing", true)]
        public static string Action(
           this UrlHelper helper,
           string actionName, 
           string controllerName, 
           object routeValues, 
           string protocol,
           CultureInfo cultureInfo)
        {
            // fallback if cultureInfo is NULL
            if (cultureInfo == null) cultureInfo = CultureInfo.CurrentCulture;

            // arrange a "localized" controllerName to be handled with a dedicated localization-aware route.
            string localizedControllerName = String.Format("{0}/{1}", cultureInfo.TwoLetterISOLanguageName, controllerName);

            // build the ActionLink
            return helper.Action(
                actionName,
                controllerName,
                routeValues,
                protocol);
        }
    }
}