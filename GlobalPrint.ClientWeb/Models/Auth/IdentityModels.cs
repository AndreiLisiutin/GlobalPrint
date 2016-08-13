using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration.Conventions;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Users;

namespace GlobalPrint.ClientWeb
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser(User user)
            : base(user)
        {
        }
        public ApplicationUser(string userName, string email)
            : base(new User())
        {
            this.UserName = userName;
            this.User.Email = email;
        }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

}