using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace GlobalPrint.Infrastructure.LogUtility
{
    public abstract class AbstractNLogFactory<V> : ILogFactory
        where V : NLog.Logger, ILogUtility
    {
        private NLog.LogFactory _logFactory { get; set; }

        public AbstractNLogFactory()
        {
            _logFactory = this.Config == null ? new NLog.LogFactory() : new NLog.LogFactory(this.Config);
            _logFactory.ThrowExceptions = this.ThrowExceptions;
        }

        protected abstract LoggingConfiguration Config { get; }
        protected virtual bool ThrowExceptions { get { return true; } }

        public ILogUtility GetLogUtility<T>()
        {
            return _logFactory.GetLogger<V>(typeof(T).Name);
        }

        public ILogUtility GetCurrentClassLogUtility()
        {
            return _logFactory.GetCurrentClassLogger<V>();
        }
    }

    public class NLogUtil : Logger, ILogUtility
    {
    }

    public class NLogFactory : AbstractNLogFactory<NLogUtil>
    {
        public NLogFactory() 
            : base()
        {
        }

        protected override LoggingConfiguration Config
        {
            get
            {
                string baseLogPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Log");
                string dateInfo = "Date: ${date:format=dd\\.MM\\.yyyy HH\\:mm\\:ss}";
                string locationInfo = "Location: ${callsite}";
                string messageInfo = "Message: ${date:format=dd\\.MM\\.yyyy HH\\:mm\\:ss}";
                string detailsInfo = "Details: ${exception:format=ToString,StackTrace:topFrames=10}";

                LoggingConfiguration config = new LoggingConfiguration();

                // Logging errors into file
                FileTarget errorFileTarget = new FileTarget();
                errorFileTarget.Name = "errorFileTarget";
                errorFileTarget.FileName = System.IO.Path.Combine(baseLogPath, "errors.txt");
                errorFileTarget.CreateDirs = true;
                errorFileTarget.KeepFileOpen = false;
                string errorInfoString = new StringBuilder("========================================= ${level:uppercase=true} =========================================")
                   .AppendLine()
                   .AppendLine(dateInfo)
                   .AppendLine(locationInfo)
                   .AppendLine(messageInfo)
                   .AppendLine(detailsInfo)
                   .AppendLine()
                   .ToString();
                errorFileTarget.Layout = errorInfoString;

                config.AddTarget(errorFileTarget);
                config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, errorFileTarget));

                // Logging into email
                MailTarget mailTarget = new MailTarget();
                mailTarget.Name = "mailTarget";
                mailTarget.Encoding = System.Text.Encoding.UTF8;
                mailTarget.EnableSsl = true;
                mailTarget.SmtpServer = "smtp.gmail.com";
                mailTarget.SmtpPort = 587;
                mailTarget.SmtpAuthentication = SmtpAuthenticationMode.Basic;
                mailTarget.SmtpUserName = "sergei.lisiutin@gmail.com";
                mailTarget.SmtpPassword = "littlelion9310-";
                mailTarget.UseSystemNetMailSettings = false;
                mailTarget.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                mailTarget.From = "sergei.lisiutin@gmail.com";
                mailTarget.To = "sergei.lisiutin@gmail.com";
                mailTarget.Subject = "Global Print Error (${level:uppercase=true}) [${date:format=dd\\.MM\\.yyyy HH\\:mm}]";
                mailTarget.Layout = "Oops... Some error occured during Global Print Web run..." +
                    Environment.NewLine + errorInfoString;
                mailTarget.Body = mailTarget.Layout;
                mailTarget.Footer = new StringBuilder("--------------------------------------------------------------------------")
                    .AppendLine()
                    .AppendLine("Best regards,")
                    .AppendLine("Your Global Print")
                    .AppendLine("Website: https://globalprint.online/")
                    .AppendLine("Mail: globalprint.online@gmail.com")
                    .ToString();

                //config.AddTarget(mailTarget);
                //config.LoggingRules.Add(new LoggingRule("*", LogLevel.Error, mailTarget));

                return config;
            }
        }
    }
}