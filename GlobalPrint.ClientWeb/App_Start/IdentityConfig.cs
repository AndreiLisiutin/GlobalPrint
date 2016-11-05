using GlobalPrint.ClientWeb.Models.Auth;
using GlobalPrint.Configuration.DI;
using GlobalPrint.Infrastructure.EmailUtility;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Users;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GlobalPrint.ClientWeb.App_Start
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser, int>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser, int> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            UserUnit userUnit = IoC.Instance.Resolve<UserUnit>();
            RoleUnit roleUnit = IoC.Instance.Resolve<RoleUnit>();
            UserRoleUnit userRoleUnit = IoC.Instance.Resolve<UserRoleUnit>();
            var manager = new ApplicationUserManager(new UserStore(userUnit, roleUnit, userRoleUnit));
            
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser, int>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                //RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                var provider = new DataProtectorTokenProvider<ApplicationUser, int>(dataProtectionProvider.Create("ASP.NET Identity"));
                manager.UserTokenProvider = provider;
            }

            //manager.SmsService = new SmsService();
            manager.EmailService = IoC.Instance.Resolve<EmailService>();

            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, int>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }

    // Configure the application role manager which is used in this application.
    public class ApplicationRoleManager : RoleManager<GlobalPrint.ClientWeb.Models.Auth.IdentityRole, int>
    {
        public ApplicationRoleManager(IRoleStore<GlobalPrint.ClientWeb.Models.Auth.IdentityRole, int> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            RoleUnit roleUnit = IoC.Instance.Resolve<RoleUnit>();
            var manager = new ApplicationRoleManager(new RoleStore(roleUnit));
            return manager;
        }
    }


    [Obsolete("For testing only", true)]
    public class CustomPasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return password;
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            if (hashedPassword.Equals(providedPassword))
                return PasswordVerificationResult.Success;
            else
                return PasswordVerificationResult.Failed;
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your sms service here to send a text message.
            return Task.FromResult(0);
        }
    }

    public class EmailService : IIdentityMessageService
    {
        private Lazy<IEmailUtility> _emailUtility { get; set; }

        public EmailService(Lazy<IEmailUtility> emailUtility)
        {
            _emailUtility = emailUtility;
        }

        public Task SendAsync(IdentityMessage message)
        {
            return this._emailUtility.Value.SendAsync(new MailAddress(message.Destination), message.Subject, message.Body);
        }
    }
}
