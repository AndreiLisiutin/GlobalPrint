using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using Microsoft.AspNet.Identity;

namespace GlobalPrint.ClientWeb
{
    public class IdentityUser : IUser<int>
    {
        public IdentityUser(User user)
        {
            this.User = user;
        }
        public User User { get; set; }
        
        public int Id
        {
            get
            {
                return this.User.UserID;
            }
            set
            {
                this.User.UserID = value;
            }
        }
        public string UserName
        {
            get
            {
                return this.User.UserName;
            }
            set
            {
                this.User.UserName = value;
            }
        }
        public bool EmailConfirmed
        {
            get
            {
                return this.User.EmailConfirmed;
            }
            set
            {
                this.User.EmailConfirmed = value;
            } 
        }
    }
}
