using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Server.Models
{
    [Table("role", Schema = "public")]
    public class Role
    {
        [Key]
        [Column("role_id")]
        public int RoleID { get; set; }
        [Column("name")]
        public string Name { get; set; }
    }
}
