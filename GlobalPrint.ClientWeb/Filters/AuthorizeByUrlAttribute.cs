using GlobalPrint.Infrastructure.CommonUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace GlobalPrint.ClientWeb.Filters
{
    /// <summary> 
    /// Only allows authorized URL addresses access. 
    /// </summary> 
    public class AuthorizeByUrlAttribute : ActionFilterAttribute
    {
        public AuthorizeByUrlAttribute(string[] authorizedHosts)
        {
            this.AuthorizedHosts = authorizedHosts ?? new string[] { };
        }
        public string[] AuthorizedHosts { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string host = HttpContext.Current?.Request?.UrlReferrer?.Host;
            string referer = HttpContext.Current?.Request?.UrlReferrer?.ToString();
            string request = JsonConvert.SerializeObject(HttpContext.Current.Request, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Error = delegate(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
                {
                    args.ErrorContext.Handled = true;
                }
            });
            throw new Exception(request);
            
            Argument.NotNullOrWhiteSpace(host, "Host текущей сессии пустой.");
            Argument.NotNullOrWhiteSpace(host, "UrlReferrer текущей сессии пустой.");
            Argument.NotNull(this.AuthorizedHosts, "authorizedHosts пустой.");

            if (!this.AuthorizedHosts.Contains(host.Trim()) && !this.AuthorizedHosts.Contains(referer.Trim()))
            {
                //Send back a HTTP Status code of 403 Forbidden
                filterContext.Result = new HttpStatusCodeResult(403);
            }

            base.OnActionExecuting(filterContext);

        }
    }
}