using GlobalPrint.Infrastructure.LogUtility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace GlobalPrint.Infrastructure.EmailUtility
{
    public class EmailUtility : IEmailUtility
    {
        private Lazy<ILogger> _logUtility { get; set; }

        public EmailUtility(ILoggerFactory loggerFactory)
        {
            this._logUtility = new Lazy<ILogger>(() => loggerFactory.GetLogger<EmailUtility>());
        }

        private readonly MailAddress _supportEmail = new MailAddress(WebConfigurationManager.AppSettings["SupportEmail"].ToString(), WebConfigurationManager.AppSettings["SupportEmailDisplayName"].ToString());
        public MailAddress SupportEmail
        {
            get
            {
                return this._supportEmail;
            }
        }

        /// <summary>
        /// Get MailMessage object from parameters
        /// </summary>
        private MailMessage GetMailNessage(MailAddress destination, string subject, string body, MailAddress sender = null)
        {
            // mail sender from Web.config/system.net/mailSettings/from
            MailMessage mail = new MailMessage();

            mail.To.Add(destination);
            mail.Subject = subject;
            mail.IsBodyHtml = true;
            // replace all Environment.NewLine with <br/>
            mail.Body = Regex.Replace(body, @"\r\n?|\n", "<br />");
            if (sender != null)
            {
                mail.Sender = sender;
            }

            return mail;
        }

        /// <summary>
        /// Send email syncroniously
        /// </summary>
        /// <param name="destination">Mail destination (To)</param>
        /// <param name="subject">Mail subject/theme</param>
        /// <param name="body">Mail body/text</param>
        public void Send(MailAddress destination, string subject, string body, MailAddress sender = null, bool throwException = false)
        {
            try
            {
                // SMTP settings from Web.config/system.net/mailSettings
                SmtpClient client = new SmtpClient();

                MailMessage mail = GetMailNessage(destination, subject, body, sender);

                client.Send(mail);
            }
            catch (Exception ex)
            {
                this._logUtility.Value.Error(ex, "Ошибка отправки email сообщения: " + ex.Message);
                if (throwException)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Send email asyncroniously
        /// </summary>
        /// <param name="destination">Mail destination (To)</param>
        /// <param name="subject">Mail subject/theme</param>
        /// <param name="body">Mail body/text</param>
        public Task SendAsync(MailAddress destination, string subject, string body, MailAddress sender = null, bool throwException = false)
        {
            try
            {
                // SMTP settings from Web.config/system.net/mailSettings
                SmtpClient client = new SmtpClient();

                MailMessage mail = GetMailNessage(destination, subject, body, sender);

                client.SendCompleted += MailDeliveryComplete;
                return client.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                this._logUtility.Value.Error(ex, "Ошибка отправки email сообщения: " + ex.Message);
                if (throwException)
                {
                    throw;
                }
                else
                {
                    return Task.FromResult(0);
                }
            }
        }

        /// <summary>
        /// Callback of async mail sending
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MailDeliveryComplete(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // handle error
                this._logUtility.Value.Error(e.Error, "Ошибка отправки email сообщения: " + e.Error.Message);
            }
            else if (e.Cancelled)
            {
                // handle cancelled
                this._logUtility.Value.Warn(e.Error, "Отправка сообщения была отменена: " + e.Error.Message);
            }
            else
            {
                //handle sent email
                //MailMessage message = (MailMessage)e.UserState;
            }
        }
    }
}
