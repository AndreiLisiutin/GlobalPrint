using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Payment
{
    /// <summary>
    /// Single action operating with money. 
    /// Increases or decreases amount of money for user by a certain value.
    /// Required to be fully performed when in status "Done", otherwise is not performed yet.
    /// </summary>
    [Table("payment_action", Schema = "public")]
    public class PaymentAction : IDomainModel
    {
        [DebuggerStepThrough]
        public PaymentAction()
        {
        }

        #region IDomainModel

        /// <summary>
        /// ID of offer type. Primary key.
        /// </summary>
        [Key]
        [Column("payment_action_id")]
        public int ID { get; set; }

        #endregion

        /// <summary>
        /// Logical transaction that holding current action.
        /// Reference to <see cref="PaymentTransaction.ID"/>.
        /// </summary>
        [Column("payment_transaction_id")]
        public int PaymentTransactionID { get; set; }

        /// <summary>
        /// Amount of money to increase (if positive) or decrease (if negative) user's amount of money.
        /// </summary>
        [Column("amount_of_money")]
        public decimal AmountOfMoney { get; set; }

        /// <summary>
        /// Receiver of money or payment person.
        /// Reference to <see cref="Users.User.ID"/>.
        /// </summary>
        [Column("user_id")]
        public int UserID { get; set; }

        /// <summary>
        /// In our case, ID for Robokassa.
        /// </summary>
        [Column("external_identifier")]
        public string ExternalIdentifier { get; set; }

        /// <summary>
        /// Type of action: fullfilling user's account or payment for order or anything.
        /// Reference to <see cref="PaymentActionType.ID"/>.
        /// </summary>
        [Column("payment_action_type_id")]
        public int PaymentActionTypeID { get; set; }

        /// <summary>
        /// Status of action: In progress, done, Aborted etc.
        /// Reference to <see cref="PaymentActionStatus.ID"/>.
        /// </summary>
        [Column("payment_action_status_id")]
        public int PaymentActionStatusID { get; set; }

        /// <summary>
        /// Comment.
        /// </summary>
        [Column("comment")]
        public string Comment { get; set; }

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
