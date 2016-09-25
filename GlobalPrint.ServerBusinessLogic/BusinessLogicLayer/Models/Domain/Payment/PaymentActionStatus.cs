using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Payment
{
    /// <summary>
    /// Status of payment action.
    /// </summary>
    [Table("payment_action_status", Schema = "public")]
    public class PaymentActionStatus : IDomainModel
    {
        [DebuggerStepThrough]
        public PaymentActionStatus()
        {
        }

        #region IDomainModel

        /// <summary>
        /// ID of offer type. Primary key.
        /// </summary>
        [Key]
        [Column("payment_action_status_id")]
        public int ID { get; set; }

        #endregion

        /// <summary>
        /// Name.
        /// </summary>
        [Column("name")]
        public string Name { get; set; }
    }
}
