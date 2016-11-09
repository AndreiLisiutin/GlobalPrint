using GlobalPrint.ClientWeb.Models;
using GlobalPrint.ClientWeb.Models.FeedbackViewModel;
using GlobalPrint.Configuration.DI;
using GlobalPrint.Infrastructure.EmailUtility;
using GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb.Controllers
{
    public class FeedbackController : BaseController
    {
        private Lazy<IEmailUtility> _emailUtility { get; set; }

        public FeedbackController(Lazy<IEmailUtility> emailUtility)
            : base()
        {
            _emailUtility = emailUtility;
        }

        /// <summary>
        /// Get feedback form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Feedback()
        {
            if (User.Identity.IsAuthenticated)
            {
                FeedbackViewModel currentUserFeedback = new FeedbackViewModel();

                var userUnit = IoC.Instance.Resolve<IUserUnit>();
                var user = userUnit.GetByID(this.GetCurrentUserID());
                currentUserFeedback.Email = user.Email;
                currentUserFeedback.UserName = user.UserName;

                return View(currentUserFeedback);
            }

            return View();
        }

        /// <summary>
        /// Send feedback message
        /// </summary>
        /// <param name="model">Feedback message information</param>
        /// <returns>Redirects to Messsage view with feedback thanks</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Feedback(FeedbackViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("About", model);
            }

            // send email to Globalprint Support
            string messageBody =
                "Email: " + model.Email + Environment.NewLine +
                "Имя пользователя: " + model.UserName + Environment.NewLine + Environment.NewLine +
                model.Message;
            MailAddress userMail = new MailAddress(model.Email, model.UserName);
            await _emailUtility.Value.SendAsync(_emailUtility.Value.SupportEmail, model.Subject, messageBody, userMail);

            // send email to user
            string userMessageBody = "Ваше сообщение обратной связи от " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + " успешно отправлено." + Environment.NewLine +
                "Ваш email: " + model.Email + Environment.NewLine +
                "Ваше имя: " + model.UserName + Environment.NewLine +
                "Тема сообщения: " + model.Subject + Environment.NewLine + Environment.NewLine +
                model.Message + Environment.NewLine + Environment.NewLine +
                "Спасибо Вам за отзыв о нашем сервисе. Нам очень важно ваше мнение, ведь Вы помогаете нам стать еще лучше!";
            await _emailUtility.Value.SendAsync(userMail, "Global Print - сообщение обратной связи", userMessageBody);

            SimpleMessage simpleMessage = new SimpleMessage()
            {
                Title = "Обратная связь",
                Message = "Ваше сообщение успешно отправлено." +
                    "Спасибо Вам за отзыв о нашем сервисе. Нам очень важно ваше мнение, ведь Вы помогаете нам стать еще лучше!"
            };
            return View("SimpleMessage", simpleMessage);
        }
    }
}