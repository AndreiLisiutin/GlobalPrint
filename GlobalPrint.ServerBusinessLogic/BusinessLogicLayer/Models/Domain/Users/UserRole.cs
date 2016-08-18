using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Users
{
    [Table("user_role", Schema = "public")]
    public class UserRole : IDomainModel
    {
        [Key]
        [Column("user_role_id")]
        public int UserRoleID { get; set; }
        [Column("user_id")]
        public int UserID { get; set; }
        [Column("role_id")]
        public int RoleID { get; set; }

        #region IDomainModel
        [NotMapped]
        public int ID
        {
            get { return this.UserRoleID; }
            set { this.UserRoleID = value; }
        }
        #endregion
    }
}
