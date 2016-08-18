using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Utilities;

namespace GlobalPrint.ClientWeb
{
    public class AccountController : BaseController
    {
        private const bool REGISTER_WITH_MAIL_CONFIRM = false;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        #region Old (Phone, ...)

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> LoginFromPhone(LoginViewModel model)
        {
            var smsUtility = new SmsUtility(this.GetSmsParams());
            //model.Phone = SmsUtility.ExtractValidPhone(model.Phone);
            //if (string.IsNullOrEmpty(model.Phone))
            //{
            //    return View("Login", model);
            //}

            var password = smsUtility.GetneratePassword(6);
            //smsUtility.Send(model.Phone, "Ваш пароль: " + password);
            this.Session["SmsLoginValidationPassword"] = password;

            return RedirectToAction("VerifyPhoneNumber", new { /*PhoneNumber = model.Phone,*/ FromRegistration = false });
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RegisterFromPhone(RegisterViewModel model)
        {
            var smsUtility = new SmsUtility(this.GetSmsParams());
            //var phoneNumber = SmsUtility.ExtractValidPhone(model.Phone);
            //if (phoneNumber == null)
            //{
            //    return View("Register", model);
            //}

            var password = smsUtility.GetneratePassword(6);
            //smsUtility.Send(model.Phone, "Ваш пароль: " + password);
            this.Session["SmsValidationPassword"] = password;

            return RedirectToAction("VerifyPhoneNumber", new { /*PhoneNumber = phoneNumber, */FromRegistration = true });
        }

        public ActionResult VerifyPhoneNumber(string phoneNumber, bool FromRegistration)
        {
            ViewBag.FromRegistration = FromRegistration;
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model, bool fromRegistration)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            string password = fromRegistration ? this.Session["SmsValidationPassword"] as string : this.Session["SmsLoginValidationPassword"] as string;
            // Костыль пока
            model.Code = password;
            if (model.Code != password)
            {
                ModelState.AddModelError("", "Введен некорректный пароль");
                return View(model);
            }
            else
            {
                if (fromRegistration)
                {
                    this.Session["SmsValidationPassword"] = null;
                    RegisterViewModel userModel = new RegisterViewModel()
                    {
                        //Name = model.PhoneNumber,
                        Email = model.PhoneNumber,
                        Password = model.Code,
                        ConfirmPassword = model.Code,
                        //Phone = model.PhoneNumber
                    };
                    return await this.Register(userModel);
                }
                else
                {
                    this.Session["SmsLoginValidationPassword"] = null;

                    var currentUser = await UserManager.FindByNameAsync(model.PhoneNumber);
                    if (currentUser != null)
                    {
                        await SignInManager.SignInAsync(currentUser, isPersistent: false, rememberBrowser: false);

                        string printerID = Session["Account_PrinterID"] as string;
                        if (printerID != null)
                        {
                            Session["Account_PrinterID"] = null;
                            return RedirectToAction("Print", "Printer", new { PrinterID = printerID });
                        }

                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            ModelState.AddModelError("", "Не найден пользователь");
            return View(fromRegistration ? "Register" : "Login");
        }

        #endregion

        #region Register

        /// <summary>
        /// Get register page
        /// </summary>
        /// <returns></returns>
        // GET: Account/Register
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Register action
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            // If we got this far, something failed, redisplay form
            if (!ModelState.IsValid)
            {
                return View("Register", model);
            }

            var user = new ApplicationUser(model.Email, model.Email);
            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (REGISTER_WITH_MAIL_CONFIRM)
                {
                    // генерируем токен для подтверждения регистрации
                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

                    // создаем ссылку для подтверждения
                    string callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code },
                               protocol: Request.Url.Scheme);

                    // отправка письма
                    await UserManager.SendEmailAsync(user.Id, "Подтверждение электронной почты",
                        "Для завершения регистрации перейдите по ссылке: <a href=\"" + callbackUrl + "\">завершить регистрацию</a>");

                    return View("DisplayEmail");
                }
                else
                {
                    // Old version of registration process, without email confirmation
                    var currentUser = await this.UserManager.FindByNameAsync(model.Email);
                    await SignInManager.SignInAsync(currentUser, isPersistent: false, rememberBrowser: false);

                    // In case of Print->Register
                    string printerID = Session["Account_PrinterID"] as string;
                    if (!string.IsNullOrEmpty(printerID))
                    {
                        Session["Account_PrinterID"] = null;
                        return RedirectToAction("Print", "Printer", new { PrinterID = printerID });
                    }

                    return RedirectToAction("Index", "Home");
                }
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
            return View("Register", model);
        }

        #endregion

        #region Login

        /// <summary>
        /// Login page
        /// </summary>
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Login action
        /// </summary>
        /// <param name="model">Login wiew model with user data</param>
        /// <param name="returnUrl">Fucking shit</param>
        /// <returns></returns>
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.Email, model.Password);
                if (user != null)
                {
                    if (!REGISTER_WITH_MAIL_CONFIRM || user.EmailConfirmed == true)
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

        #region Forgot Password

        /// <summary>
        /// Get ForgotPassword view
        /// </summary>
        /// <returns></returns>
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Process forgot password action
        /// </summary>
        /// <param name="model">Forgot password model</param>
        /// <returns></returns>
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code, email = model.Email }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Сброс пароля", "Для сброса пароля, перейдите по <a href=\"" + callbackUrl + "\">ссылке</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Get ForgotPasswordConfirmation view
        /// </summary>
        /// <returns></returns>
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        /// <summary>
        /// Get ResetPassword view
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code, string email)
        {
            return code == null || string.IsNullOrEmpty(email) 
                ? View("Error") 
                : View(new ResetPasswordViewModel() { Email = email });
        }

        /// <summary>
        /// Perform reset password action
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
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
        /// Get ResetPasswordConfirmation view
        /// </summary>
        /// <returns></returns>
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        #endregion

        /// <summary>
        /// Confirm email
        /// </summary>
        /// <param name="userId">Registred user ID</param>
        /// <param name="code">Confirmation code</param>
        /// <returns></returns>
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(int userId, string code)
        {
            if (userId == 0 || string.IsNullOrEmpty(code))
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        /// <summary>
        /// Login and print. Causes when unregistred user wants to print smth
        /// </summary>
        /// <param name="printerID">Printer ID to remember</param>
        /// <returns></returns>
        // GET: /Account/LoginAndPrint
        [HttpGet]
        [AllowAnonymous]
        public ActionResult LoginAndPrint(int printerID)
        {
            Session["Account_PrinterID"] = printerID.ToString();
            return View("Login");
        }

        /// <summary>
        /// Log off from the application
        /// </summary>
        /// <returns>Redirect to the Index/Home</returns>
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            this.AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

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
                return RedirectToAction("Print", "Printer", new { PrinterID = printerID });
            }
            return RedirectToAction("Index", "Home");
        }
        
        /// <summary>
        /// Get ConfirmEmail view to test css styles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ConfirmEmail()
        {
            return View();
        }

        /// <summary>
        /// Get DisplayEmail view to test css styles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult DisplayEmail()
        {
            return View();
        }

        /// <summary>
        /// Dispose managed resources
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}
