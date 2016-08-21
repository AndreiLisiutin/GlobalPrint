using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Printers
{
    [Table("printer_service", Schema = "public")]
    public class PrinterService : IDomainModel
    {
        [NotMapped]
        private int PrinterServiceID { get; set; }
        [Column("print_service_id")]
        public int PrintServiceID { get; set; }
        [Column("printer_id")]
        public int PrinterID { get; set; }
        [Column("price_per_page")]
        public decimal PricePerPage { get; set; }

        #region IDomainModel
        [Key]
        [Column("printer_service_id")]
        public int ID
        {
            get { return this.PrinterServiceID; }
            set { this.PrinterServiceID = value; }
        }
        #endregion
    }
}
