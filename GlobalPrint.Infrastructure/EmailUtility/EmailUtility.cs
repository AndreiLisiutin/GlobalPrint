using GlobalPrint.Infrastructure.LogUtility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.EmailUtility
{
    public class EmailUtility : IEmailUtility
    {
        private Lazy<ILogUtility> _logUtility = new Lazy<ILogUtility>(() => new NlogUtility<EmailUtility>());

        /// <summary>
        /// Send email syncroniously
        /// </summary>
        /// <param name="destination">Mail destination (To)</param>
        /// <param name="subject">Mail subject/theme</param>
        /// <param name="body">Mail body/text</param>
        public void Send(string destination, string subject, string body)
        {
            try
            {
                // SMTP settings from Web.config/system.net/mailSettings
                SmtpClient client = new SmtpClient();

                // mail sender from Web.config/system.net/mailSettings/from
                MailMessage mail = new MailMessage();
                mail.To.Add(destination);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                client.Send(mail);
            }
            catch(Exception ex)
            {
                this._logUtility.Value.Error(ex, "Ошибка отправки email сообщения: " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Send email asyncroniously
        /// </summary>
        /// <param name="destination">Mail destination (To)</param>
        /// <param name="subject">Mail subject/theme</param>
        /// <param name="body">Mail body/text</param>
        public Task SendAsync(string destination, string subject, string body)
        {
           try
            {
                // SMTP settings from Web.config/system.net/mailSettings
                SmtpClient client = new SmtpClient();

                // mail sender from Web.config/system.net/mailSettings/from
                MailMessage mail = new MailMessage();
                mail.To.Add(destination);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                client.SendCompleted += MailDeliveryComplete;
                return client.SendMailAsync(mail);
            }
            catch(Exception ex)
            {
                this._logUtility.Value.Error(ex, "Ошибка отправки email сообщения: " + ex.Message);
                throw;
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
