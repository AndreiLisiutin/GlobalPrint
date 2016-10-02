using GlobalPrint.ClientWeb.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace GlobalPrint.ClientWeb.Helpers
{
    public static class ActionLinkHelperExtensions
    {
        /// <summary>
        /// Extension method to handle localized URL using a dedicated, multi-language custom route.
        /// for additional info, read the following post:
        /// http://www.ryadel.com/en/setup-a-multi-language-website-using-asp-net-mvc/
        /// Using: @Html.ActionLink("linkText", "actionName", "Home", null, null, (CultureInfo)null)
        /// </summary>
        [Obsolete("Bad routing", true)]
        public static IHtmlString ActionLink(
            this HtmlHelper helper,
            string linkText,
            string actionName,
            string controllerName,
            object routeValues,
            object htmlAttributes,
            CultureInfo cultureInfo)
        {
            // fallback if cultureInfo is NULL
            if (cultureInfo == null) cultureInfo = CultureInfo.CurrentCulture;

            // arrange a "localized" controllerName to be handled with a dedicated localization-aware route.
            //string localizedControllerName = String.Format("{0}/{1}", cultureInfo.TwoLetterISOLanguageName, controllerName);

            // build the ActionLink
            return helper.ActionLink(
                linkText,
                actionName,
                controllerName,
                routeValues,
                htmlAttributes);
        }

        [Obsolete("Bad routing", true)]
        public static IHtmlString ActionLink(
            this HtmlHelper helper,
            string linkText,
            string actionName,
            string controllerName,
            string protocol,
            string hostName,
            string fragment,
            object routeValues,
            object htmlAttributes,
            CultureInfo cultureInfo)
        {
            // fallback if cultureInfo is NULL
            if (cultureInfo == null) cultureInfo = CultureInfo.CurrentCulture;

            // arrange a "localized" controllerName to be handled with a dedicated localization-aware route.
            //string localizedControllerName = String.Format("{0}/{1}", cultureInfo.TwoLetterISOLanguageName, controllerName);

            // build the ActionLink
            return helper.ActionLink(
                linkText,
                actionName,
                controllerName,
                protocol,
                hostName,
                fragment,
                routeValues,
                htmlAttributes);
        }
    }
}