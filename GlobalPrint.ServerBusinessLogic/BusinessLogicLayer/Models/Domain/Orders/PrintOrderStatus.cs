﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Orders
{
    [Table("print_order_status", Schema = "public")]
    public class PrintOrderStatus : IDomainModel
    {
        [Key]
        [Column("print_order_status_id")]
        public int PrintOrderStatusID { get; set; }
        [Column("status")]
        public string Status { get; set; }

        #region IDomainModel
        [NotMapped]
        public int ID
        {
            get { return this.PrintOrderStatusID; }
            set { this.PrintOrderStatusID = value; }
        }
        #endregion
    }
}
