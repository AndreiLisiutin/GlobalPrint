using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.EmailUtility
{
    public interface IEmailUtility
    {
        void Send(EmailMessage message, MailAddress sender = null, bool throwException = false);
        void Send(MailAddress destination, string subject, string body, MailAddress sender = null, bool throwException = false);
        Task SendAsync(MailAddress destination, string subject, string body, MailAddress sender = null, bool throwException = false);
        MailAddress SupportEmail { get; }
        MailAddress DevelopersEmail { get; }
    }
}
