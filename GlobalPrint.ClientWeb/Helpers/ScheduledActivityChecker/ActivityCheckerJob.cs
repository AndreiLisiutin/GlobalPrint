using GlobalPrint.ClientWeb.Models.PushNotifications;
using GlobalPrint.Configuration.DI;
using GlobalPrint.Infrastructure.EmailUtility;
using GlobalPrint.Infrastructure.LogUtility;
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
using System.Web.Configuration;

namespace GlobalPrint.ClientWeb.Helpers.ScheduledActivityChecker
{
    public class ActivityCheckerJob : IJob
    {
        private readonly TimeSpan _threshold;
        private readonly TimeSpan _callInterval;
        private readonly string _emailSubject = "Activity in GlobalPrint";
        
        private IUserUnit _userUnit;
        private IEmailUtility _emailUtility;
        private Lazy<ILogger> _logUtility;

        public ActivityCheckerJob()
        {
            var loggerFactory = IoC.Instance.Resolve<ILoggerFactory>();
            _logUtility = new Lazy<ILogger>(() => loggerFactory.GetLogger<ActivityCheckerJob>());
            _userUnit = IoC.Instance.Resolve<IUserUnit>();
            _emailUtility = IoC.Instance.Resolve<IEmailUtility>();

            _threshold = TimeSpan.FromMinutes(Int32.Parse(WebConfigurationManager.AppSettings["ActivityCheckerThreshold"]));
            _callInterval = TimeSpan.FromMinutes(Int32.Parse(WebConfigurationManager.AppSettings["ActivityCheckerCallInterval"]));
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
                MailAddress destination = new MailAddress(item.PrinterOperator.Email, item.PrinterOperator.UserName);
                string messageBody = string.Format(
                     "Вы являетесь владельцем активного на данный момент принтера \"{0}\" по адресу {1}." +
                     " Однако вы давно не проявляли активность - последняя Ваша активность зафиксирована {2}." +
                     " Совершите какое-либо действие (обновите страницу сайта globalprint), чтобы обновить дату последней активности.",
                     item.Printer.Name,
                     item.Printer.Location,
                     item.PrinterOperator.LastActivityDate.ToString("dd.MM.yyyy")
                 );

                _emailUtility.Send(destination, _emailSubject, messageBody);

                PushNotificationHub pushNotificationHub = IoC.Instance.Resolve<PushNotificationHub>();
                pushNotificationHub.PrinterOperatorInactivityNotification(messageBody, item.PrinterOperator.ID);
            }
            catch (Exception e)
            {
                _logUtility.Value.Error(e, "Error happens in ActivityCheckerJob.NotifyUser: " + e.Message);
            }
        }
    }

}
