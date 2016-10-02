using GlobalPrint.ClientWeb.Helpers;
using GlobalPrint.Infrastructure.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GlobalPrint.ClientWeb.App_Start
{
    public class LocalizedControllerActivator : IControllerActivator
    {
        public IController Create(RequestContext requestContext, Type controllerType)
        {
            // Get the {lang} parameter in the RouteData
            string lang = requestContext.RouteData.Values["lang"] as string;

            // Validate culture name, default if not valid
            lang = LocalizationHelper.GetImplementedCulture(lang);

            Thread.CurrentThread.CurrentCulture =
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);

            return DependencyResolver.Current.GetService(controllerType) as IController;
        }
    }
}