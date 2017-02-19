using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.EmailUtility
{
    /// <summary>
    /// Интерфейс утилит для отправки email сообщений.
    /// </summary>
    public interface IEmailUtility
    {
        /// <summary>
        /// Отправить email сообщение.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        /// <param name="sender">Отправитель. Опциональное поле, по умолчанию пусто.</param>
        /// <param name="throwException">Бросать ли ошибку в случае неудачной отправки сообшения. Опциональное поле, по умолчанию нет.</param>
        void Send(EmailMessage message, MailAddress sender = null, bool throwException = false);

        /// <summary>
        /// Отправить email сообщение.
        /// </summary>
        /// <param name="destination">Email адрес получателя.</param>
        /// <param name="subject">Тема сообщения.</param>
        /// <param name="body">Сообщение.</param>
        /// <param name="sender">Отправитель. Опциональное поле, по умолчанию пусто.</param>
        /// <param name="throwException">Бросать ли ошибку в случае неудачной отправки сообшения. Опциональное поле, по умолчанию нет.</param>
        void Send(MailAddress destination, string subject, string body, MailAddress sender = null, bool throwException = false);

        /// <summary>
        /// Асинхронно отправить email сообщение.
        /// </summary>
        /// <param name="destination">Email адрес получателя.</param>
        /// <param name="subject">Тема сообщения.</param>
        /// <param name="body">Сообщение.</param>
        /// <param name="sender">Отправитель. Опциональное поле, по умолчанию пусто.</param>
        /// <param name="throwException">Бросать ли ошибку в случае неудачной отправки сообшения. Опциональное поле, по умолчанию нет.</param>
        /// <returns></returns>
        Task SendAsync(MailAddress destination, string subject, string body, MailAddress sender = null, bool throwException = false);

        /// <summary>
        /// Email адрес саппорта globalprint (support@globalprint.online)
        /// </summary>
        MailAddress SupportEmail { get; }

        /// <summary>
        /// Email адрес технической поддержки, разработчиков (sup.globalprint.online@gmail.com)
        /// </summary>
        MailAddress DevelopersEmail { get; }
    }
}
