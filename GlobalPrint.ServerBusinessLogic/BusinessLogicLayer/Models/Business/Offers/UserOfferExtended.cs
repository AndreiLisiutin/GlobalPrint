﻿using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Offers;
using System.Diagnostics;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Offers
{
    /// <summary> 
    /// Extended model for user offer.
    /// </summary>
    public class UserOfferExtended
    {
        [DebuggerStepThrough]
        public UserOfferExtended()
        {
        }

        public UserOffer LatestUserOffer { get; set; }
        public Offer Offer { get; set; }
        public OfferType OfferType { get; set; }

        public bool HasUserOffer
        {
            get
            {
                return this.LatestUserOffer != null;
            }
        }
        public bool HasOffer
        {
            get
            {
                return this.Offer != null;
            }
        }
        public bool HasOfferType
        {
            get
            {
                return this.OfferType != null;
            }
        }

        public string UserOfferString
        {
            get
            {
                string userOfferString = string.Empty;
                string offerType = string.Empty;
                string userOfferCharacteristics = string.Empty;

                #region OfferType

                if (HasOfferType)
                {
                    offerType = this.OfferType.Name;
                }
                else if (HasOffer)
                {
                    switch ((OfferTypeEnum)this.Offer.OfferTypeID)
                    {
                        case OfferTypeEnum.UserOffer:
                            offerType = "Договор оферты пользователя";
                            break;
                        case OfferTypeEnum.PrinterOwnerOffer:
                            offerType = "Договор владельца принтера";
                            break;
                    }
                }

                #endregion

                #region User offer characteristics

                if (HasUserOffer)
                {
                    userOfferCharacteristics = string.Format(
                        "{0} от {1}",
                        string.IsNullOrWhiteSpace(LatestUserOffer.OfferNumber) ? "" : "№ " + LatestUserOffer.OfferNumber,
                        LatestUserOffer.OfferDate.ToString("dd.MM.yyyy")
                    ).Trim();
                }

                #endregion

                return string.Format(
                    "{0} {1}",
                    offerType,
                    userOfferCharacteristics
                ).Trim();
            }
        }
    }
}