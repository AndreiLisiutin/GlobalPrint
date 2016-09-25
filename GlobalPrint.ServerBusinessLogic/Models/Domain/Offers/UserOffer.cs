using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;

namespace GlobalPrint.ServerBusinessLogic.Models.Domain.Offers
{
    /// <summary>
    /// Concrete offer of concrete user. Realizes M:N relationship user-offer.
    /// </summary>
    [Table("user_offer", Schema = "public")]
    public class UserOffer : IDomainModel
    {
        [DebuggerStepThrough]
        public UserOffer()
        {
        }

        #region IDomainModel

        /// <summary>
        /// ID of user offer. Primary key.
        /// </summary>
        [Key]
        [Column("user_offer_id")]
        public int ID { get; set; }

        #endregion

        /// <summary>
        /// User identifier. Foreign key on <see cref="User.ID"/>.
        /// </summary>
        [Column("user_id")]
        public int UserID { get; set; }

        /// <summary>
        /// Offer identifier. Foreign key on <see cref="Offer.ID"/>.
        /// </summary>
        [Column("offer_id")]
        public int OfferID { get; set; }

        /// <summary>
        /// Date when user <see cref="UserID"/> signs his offer <see cref="OfferID"/>.
        /// </summary>
        [Column("offer_date")]
        public DateTime OfferDate { get; set; }

        /// <summary>
        /// Number of signed offer.
        /// </summary>
        [Column("offer_number")]
        public string OfferNumber { get; set; }
    }
}
