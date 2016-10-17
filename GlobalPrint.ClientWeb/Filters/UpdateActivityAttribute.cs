using GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Users;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb.Filters
{
    /// <summary> 
    /// Updates current user activity date.
    /// </summary> 
    public class UpdateActivityAttribute : ActionFilterAttribute
    {
        public IUserUnit _userUnit;

        public UpdateActivityAttribute(IUserUnit userUnit)
        {
            _userUnit = userUnit;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                int currentUserID = HttpContext.Current.User.Identity.GetUserId<int>();
                _userUnit.UpdateUserActivity(currentUserID);
            }

            base.OnActionExecuting(filterContext);
        }
    }
}