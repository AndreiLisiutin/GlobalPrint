using GlobalPrint.Configuration.DI;
using GlobalPrint.Infrastructure.LogUtility;
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
        private Lazy<ILogger> _logUtility;

        public PushNotificationHub(ILoggerFactory loggerFactory)
            : base()
        {
            _logUtility = new Lazy<ILogger>(() => loggerFactory.GetLogger<PushNotificationHub>());
        }

        public void NotifyUserByID(string message, int userID)
        {
            try
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<PushNotificationHub>();
                context.Clients.User(userID.ToString()).displayMessage(message);
            }
            catch (Exception ex)
            {
                _logUtility.Value.Error(ex, "Exception occurs in PushNotificationHub.NotifyUserByID: " + ex.Message);
            }
        }

        public void NewIncomingOrder(string message, int clientUserID)
        {
            try
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<PushNotificationHub>();
                var userToNotify = context.Clients.User(clientUserID.ToString());

                // Number of waiting orders, not processed by current user
                int printOrdersCount = new PrinterUnit().GetWaitingIncomingOrdersCount(clientUserID);

                // notify by message
                userToNotify.displayMessage(message, "/UserRecievedPrintOrderList/UserRecievedPrintOrderList");
                // update orders count badge
                userToNotify.updateIncomingOrdersCount(printOrdersCount);
            }
            catch (Exception ex)
            {
                _logUtility.Value.Error(ex, "Exception occurs in PushNotificationHub.NewIncomingOrder: " + ex.Message);
            }
        }
    }
}