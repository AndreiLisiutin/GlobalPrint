using GlobalPrint.Infrastructure.EmailUtility;
using Moq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace GlobalPrint.Test
{
    /// <summary>
    /// Parent for all test classes
    /// </summary>
    public abstract class BaseTest
    {
        /// <summary>
        /// User name for identity config.
        /// </summary>
        protected string CurrentUserName = ConfigurationManager.AppSettings["TestUserName"];

        /// <summary>
        /// User ID for identity config.
        /// </summary>
        protected int CurrentUserID = Int32.Parse(ConfigurationManager.AppSettings["TestUserID"]);

        /// <summary>
        /// Returns moq object for email component.
        /// </summary>
        /// <returns>Moq object for email component</returns>
        protected Mock<IEmailUtility> GetEmailMoq()
        {
            Mock<IEmailUtility> emailUtilityMoq = new Mock<IEmailUtility>();
            emailUtilityMoq
                .Setup(e => e.Send(It.IsAny<MailAddress>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MailAddress>(), It.IsAny<bool>()))
                .Verifiable();

            emailUtilityMoq
                .SetupGet(e => e.SupportEmail)
                .Returns(new MailAddress(WebConfigurationManager.AppSettings["SupportEmail"].ToString(), 
                    WebConfigurationManager.AppSettings["SupportEmailDisplayName"].ToString()));

            emailUtilityMoq
               .SetupGet(e => e.DevelopersEmail)
               .Returns(new MailAddress(WebConfigurationManager.AppSettings["DevelopersEmail"].ToString(), 
                    WebConfigurationManager.AppSettings["DevelopersEmailDisplayName"].ToString()));

            emailUtilityMoq
                .Setup(e => e.SendAsync(It.IsAny<MailAddress>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MailAddress>(), It.IsAny<bool>()))
                .Verifiable();

            return emailUtilityMoq;
        }
    }
}
