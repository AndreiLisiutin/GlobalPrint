using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.Models.Domain.Users
{
    [Table("role", Schema = "public")]
    public class Role : IDomainModel
    {
        [DebuggerStepThrough]
        public Role()
        {
        }

        [Key]
        [Column("role_id")]
        public int RoleID { get; set; }
        [Column("name")]
        public string Name { get; set; }

        #region IDomainModel
        [NotMapped]
        public int ID
        {
            get { return this.RoleID; }
            set { this.RoleID = value; }
        }
        #endregion
    }
}
