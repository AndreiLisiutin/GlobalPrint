using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Offers
{
    /// <summary>
    /// Offer type enum.
    /// </summary>
    public enum OfferTypeEnum
    {
        /// <summary>
        /// User offer that user signs during registration. Real <see cref="OfferType.ID"/> = 1.
        /// </summary>
        UserOffer = 1,

        /// <summary>
        /// Printer owner offer that user signs, when he adds new printer. Real <see cref="OfferType.ID"/> = 2.
        /// </summary>
        PrinterOwnerOffer = 2
    }
}
