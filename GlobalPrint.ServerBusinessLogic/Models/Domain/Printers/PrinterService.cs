using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.Models.Domain.Printers
{
    [Table("printer_service", Schema = "public")]
    public class PrinterService : IDomainModel
    {
        [DebuggerStepThrough]
        public PrinterService()
        {
        }
        
        [Column("print_service_id")]
        public int PrintServiceID { get; set; }
        [Column("printer_id")]
        public int PrinterID { get; set; }
        [Column("price_per_page")]
        public decimal PricePerPage { get; set; }

        /// <summary>
        /// Formatted value (2 decimals after point) for price per page.
        /// </summary>
        public string PricePerPageString
        {
            get
            {
                return this.PricePerPage.ToString("0.00");
            }
        }

        #region IDomainModel
        [Key]
        [Column("printer_service_id")]
        public int ID { get; set; }
        #endregion
    }
}
