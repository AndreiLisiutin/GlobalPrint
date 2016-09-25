using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Payment
{
    /// <summary>
    /// Type ofpayment action, like payment for an order or fullfilling the user's balance.
    /// </summary>
    [Table("payment_action_type", Schema = "public")]
    public class PaymentActionType : IDomainModel
    {
        [DebuggerStepThrough]
        public PaymentActionType()
        {
        }

        #region IDomainModel

        /// <summary>
        /// ID of offer type. Primary key.
        /// </summary>
        [Key]
        [Column("payment_action_type_id")]
        public int ID { get; set; }

        #endregion

        /// <summary>
        /// Name.
        /// </summary>
        [Column("name")]
        public string Name { get; set; }
    }
}
