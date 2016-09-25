using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace GlobalPrint.ServerBusinessLogic.Models.Domain.Payment
{
    /// <summary>
    /// Payment transaction. Contains one or several payment actions 
    /// and execute all of not yet executea actions on commit 
    /// or rolls back already executed actions on rollback.
    /// </summary>
    [Table("payment_transaction", Schema = "public")]
    public class PaymentTransaction : IDomainModel
    {
        [DebuggerStepThrough]
        public PaymentTransaction()
        {
        }

        #region IDomainModel

        /// <summary>
        /// ID of offer type. Primary key.
        /// </summary>
        [Key]
        [Column("payment_transaction_id")]
        public int ID { get; set; }

        #endregion

        /// <summary>
        /// Name.
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Status of transaction: In progress, commited, rolled back etc.
        /// Reference to <see cref="PaymentTransactionStatus.ID"/>.
        /// </summary>
        [Column("payment_transaction_status_id")]
        public int PaymentTransactionStatusID { get; set; }

        /// <summary>
        /// Comment.
        /// </summary>
        [Column("comment")]
        public int Comment { get; set; }

        /// <summary>
        /// Start date.
        /// </summary>
        [Column("started_on")]
        public DateTime StartedOn { get; set; }

        /// <summary>
        /// Finish date.
        /// </summary>
        [Column("finished_on")]
        public DateTime? FinishedOn { get; set; }
    }
}
