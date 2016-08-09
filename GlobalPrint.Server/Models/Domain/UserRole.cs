using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Server.Models
{
    [Table("user_role", Schema = "public")]
    public class UserRole
    {
        [Key]
        [Column("user_role_id")]
        public int UserRoleID { get; set; }
        [Column("user_id")]
        public int UserID { get; set; }
        [Column("role_id")]
        public int RoleID { get; set; }
    }
}
