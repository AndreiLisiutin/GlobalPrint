using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.EmailUtility
{
    /// <summary>
    /// Class which contains data for email message sending.
    /// </summary>
    public class EmailMessage
    {
        /// <summary>
        /// Email message constructor. 
        /// </summary>
        /// <param name="emailAddress">Email address. Example: test@test.test.</param>
        /// <param name="userName">User name. Example: Ivan Ivanov.</param>
        /// <param name="subject">Subject.</param>
        /// <param name="body">Email text or html.</param>
        public EmailMessage(string emailAddress, string userName, string subject, string body)
        {
            this.Destination = new MailAddress(emailAddress, userName);
            this.Subject = subject;
            this.Body = body;
        }

        /// <summary>
        /// Email to.
        /// </summary>
        public MailAddress Destination { get; set; }
        /// <summary>
        /// Subject.
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// Email text or html.
        /// </summary>
        public string Body { get; set; }
    }
}
