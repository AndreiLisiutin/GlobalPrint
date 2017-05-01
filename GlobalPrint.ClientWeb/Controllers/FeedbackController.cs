using GlobalPrint.ClientWeb.Models;
using GlobalPrint.ClientWeb.Models.FeedbackViewModel;
using GlobalPrint.Infrastructure.EmailUtility;
using GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Users;
using System;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb.Controllers
{
    /// <summary>
    /// Контроллер обратной связи.
    /// </summary>
    public class FeedbackController : BaseController
    {
        /// <summary>
        /// Утилита отправки email сообщений.
        /// </summary>
        private Lazy<IEmailUtility> _emailUtility { get; set; }

        /// <summary>
        /// Модуль бизнес логики пользователей.
        /// </summary>
        private IUserUnit _userUnit { get; set; }

        public FeedbackController(Lazy<IEmailUtility> emailUtility, IUserUnit userUnit)
            : base()
        {
            _emailUtility = emailUtility;
            _userUnit = userUnit;
        }

        /// <summary>
        /// Получить страницу обратной связи.
        /// </summary>
        /// <returns>Страница обратной связи.</returns>
        [HttpGet, AllowAnonymous]
        public ActionResult Feedback()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = _userUnit.GetByID(GetCurrentUserID());
                var currentUserFeedback = new FeedbackViewModel
                {
                    Email = user.Email,
                    UserName = user.UserName
                };
                return View(currentUserFeedback);
            }

            return View();
        }

        /// <summary>
        /// Отправить сообщение обратной связи.
        /// </summary>
        /// <param name="model">Обратная связь пользователя.</param>
        /// <returns>Страница с сообщением об успешности операции.</returns>
        [HttpPost, AllowAnonymous]
        public async Task<ActionResult> Feedback(FeedbackViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("About", model);
            }

            // Отправить соообщение на почту поддержки globalprint
            var n = Environment.NewLine;
            var messageBody = 
                $"Email: {model.Email} {n}" +
                $"Имя пользователя: {model.UserName} {n}{n}" +
                $"{model.Message}";
            MailAddress userMail = new MailAddress(model.Email, model.UserName);
            await _emailUtility.Value.SendAsync(_emailUtility.Value.SupportEmail, model.Subject, messageBody, userMail);
            
            // Отправить сообщение о его сообщении ему на email
            string userMessageBody = 
                $"Ваше сообщение обратной связи от {DateTime.Now.ToString("dd.MM.yyyy HH:mm")} успешно отправлено. {n}" +
                $"Ваш email: {model.Email} {n}" +
                $"Ваше имя: {model.UserName} {n}" +
                $"Тема сообщения: {model.Subject} {n}{n}" +
                $"{model.Message} {n}{n}" +
                "Спасибо Вам за отзыв о нашем сервисе. Нам очень важно ваше мнение, ведь Вы помогаете нам стать еще лучше!";
            await _emailUtility.Value.SendAsync(userMail, "Global Print - сообщение обратной связи", userMessageBody);
            
            return View("FeedbackCompleted");
        }
    }
}