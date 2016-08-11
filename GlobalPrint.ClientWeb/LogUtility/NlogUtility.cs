using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace GlobalPrint.ClientWeb
{
    public class NlogUtility : ILogUtility
    {
        public Logger LoggerInstace { get; private set; }
        public NlogUtility()
        {
            this.LoggerInstace = LogManager.GetCurrentClassLogger();
            this.SetConfig();
        }

        public void Debug(string message)
        {
            LoggerInstace.Debug(message);
        }

        public void Debug(Exception ex, string message, params object[] args)
        {
            LoggerInstace.Debug(ex, message, args);
        }

        public void Error(string message)
        {
            LoggerInstace.Error(message);
        }

        public void Error(Exception ex, string message, params object[] args)
        {
            LoggerInstace.Error(ex, message, args);
        }

        public void Fatal(string message)
        {
            LoggerInstace.Fatal(message);
        }

        public void Fatal(Exception ex, string message, params object[] args)
        {
            LoggerInstace.Fatal(ex, message, args);
        }

        public void Info(string message)
        {
            LoggerInstace.Info(message);
        }

        public void Info(Exception ex, string message, params object[] args)
        {
            LoggerInstace.Info(ex, message, args);
        }

        public void Trace(string message)
        {
            LoggerInstace.Trace(message);
        }

        public void Trace(Exception ex, string message, params object[] args)
        {
            LoggerInstace.Trace(ex, message, args);
        }

        public void Warn(string message)
        {
            LoggerInstace.Warn(message);
        }

        public void Warn(Exception ex, string message, params object[] args)
        {
            LoggerInstace.Warn(ex, message, args);
        }

        private void SetConfig()
        {
            string baseLogPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Log");
            string dateInfo = "Date: ${date:format=dd\\.MM\\.yyyy HH\\:mm\\:ss}";
            string locationInfo = "Location: ${callsite}";
            string messageInfo = "Message: ${date:format=dd\\.MM\\.yyyy HH\\:mm\\:ss}";
            string detailsInfo = "Details: ${exception:format=ToString,StackTrace}";

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

            LogManager.Configuration = config;
            LogManager.ThrowExceptions = true;
        }
    }
}