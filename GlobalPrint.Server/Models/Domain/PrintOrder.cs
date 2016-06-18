using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Server
{
    [Table("print_order", Schema = "public")]
    public class PrintOrder
    {
        [Key]
        [Column("print_order_id")]
        public int PrintOrderID { get; set; }
        [Column("user_id")]
        public int UserID { get; set; }
        [Column("printer_id")]
        public int PrinterID { get; set; }
        [Column("document")]
        public string Document { get; set; }
        [Column("ordered_on")]
        public DateTime OrderedOn { get; set; }
        [Column("printed_on")]
        public DateTime? PrintedOn { get; set; }
        [Column("price")]
        public decimal Price { get; set; }
        [Column("pages_count")]
        public int PagesCount { get; set; }
        [Column("secret_code")]
        public string SecretCode { get; set; }
        [Column("format")]
        public string Format { get; set; }
        [Column("is_both_sides_print")]
        public bool IsBothSidesPrint { get; set; }
        [Column("print_order_status_id")]
        public int PrintOrderStatusID { get; set; }

        [NotMapped]
        public string PriceInCurrency
        {
            get
            {
                return this.Price.ToString() + " руб.";
            }
        }
    }

    public enum PrintOrderStatusEnum
    {
        Waiting = 1,
        Accepted = 2,
        Printed = 3,
        Rejected = 4
    }
}