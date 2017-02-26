using GlobalPrint.ClientWeb.Models.PushNotifications;
using GlobalPrint.Configuration.DI;
using GlobalPrint.Infrastructure.EmailUtility;
using GlobalPrint.Infrastructure.LogUtility;
using GlobalPrint.Infrastructure.Notifications;
using GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Users;
using GlobalPrint.ServerBusinessLogic.Models.Business.Printers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using Ninject;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb.Helpers.ScheduledActivityChecker
{
    public class ActivityCheckerJob : IJob
    {
        private readonly TimeSpan _threshold;
        private readonly TimeSpan _callInterval;
        private readonly string _emailSubject = "Активность в GlobalPrint";
        private readonly string _globalPrintSiteUrl;

        private IUserUnit _userUnit;
        private IEmailUtility _emailUtility;
        private Lazy<ILogger> _logUtility;

        public ActivityCheckerJob()
        {
            var loggerFactory = IoC.Instance.Resolve<ILoggerFactory>();
            _logUtility = new Lazy<ILogger>(() => loggerFactory.GetLogger<ActivityCheckerJob>());
            _userUnit = IoC.Instance.Resolve<IUserUnit>();
            _emailUtility = IoC.Instance.Resolve<IEmailUtility>();

            _threshold = TimeSpan.FromMinutes(double.Parse(WebConfigurationManager.AppSettings["ActivityCheckerThreshold"]));
            _callInterval = TimeSpan.FromMinutes(double.Parse(WebConfigurationManager.AppSettings["ActivityCheckerCallInterval"]));
            _globalPrintSiteUrl = WebConfigurationManager.AppSettings["GlobalPrintSiteUrl"].ToString();
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var inactiveUserList = _userUnit.GetInactiveUsers(_threshold, _callInterval);
                if (inactiveUserList != null)
                {
                    foreach (var user in inactiveUserList)
                    {
                        NotifyUser(user);
                    }
                }
            }
            catch (Exception e)
            {
                _logUtility.Value.Error(e, "Error happens in ActivityCheckerJob.Execute: " + e.Message);
            }
        }

        /// <summary>
        /// Notify user about his inactivity.
        /// </summary>
        /// <param name="item">User to notify and his printer.</param>
        private void NotifyUser(PrinterOperatorModel item)
        {
            try
            {
                MailAddress printerOperatorMail = new MailAddress(item.PrinterOperator.Email, item.PrinterOperator.UserName);
                string messageBody = string.Format(
                     "Вы являетесь оператором принтера \"{0}\" по адресу {1}." +
                     " В {2} ваш принтер стал неактивным на карте и клиенты больше не могут отправлять вам заказы." +
                     " Совершите какое-либо действие в системе (обновите страницу сайта www.globalprint.online)," +
                     " чтобы продлить период доступности для клиентов вашего принтера на {3} минут.",
                     item.Printer.Name,
                     item.Printer.Location,
                     DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"),
                     _threshold.TotalMinutes
                 );

                _emailUtility.Send(printerOperatorMail, _emailSubject, messageBody);

                // Simple push notification
                PushNotificationHub pushNotificationHub = IoC.Instance.Resolve<PushNotificationHub>();
                pushNotificationHub.UserActivityNotification(messageBody, item.PrinterOperator.ID);

                // Browser push notification
                if (!string.IsNullOrWhiteSpace(item.PrinterOperator.DeviceID))
                {
                    NotificationMessage message = new NotificationMessage()
                    {
                        Body = messageBody,
                        Title = _emailSubject,
                        Action = _globalPrintSiteUrl,
                        DestinationUserID = item.PrinterOperator.ID
                    };
                    FirebaseNotificator firebaseNotification = new FirebaseNotificator();
                    firebaseNotification.SendNotification(message);
                }
            }
            catch (Exception e)
            {
                _logUtility.Value.Error(e, "Error happens in ActivityCheckerJob.NotifyUser: " + e.Message);
            }
        }
    }

}
