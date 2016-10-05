using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.UnitsOfWork.Order;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.Models.Domain.Orders
{
    [Table("print_order", Schema = "public")]
    public class PrintOrder : IDomainModel
    {
        [DebuggerStepThrough]
        public PrintOrder()
        {
        }

        [Column("user_id")]
        public int UserID { get; set; }
        [Column("printer_id")]
        public int PrinterID { get; set; }
        /// <summary>
        /// Physical name of the file on disc included file extension. Example: myFile.txt.
        /// </summary>
        [Column("document")]
        public string InternalDocumentName { get; set; }
        [Column("ordered_on")]
        public DateTime OrderedOn { get; set; }
        [Column("printed_on")]
        public DateTime? PrintedOn { get; set; }
        [Column("price_per_page")]
        public decimal PricePerPage { get; set; }
        [Column("pages_count")]
        public int PagesCount { get; set; }
        [Column("secret_code")]
        public string SecretCode { get; set; }
        [Column("print_order_status_id")]
        public int PrintOrderStatusID { get; set; }
        [Column("print_service_id")]
        public int PrintServiceID { get; set; }

        /// <summary>
        /// Reference to the payment transaction, which operates the order's payment actions.
        /// </summary>
        [Column("payment_transaction_id")]
        public int PaymentTransactionID { get; set; }

        /// <summary>
        /// Name of the document included extension like: myFile.txt.
        /// </summary>
        [Column("document_name")]
        public string DocumentName { get; set; }

        /// <summary>
        /// Only extension of the printed document: "txt" for document_name "myFile.txt".
        /// </summary>
        [Column("document_extension")]
        public string DocumentExtension { get; set; }

        /// <summary> Number of copies that is requested to print.
        /// </summary>
        [Column("copies_count")]
        public int CopiesCount { get; set; }

        /// <summary> Comment to the order.
        /// </summary>
        [Column("comment")]
        public string Comment { get; set; }

        /// <summary> Rating of the order.
        /// </summary>
        [Column("rating")]
        public float? Rating { get; set; }

        [NotMapped]
        public string PriceInCurrency
        {
            get
            {
                return this.FullPrice.ToString("#.##").ToString() + " руб.";
            }
        }
        [NotMapped]
        public decimal FullPrice
        {
            get
            {
                return PrintOrderUnit.CALCULATE_FULL_PRICE(this.PricePerPage, this.PagesCount, this.CopiesCount);
            }
        }

        #region IDomainModel
        [Key]
        [Column("print_order_id")]
        public int ID { get; set; }
        #endregion
    }
}