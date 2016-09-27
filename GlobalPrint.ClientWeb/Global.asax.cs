using GlobalPrint.ClientWeb.App_Start;
using GlobalPrint.Configuration.DI;
using GlobalPrint.Infrastructure.LogUtility;
using Microsoft.AspNet.SignalR;
using Ninject;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace GlobalPrint.ClientWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private ILogger _logUtility { get; set; }

        public MvcApplication()
            :base()
        {
            ILoggerFactory loggerFactory = IoC.Instance.Resolve<ILoggerFactory>();
            _logUtility = loggerFactory.GetLogger<MvcApplication>();
        }
                        
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            ModelBinders.Binders.Add(typeof(float?), new FloatModelBinder());
            ModelBinders.Binders.Add(typeof(float), new FloatModelBinder());
            _logUtility.Info("Application Start");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            _logUtility.Error(exception, exception.Message);
        }
    }
}
