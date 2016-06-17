using GlobalPrint.Server;
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
    [Table("user", Schema = "public")]
    public class IdentityUser : IUser<int>
    {
        [Key]
        [Column("user_id")]
        public int Id { get; set; }
        [Column("name")]
        public string UserName { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("phone")]
        public string Phone { get; set; }
        [Column("login")]
        public string Login { get; set; }
        [Column("password")]
        public string Password { get; set; }
        [Column("password_hash")]
        public string PasswordHash { get; set; }

        public static ApplicationUser FromDbUser(User user)
        {
            if (user == null) return null;
            return new ApplicationUser()
            {
                Id = user.UserID,
                UserName = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Login = user.Login,
                Password = user.Password,
                PasswordHash = user.PasswordHash
            };
        }

        public User ToDbUser()
        {
            if (this == null) return null;
            return new User()
            {
                UserID = this.Id,
                Name = this.UserName,
                Email = this.Email,
                Phone = this.Phone,
                Login = this.Login,
                Password = this.Password,
                PasswordHash = this.PasswordHash
            };
        }
    }

    public class IdentityUserLogin
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Provider { get; set; }
        public string ProviderKey { get; set; }
    }

    public class IdentityUserClaim
    {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }

    public class IdentityUserByUserName
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
