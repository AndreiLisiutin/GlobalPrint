using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace GlobalPrint.ServerBusinessLogic.Models.Domain.Payment
{
    /// <summary>
    /// Status of payment transaction.
    /// </summary>
    [Table("payment_transaction_status", Schema = "public")]
    public class PaymentTransactionStatus : IDomainModel
    {
        [DebuggerStepThrough]
        public PaymentTransactionStatus()
        {
        }

        #region IDomainModel

        /// <summary>
        /// ID of offer type. Primary key.
        /// </summary>
        [Key]
        [Column("payment_transaction_status_id")]
        public int ID { get; set; }

        #endregion

        /// <summary>
        /// Name.
        /// </summary>
        [Column("name")]
        public string Name { get; set; }
    }
}
