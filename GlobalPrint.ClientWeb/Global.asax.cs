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
        private static ILogger _logUtility { get; set; }
                
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ModelBinders.Binders.Add(typeof(float?), new FloatModelBinder());
            ModelBinders.Binders.Add(typeof(float), new FloatModelBinder());

            IKernel kernel = new StandardKernel();
            IoC.Instance.Initialize(kernel);
            ClientWeb.DI.ClientWebIoC.InitializeIoC(IoC.Instance);
            IoC.Instance.RegisterInfrastructure();
            IoC.Instance.RegisterServerBusinessLogic();
            IoC.Instance.RegisterServerDataAccess();
            ClientWeb.DI.ClientWebIoC.InitializeSignalRResolver(kernel);

            ILoggerFactory loggerFactory = IoC.Instance.Resolve<ILoggerFactory>();
            _logUtility = loggerFactory.GetLogger<MvcApplication>();

            _logUtility.Info("Application Start");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            _logUtility.Error(exception, exception.Message);
        }
    }
}
