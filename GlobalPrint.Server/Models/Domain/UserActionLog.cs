using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Server.Models
{
    [Table("user_action_log")]
    public class UserActionLog
    {
        [Column("user_action_log_id")]
        public int UserActionLogID { get; set; }
        [Column("user_action_type_id")]
        public int UserActionTypeID { get; set; }
        [Column("date")]
        public DateTime Date { get; set; }
        [Column("log")]
        public string Log { get; set; }
        [Column("user_id")]
        public int UserID { get; set; }
    }
}
