using GlobalPrint.ClientWeb.Models.Auth;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

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

    //public class ApplicationRole : IdentityRole
    //{
    //    public ApplicationRole(Role role)
    //        : base(role)
    //    {
    //    }
    //    public ApplicationRole(int ID)
    //        : base(ID)
    //    {
    //    }
    //    public ApplicationRole(string name)
    //        : base(name)
    //    {
    //    }
    //    public ApplicationRole(int ID, string name)
    //        : base(ID, name)
    //    {
    //    }

    //    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, int> manager)
    //    {
    //        // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
    //        var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
    //        // Add custom user claims here
    //        return userIdentity;
    //    }
    //}

}