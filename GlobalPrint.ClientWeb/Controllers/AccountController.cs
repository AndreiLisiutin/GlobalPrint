using GlobalPrint.ClientWeb.App_Start;
using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.Infrastructure.LogUtility;
using GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Users;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace GlobalPrint.ClientWeb
{
    /// <summary>
    /// Контроллер аккаунта пользователя.
    /// </summary>
    public class AccountController : BaseController
    {
        public ApplicationRoleManager RoleManager => HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();
        public IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;
        public ApplicationSignInManager SignInManager => HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
        public ApplicationUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

        /// <summary>
        /// Утилита для логирования.
        /// </summary>
        private readonly Lazy<ILogger> _logger;

        /// <summary>
        /// Модуль бизнес логики пользователей.
        /// </summary>
        private readonly IUserUnit _userUnit;

        public AccountController(ILoggerFactory loggerFactory, IUserUnit userUnit)
        {
            _logger = new Lazy<ILogger>(() => loggerFactory.GetLogger<AccountController>());
            _userUnit = userUnit;
        }


        #region Регистрация и логинка

        /// <summary>
        /// Получить страницу регистрции.
        /// </summary>
        /// <returns>Страница регистрации.</returns>
        [HttpGet, AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Зарегистрировать пользователя.
        /// </summary>
        /// <param name="model">Регистрационные данные пользователя.</param>
        /// <returns>Страница подтверждения регистрации через email.</returns>
        [ValidateAntiForgeryToken, HttpPost, AllowAnonymous]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            // If we got this far, something failed, redisplay form
            if (!ModelState.IsValid)
            {
                return View("Register", model);
            }

            var registeredUser = _userUnit.GetByFilter(e => e.Email == model.Email);
            if (registeredUser != null)
            {
                ModelState.AddModelError("", "Пользователь с таким email уже зарегистрирован в системе.");
                return View("Register", model);
            }

            var user = new ApplicationUser(model.UserName ?? model.Email, model.Email);
            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // генерируем токен для подтверждения регистрации
                var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

                string callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code },
                    protocol: Request.Url.Scheme);

                // отправка письма
                await UserManager.SendEmailAsync(user.Id, "Подтверждение электронной почты",
                    "Для завершения регистрации перейдите по ссылке: <a href=\"" + callbackUrl + "\">завершить регистрацию</a>");

                return View("DisplayEmail");

                #region Old version of registration process, without email confirmation
                if (false)
                {
                    var currentUser = await UserManager.FindByNameAsync(model.Email);
                    await SignInManager.SignInAsync(currentUser, isPersistent: false, rememberBrowser: false);

                    // In case of Print->Register
                    string printerID = Session["Account_PrinterID"] as string;
                    if (!string.IsNullOrEmpty(printerID))
                    {
                        Session["Account_PrinterID"] = null;
                        return RedirectToAction("New", "Order", new { PrinterID = printerID });
                    }

                    return RedirectToAction("Index", "Home");
                }
                #endregion
            }
            else
            {
                // Add errors to ModelState, replasing "Name XXX is alerady taken." by russian error
                AddErrors(result);
            }

            return View("Register", model);
        }

        /// <summary>
        /// Получить страницу логина.
        /// </summary>
        /// <returns>Страница логина.</returns>
        [HttpGet, AllowAnonymous, OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Залогинить пользователя.
        /// </summary>
        /// <param name="model">Информация пользователя для логина на сайт.</param>
        /// <param name="returnUrl">Url для перехода после логина.</param>
        /// <returns>Переходит по returnUrl.</returns>
        [ValidateAntiForgeryToken, HttpPost, AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.Email, model.Password);
                if (user != null)
                {
                    if (user.EmailConfirmed == true)
                    {
                        // This doesn't count login failures towards account lockout
                        // To enable password failures to trigger account lockout, change to shouldLockout: true
                        var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
                        switch (result)
                        {
                            case SignInStatus.Success:
                                return RedirectToLocal(returnUrl);
                            case SignInStatus.LockedOut:
                                return View("Lockout");
                            case SignInStatus.RequiresVerification:
                                return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                            case SignInStatus.Failure:
                            default:
                                ModelState.AddModelError("", "Неверный логин или пароль.");
                                break;
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Не подтвержден email.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неверный логин или пароль.");
                }
            }

            return View(model);
        }

        #endregion

        #region Восстановление пароля

        /// <summary>
        /// Получить страницу сброса пароля.
        /// </summary>
        /// <returns>Страница сброса пароля.</returns>
        [HttpGet, AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Сброс пароля.
        /// </summary>
        /// <param name="model">Информация о сбрасываемом пароле.</param>
        /// <returns>Страница подтверждения сброса пароля.</returns>
        [ValidateAntiForgeryToken, HttpPost, AllowAnonymous]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError("", "Не найден пользователь с указаным email.");
                }
                else if (!user.EmailConfirmed)
                {
                    ModelState.AddModelError("", "Не подтвержден email.");
                }
                else if (!(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    ModelState.AddModelError("", "Не подтвержден email.");
                }
                else
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code, email = model.Email },
                        protocol: Request.Url.Scheme);

                    await UserManager.SendEmailAsync(user.Id, "Сброс пароля", "Для сброса пароля, перейдите по <a href=\"" + callbackUrl + "\">ссылке</a>");
                    return RedirectToAction("ForgotPasswordConfirmation", "Account");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Получить страницу подтверждения сброса пароля.
        /// </summary>
        /// <returns>Страница подтверждения сброса пароля.</returns>
        [HttpGet, AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        /// <summary>
        /// Получить страницу восстановления пароля.
        /// </summary>
        /// <param name="code">Токен для восстановления пароля.</param>
        /// <param name="email">Email пользователя.</param>
        /// <returns>Страница восстановления пароля.</returns>
        [HttpGet, AllowAnonymous]
        public ActionResult ResetPassword(string code, string email)
        {
            return code == null || string.IsNullOrEmpty(email)
                ? View("Error")
                : View(new ResetPasswordViewModel() { Email = email });
        }

        /// <summary>
        /// Восстановить пароль пользователя.
        /// </summary>
        /// <param name="model">Данные для восстановления пароля.</param>
        /// <returns>Страница подтверждения восстановления пароля.</returns>
        [ValidateAntiForgeryToken, HttpPost, AllowAnonymous]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Don't reveal that the user does not exist
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            // Reset password in DB
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            // In case of errors - display them
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
            return View();
        }

        /// <summary>
        /// Получить страницу подтверждения восстановления пароля.
        /// </summary>
        /// <returns>Страница подтверждения восстановления пароля.</returns>
        [HttpGet, AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        #endregion

        /// <summary>
        /// Подтвердить email.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="code">Токен для подтверждения email.</param>
        /// <returns>Главная страница сайта.</returns>
        [HttpGet, AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(int userId, string code)
        {
            if (userId > 0 && !string.IsNullOrEmpty(code))
            {
                IdentityResult result = await UserManager.ConfirmEmailAsync(userId, code);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(userId);
                    await SignInManager.SignInAsync(user, false, false);
                    return RedirectToLocal(null);
                }
            }

            return View("Error");
        }

        /// <summary>
        /// Войти на сайт и потом распечатать файл.
        /// Вызывается при попытке незалогиненного пользователя создать заказ на печать.
        /// </summary>
        /// <param name="printerID">Идентификатор принтера, в котором пользователь хочет распечатать документ.</param>
        /// <returns>Страница логина.</returns>
        [HttpGet, AllowAnonymous]
        public ActionResult LoginAndPrint(int printerID)
        {
            Session["Account_PrinterID"] = printerID.ToString();
            return View("Login");
        }

        /// <summary>
        /// Выйти из приложения.
        /// </summary>
        /// <returns>Главная страница сайта.</returns>
        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Перейти на указанную страницу.
        /// </summary>
        /// <param name="returnUrl">Url страницы для редиректа.</param>
        /// <returns>Страница, на которую хотели перейти. Если не указана, редирект на главную страницу сайта.</returns>
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            string printerID = Session["Account_PrinterID"] as string;
            if (printerID != null)
            {
                Session["Account_PrinterID"] = null;
                return RedirectToAction("New", "Order", new { PrinterID = printerID });
            }
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Добавить ошибку в ModelState, заменив "Name XXX is alerady taken." на русскую ошибку.
        /// </summary>
        /// <param name="result">Результат регистрации/логина.</param>
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                if (error.EndsWith("is already taken.", StringComparison.InvariantCultureIgnoreCase))
                {
                    ModelState.AddModelError("", "Указанный email занят.");
                }
                else
                {
                    ModelState.AddModelError("", error);
                }
            }
        }

    }
}
