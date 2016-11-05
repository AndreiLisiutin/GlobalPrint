using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace GlobalPrint.ServerBusinessLogic.Models.Domain.Users
{
    [Table("role", Schema = "public")]
    public class Role : IDomainModel
    {
        [DebuggerStepThrough]
        public Role()
        {
        }
        
        #region IDomainModel

        /// <summary>
        /// Role identifier.
        /// </summary>
        [Key]
        [Column("role_id")]
        public int ID { get; set; }

        #endregion

        /// <summary>
        /// Role name.
        /// </summary>
        [Column("name")]
        public string Name { get; set; }
    }
}
