using GlobalPrint.Configuration.DI;
using GlobalPrint.Infrastructure.LogUtility;
using Ninject;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GlobalPrint.ClientWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        //private Lazy<ILogUtility> _logUtility = new Lazy<ILogUtility>(() => new NlogUtility<MvcApplication>());
        private Logger logger = LogManager.GetCurrentClassLogger();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ModelBinders.Binders.Add(typeof(float?), new FloatModelBinder());
            ModelBinders.Binders.Add(typeof(float), new FloatModelBinder());

            IoC.Instance.Initialize(new StandardKernel());
            IoC.Instance.RegisterServerBusinessLogic();
            IoC.Instance.RegisterServerDataAccess();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            //logger.Error(exception, exception.Message);
        }
    }
}
