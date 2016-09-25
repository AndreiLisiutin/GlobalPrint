using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic.Models.Business.Offers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Offers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Offers
{
    public interface IOfferUnit
    {
        /// <summary>
        /// Get latest offer with specified type.
        /// </summary>
        /// <param name="offerTypeID">Type of offer.</param>
        /// <returns>Latest offer with specified type or null if not exists.</returns>
        Offer GetLatestOfferByType(OfferTypeEnum offerTypeID);
        Offer GetLatestOfferByType(OfferTypeEnum offerTypeID, IDataContext context);

        /// <summary>
        /// Get actual offer of specified type.
        /// </summary>
        /// <param name="offerTypeID">Type of offer.</param>
        /// <returns>Returns actual offer type.</returns>
        Offer GetActualOfferByType(OfferTypeEnum offerTypeID);
    }
}
