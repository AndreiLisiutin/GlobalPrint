using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace GlobalPrint.ServerBusinessLogic.Models.Domain.Offers
{
    /// <summary>
    /// Offer type. User offer/printer owner offer/smth else.
    /// </summary>
    [Table("offer_type", Schema = "public")]
    public class OfferType : IDomainModel
    {
        [DebuggerStepThrough]
        public OfferType()
        {
        }

        #region IDomainModel

        /// <summary>
        /// ID of offer type. Primary key.
        /// </summary>
        [Key]
        [Column("offer_type_id")]
        public int ID { get; set; }

        #endregion

        /// <summary>
        /// Offer type name.
        /// </summary>
        [Column("name")]
        public string Name { get; set; }
    }
}
