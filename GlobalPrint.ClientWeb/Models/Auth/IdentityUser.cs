using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Users;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                return User.UserID;
            }
            set
            {
                User.UserID = value;
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
    }
}
