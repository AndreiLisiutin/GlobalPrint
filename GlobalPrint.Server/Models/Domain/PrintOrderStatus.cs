using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Server
{
    [Table("print_order_status", Schema = "public")]
    public class PrintOrderStatus
    {
        [Key]
        [Column("print_order_status_id")]
        public int PrintOrderStatusID { get; set; }
        [Column("status")]
        public string Status { get; set; }
    }
}
