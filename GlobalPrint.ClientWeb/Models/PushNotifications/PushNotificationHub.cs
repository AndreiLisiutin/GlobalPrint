using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Printers;
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

        public void NewIncomingOrder(string message, int clientUserID)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<PushNotificationHub>();
            var userToNotify = context.Clients.User(clientUserID.ToString());

            // Number of waiting orders, not processed by current user
            int printOrdersCount = new PrinterUnit().GetWaitingIncomingOrdersCount(clientUserID);
            
            // notify by message
            userToNotify.displayMessage(message);
            // update orders count badge
            userToNotify.updateIncomingOrdersCount(printOrdersCount);
        }
    }
}