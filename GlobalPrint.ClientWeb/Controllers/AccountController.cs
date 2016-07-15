using GlobalPrint.Server;
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

namespace GlobalPrint.ClientWeb
{
    public class AccountController : BaseController
    {
        #region Helpers
        
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

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
        #endregion

        #region Register

        // GET: Account/Register
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            model.Name = model.Email ?? model.Phone;
            // If we got this far, something failed, redisplay form
            if (!ModelState.IsValid)
            {
                return View("Register", model);
            }

            var user = new ApplicationUser { UserName = model.Name, Email = model.Email, PhoneNumber = model.Phone ?? "" };
            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var currentUser = await this.UserManager.FindByNameAsync(model.Name);
                //await SignInManager.PasswordSignInAsync(model.Email, model.Password, false, shouldLockout: false);
                await SignInManager.SignInAsync(currentUser, isPersistent: false, rememberBrowser: false);

                string printerID = Session["Account_PrinterID"] as string;
                if (printerID != null)
                {
                    Session["Account_PrinterID"] = null;
                    return RedirectToAction("Print", "Printer", new { PrinterID = printerID });
                }

                return RedirectToAction("Index", "Home");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
            return View("Register", model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RegisterFromPhone(RegisterViewModel model)
        {
            var smsUtility = new SmsUtility(this.GetSmsParams());
            var phoneNumber = SmsUtility.ExtractValidPhone(model.Phone);
            if (phoneNumber == null)
            {
                return View("Register", model);
            }

            var password = smsUtility.GetneratePassword(6);
            smsUtility.Send(model.Phone, "Ваш пароль: " + password);
            this.Session["SmsValidationPassword"] = password;

            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = phoneNumber, FromRegistration = true });
        }

        #endregion
        
        #region Login

        // GET: Account/Login
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View("Login", model);
            }

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
                    ModelState.AddModelError("", "Некорректно введен логин/пароль.");
                    return View("Login", model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> LoginFromPhone(LoginViewModel model)
        {
            var smsUtility = new SmsUtility(this.GetSmsParams());
            model.Phone = SmsUtility.ExtractValidPhone(model.Phone);
            if (string.IsNullOrEmpty(model.Phone))
            {
                return View("Login", model);
            }

            var password = smsUtility.GetneratePassword(6);
            smsUtility.Send(model.Phone, "Ваш пароль: " + password);
            this.Session["SmsLoginValidationPassword"] = password;

            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Phone, FromRegistration = false });
        }
        
        #endregion
        
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
                        Name = model.PhoneNumber,
                        Email = model.PhoneNumber,
                        Password = model.Code,
                        ConfirmPassword = model.Code,
                        Phone = model.PhoneNumber
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


        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

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



        [HttpGet]
        [AllowAnonymous]
        public ActionResult LoginAndPrint(int printerID)
        {
            Session["Account_PrinterID"] = printerID.ToString();
            return View("Login");
        }
    }
}
