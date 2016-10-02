using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb.Filters
{
    /// <summary> 
    /// Only allows authorized URL addresses access. 
    /// </summary> 
    public class AuthorizeByUrlAttribute : ActionFilterAttribute
    {
        public AuthorizeByUrlAttribute(string[] authorizedHosts)
        {
            this.AuthorizedHosts = authorizedHosts ?? new string[] {};
        }
        public string[] AuthorizedHosts { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string host = HttpContext.Current.Request.UrlReferrer.Host;
            string referer = HttpContext.Current.Request.UrlReferrer.ToString();

            if (!this.AuthorizedHosts.Contains(host.Trim()) && !this.AuthorizedHosts.Contains(referer.Trim()))
            {
                //Send back a HTTP Status code of 403 Forbidden
                filterContext.Result = new HttpStatusCodeResult(403);
            }

            base.OnActionExecuting(filterContext);
        }
    }
}