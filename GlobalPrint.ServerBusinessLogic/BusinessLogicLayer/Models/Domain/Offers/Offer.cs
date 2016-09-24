using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Offers
{
    /// <summary>
    /// In fact, it's offer type.
    /// </summary>
    [Table("offer", Schema = "public")]
    public class Offer : IDomainModel
    {
        [DebuggerStepThrough]
        public Offer()
        {
        }

        #region IDomainModel

        /// <summary>
        /// ID of offer type. Primary key.
        /// </summary>
        [Key]
        [Column("offer_id")]
        public int ID { get; set; }

        #endregion

        /// <summary>
        /// Name of offer type.
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Offer type identifier. Foreign key on <see cref="OfferType.ID"/>.
        /// </summary>
        [Column("offer_type_id")]
        public int OfferTypeID { get; set; }

        /// <summary>
        /// Version of offer type.
        /// </summary>
        [Column("version")]
        public string Version { get; set; }

        /// <summary>
        /// Direct text of the offer.
        /// </summary>
        [Column("text")]
        public string Text { get; set; }

        /// <summary>
        /// Date of offer type creation.
        /// </summary>
        [Column("created_on")]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Whether this offer with <see cref="OfferTypeID"/> is actial.
        /// </summary>
        [Column("is_actual")]
        public bool IsActual { get; set; }
    }
}
