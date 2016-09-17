using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Offers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Offers;
using System.Diagnostics;
using System.Linq;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Offers
{
    public class OfferUnit : BaseUnit
    {
        [DebuggerStepThrough]
        public OfferUnit()
            : base()
        {
        }

        /// <summary>
        /// Get latest offer with specified type.
        /// </summary>
        /// <param name="offerTypeID">Type of offer.</param>
        /// <returns>Latest offer with specified type or null if not exists.</returns>
        public Offer GetLatestOfferByType(OfferTypeEnum offerTypeID)
        {
            using (IDataContext context = this.Context())
            {
                return GetLatestOfferByType(offerTypeID, context);
            }
        }
        public Offer GetLatestOfferByType(OfferTypeEnum offerTypeID, IDataContext context)
        {
            IOfferRepository offerRepo = this.Repository<IOfferRepository>(context);

            return offerRepo.Get(e => e.OfferTypeID == (int)offerTypeID)
                .OrderByDescending(e => e.CreatedOn)
                .FirstOrDefault();
        }
    }
}
