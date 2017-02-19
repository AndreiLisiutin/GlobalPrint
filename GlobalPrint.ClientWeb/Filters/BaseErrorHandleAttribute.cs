using GlobalPrint.Configuration.DI;
using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.Infrastructure.LogUtility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GlobalPrint.ClientWeb.Filters
{
    public class BaseErrorHandleAttribute : HandleErrorAttribute
    {
        private ILogger _logUtility { get; set; }

        public BaseErrorHandleAttribute()
            : base()
        {
            ILoggerFactory loggerFactory = IoC.Instance.Resolve<ILoggerFactory>();
            _logUtility = loggerFactory.GetLogger<MvcApplication>();

            this.View = "Errors/Default";
        }

        public override void OnException(ExceptionContext filterContext)
        {
            Argument.NotNull(filterContext, "BaseErrorHandleAttribute filterContext is null.");
            Argument.NotNull(filterContext.HttpContext, "BaseErrorHandleAttribute filterContext.HttpContext is null.");
            
            if (!this.ExceptionType.IsInstanceOfType(filterContext.Exception) || filterContext.ExceptionHandled)
            {
                return;
            }
            this._logUtility.Error(filterContext.Exception, filterContext.Exception.Message);

            // If it's anti-forgery exception, go to login page
            var exception = filterContext.Exception as HttpAntiForgeryException;
            if (exception != null)
            {
                var routeValues = new RouteValueDictionary();
                routeValues["controller"] = "Account";
                routeValues["action"] = "Login";
                filterContext.Result = new RedirectToRouteResult(routeValues);
                filterContext.ExceptionHandled = true;
                return;
            }

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                base.OnException(filterContext);
                return;
            }

            string controllerName = (string)filterContext.RouteData.Values["controller"];
            string actionName = (string)filterContext.RouteData.Values["action"];
            HandleErrorInfo model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = filterContext.Exception is HttpException
                ? ((HttpException)filterContext.Exception).GetHttpCode()
                : new HttpException(null, filterContext.Exception).GetHttpCode();

            this.View = this.ErrorPageView(filterContext.Exception, filterContext.HttpContext.Response.StatusCode);
            filterContext.Result = new ViewResult
            {
                ViewName = View,
                MasterName = Master,
                ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                TempData = filterContext.Controller.TempData
            };

            // Certain versions of IIS will sometimes use their own error page when
            // they detect a server error. Setting this property indicates that we
            // want it to try to render ASP.NET MVC's error page instead.
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }

        /// <summary>
        /// Get error page by status code.
        /// </summary>
        /// <param name="statusCode">Status code.</param>
        /// <returns>View page name.</returns>
        public string ErrorPageView(Exception exception, int statusCode)
        {
            if (exception != null && exception is FileNotFoundException)
            {
                return $"Errors/FileNotFound";
            }
            switch (statusCode)
            {
                case 404:
                    return $"Errors/{statusCode}";
                default:
                    return this.View;
            }
        }
    }
}