using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Server
{
    [Table("printer", Schema = "public")]
    public class Printer
    {
        [Key]
        [Column("printer_id")]
        public int PrinterID { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("location")]
        public string Location { get; set; }
        [Column("user_id")]
        public int UserID { get; set; }
        [Column("latitude")]
        public float Latitude { get; set; }
        [Column("longtitude")]
        public float Longtitude { get; set; }
    }
}