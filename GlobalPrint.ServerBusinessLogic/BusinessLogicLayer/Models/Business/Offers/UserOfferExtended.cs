using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Offers;
using System.Diagnostics;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Offers
{
    /// <summary> Extended model for user offer.
    /// </summary>
    public class UserOfferExtended
    {
        [DebuggerStepThrough]
        public UserOfferExtended()
        {
        }
        public UserOffer LatestUserOffer { get; set; }
        public Offer Offer { get; set; }
        public bool HasUserOffer
        {
            get
            {
                return this.LatestUserOffer != null;
            }
        }
        public string UserOfferString
        {
            get
            {
                if (HasUserOffer)
                {
                    string offerType = Offer != null ? (Offer.OfferTypeID == (int)OfferTypeEnum.UserOffer ? "пользователя" : "владельца принтера") : string.Empty;

                    return string.Format(
                        "Договор оферты {0} № {1} от {2}",
                        offerType,
                        LatestUserOffer.OfferNumber ?? "{Б/Н}",
                        LatestUserOffer.OfferDate.ToString("dd.MM.yyyy")
                    );
                }
                return string.Empty;
            }
        }
    }
}
