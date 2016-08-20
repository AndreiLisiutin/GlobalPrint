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
    /// <summary>
    /// Log utility of NLog
    /// </summary>
    public class NlogUtility<T> : NLog.Logger, ILogUtility
    {
        public NlogUtility()
            :base()
        {
            //this.SetConfig();
        }

        private void SetConfig()
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
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Warn, errorFileTarget));

            // Logging all into file
            FileTarget fullFileTarget = new FileTarget();
            fullFileTarget.Name = "fullFileTarget";
            fullFileTarget.FileName = System.IO.Path.Combine(baseLogPath, "log.txt");
            fullFileTarget.Layout = errorInfoString;
            fullFileTarget.CreateDirs = true;
            fullFileTarget.KeepFileOpen = false;

            config.AddTarget(fullFileTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, fullFileTarget));

            // Logging into email
            MailTarget mailTarget = new MailTarget();
            mailTarget.Name = "mailTarget";
            mailTarget.EnableSsl = true;
            mailTarget.SmtpServer = "smtp.gmail.com";
            mailTarget.SmtpPort = 587;
            mailTarget.SmtpAuthentication = SmtpAuthenticationMode.Basic;
            mailTarget.SmtpUserName = "sup.globalprint.online@gmail.com";
            mailTarget.SmtpPassword = "global2016print-";
            mailTarget.UseSystemNetMailSettings = false;
            mailTarget.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            mailTarget.From = "sup.globalprint.online@gmail.com";
            mailTarget.To = "sup.globalprint.online@gmail.com";
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

            //NLog.LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration("<your path>" + "\\NLog.config", false);
            LogManager.Configuration = config;
            LogManager.ThrowExceptions = true;
        }
    }
}