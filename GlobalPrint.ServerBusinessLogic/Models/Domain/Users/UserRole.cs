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
    [Table("user_role", Schema = "public")]
    public class UserRole : IDomainModel
    {
        [DebuggerStepThrough]
        public UserRole()
        {
        }

        #region IDomainModel

        [Key]
        [Column("user_role_id")]
        public int ID { get; set; }

        #endregion
        
        /// <summary>
        /// User identifier.
        /// </summary>
        [Column("user_id")]
        public int UserID { get; set; }

        /// <summary>
        /// Role identifier.
        /// </summary>
        [Column("role_id")]
        public int RoleID { get; set; }
    }
}
