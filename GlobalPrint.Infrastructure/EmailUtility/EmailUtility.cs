using GlobalPrint.Infrastructure.LogUtility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.EmailUtility
{
    public class EmailUtility : IEmailUtility
    {
        private Lazy<ILogUtility> _logUtility = new Lazy<ILogUtility>(() => new NlogUtility<EmailUtility>());

        public void Send(string destination, string subject, string body)
        {
            try
            {
                // настройка логина, пароля отправителя
                var from = "sergei.lisiutin@gmail.com";
                var pass = "littlelion9310-";

                // адрес и порт smtp-сервера, с которого мы и будем отправлять письмо
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);

                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(from, pass);
                client.EnableSsl = true;

                // создаем письмо: message.Destination - адрес получателя
                MailMessage mail = new MailMessage(from, destination);
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

        public Task SendAsync(string destination, string subject, string body)
        {
           try
            {
                // настройка логина, пароля отправителя
                var from = "sergei.lisiutin@gmail.com";
                var pass = "littlelion9310-";

                // адрес и порт smtp-сервера, с которого мы и будем отправлять письмо
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);

                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(from, pass);
                client.EnableSsl = true;

                // создаем письмо: message.Destination - адрес получателя
                MailMessage mail = new MailMessage(from, destination);
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
