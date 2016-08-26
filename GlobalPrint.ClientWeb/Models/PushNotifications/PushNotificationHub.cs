using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobalPrint.ClientWeb.Models.PushNotifications
{
    [Authorize]
    public class PushNotificationHub : Hub
    {
        public void NotifyUserByID(string message, int userID)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<PushNotificationHub>();
            context.Clients.User(userID.ToString()).displayMessage(message);
        }
    }
}