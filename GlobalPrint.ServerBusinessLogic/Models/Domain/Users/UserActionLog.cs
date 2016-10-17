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
    [Table("user_action_log", Schema = "public")]
    public class UserActionLog : IDomainModel
    {
        [DebuggerStepThrough]
        public UserActionLog()
        {
        }
        
        [Column("user_action_type_id")]
        public int UserActionTypeID { get; set; }
        [Column("date")]
        public DateTime Date { get; set; }
        [Column("log")]
        public string Log { get; set; }
        [Column("user_id")]
        public int UserID { get; set; }

        #region IDomainModel
        [Key]
        [Column("user_action_log_id")]
        public int ID { get; set; }
        #endregion
    }
}
