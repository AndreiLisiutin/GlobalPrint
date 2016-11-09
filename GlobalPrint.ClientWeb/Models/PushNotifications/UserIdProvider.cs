using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Users;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobalPrint.ClientWeb.Models.PushNotifications
{
    public class UserIdProvider : IUserIdProvider
    {
        private readonly UserUnit _unit;

        public UserIdProvider(UserUnit unit)
        {
            this._unit = unit;
        }

        public string GetUserId(IRequest request)
        {
            if (request.User.Identity.IsAuthenticated)
            {
                return request.User.Identity.GetUserId();
            }

            return "0";
        }
    }
}