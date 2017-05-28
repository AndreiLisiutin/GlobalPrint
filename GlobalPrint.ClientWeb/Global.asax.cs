using GlobalPrint.ClientWeb.App_Start;
using GlobalPrint.ClientWeb.Binders;
using GlobalPrint.ClientWeb.Helpers.ScheduledActivityChecker;
using GlobalPrint.Configuration.DI;
using GlobalPrint.Infrastructure.LogUtility;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNet.SignalR;
using Ninject;
using System;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Routing;

namespace GlobalPrint.ClientWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private ILogger _logger { get; set; }

        public MvcApplication()
            : base()
        {
            ILoggerFactory loggerFactory = IoC.Instance.Resolve<ILoggerFactory>();
            _logger = loggerFactory.GetLogger<MvcApplication>();
        }

        protected void Application_Start()
        {
            DisableApplicationInsightsOnDebug();
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            ModelBinders.Binders.Add(typeof(float?), new FloatModelBinder());
            ModelBinders.Binders.Add(typeof(float), new FloatModelBinder());
            ModelBinders.Binders.Add(typeof(decimal), new FloatModelBinder());
            ModelBinders.Binders.Add(typeof(decimal?), new FloatModelBinder());

            // Set controller factory implementing multilanguage stuff
            ControllerBuilder.Current.SetControllerFactory(new DefaultControllerFactory(new LocalizedControllerActivator()));

            // run web service to check inactive users with - printer operators and send notifications to them
            new ActivityCheckerJobScheduler().Start();

            _logger.Info("Application Start");
        }


        /// <summary>
        /// Disables the application insights locally.
        /// </summary>
        [Conditional("DEBUG")]
        private static void DisableApplicationInsightsOnDebug()
        {
            TelemetryConfiguration.Active.DisableTelemetry = true;
        }

        /// <summary>
        /// При возникновении любого необработанного исключения писать сообщение в лог.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            _logger.Error(exception, exception.Message);
        }
    }
}
