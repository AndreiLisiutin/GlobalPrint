using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Printers
{
    [Table("print_size", Schema = "public")]
    public class PrintSize
    {
        [Key]
        [Column("print_size_id")]
        public int PrintSizeID { get; set; }
        [Column("name")]
        public string Name { get; set; }
    }
}
