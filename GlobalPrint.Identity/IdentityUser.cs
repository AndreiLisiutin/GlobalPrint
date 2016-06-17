using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Identity
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
