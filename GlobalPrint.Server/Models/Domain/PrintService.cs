using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Server.Models
{
    [Table("print_service", Schema = "public")]
    public class PrintService
    {
        [Key]
        [Column("print_service_id")]
        public int PrintServiceID { get; set; }
        [Column("print_size_print_type_id")]
        public int PrintSizePrintTypeID { get; set; }
        [Column("is_colored")]
        public bool IsColored { get; set; }
        [Column("is_two_sided")]
        public bool IsTwoSided { get; set; }
    }
}
