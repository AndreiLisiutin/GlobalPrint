﻿using System;
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
        [Column("document")]
        public string Document { get; set; }
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

        /// <summary> Number of copies that is requested to print.
        /// </summary>
        [Column("copies_count")]
        public int CopiesCount { get; set; }

        /// <summary> Comment to the order.
        /// </summary>
        [Column("comment")]
        public string Comment { get; set; }
        
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
                return this.PricePerPage * this.PagesCount * this.CopiesCount;
            }
        }
        [NotMapped]
        public string DocumentName
        {
            get
            {
                return this.Document == null ? null : new FileInfo(this.Document).Name;
            }
        }

        #region IDomainModel
        [Key]
        [Column("print_order_id")]
        public int ID { get; set; }
        #endregion
    }
}