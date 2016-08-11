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
        protected ILogUtility logUtility { get; set; }

        public MvcApplication(/*ILogUtility logUtility*/) 
            : base()
        {
            this.logUtility = new NlogUtility();
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ModelBinders.Binders.Add(typeof(float?), new FloatModelBinder());
            ModelBinders.Binders.Add(typeof(float), new FloatModelBinder());
            //this.logUtility.Info("Application Start");
            //this.logUtility.Error("Error message");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            this.logUtility.Error(exception, exception.Message);
        }
    }
}
