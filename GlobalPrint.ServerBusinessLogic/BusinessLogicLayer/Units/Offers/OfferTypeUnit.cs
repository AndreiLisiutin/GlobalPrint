using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Offers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Offers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Offers
{
    /// <summary>
    /// Business logic unit of offer type.
    /// </summary>
    public class OfferTypeUnit : BaseUnit
    {
        [DebuggerStepThrough]
        public OfferTypeUnit()
            : base()
        {
        }

        /// <summary>
        /// Get all offer types.
        /// </summary>
        /// <returns>All offer types.</returns>
        public List<OfferType> GetOfferTypes()
        {
            using (IDataContext context = this.Context())
            {
                IOfferTypeRepository offerTypeRepo = this.Repository<IOfferTypeRepository>(context);
                return offerTypeRepo.GetAll().ToList();
            }
        }
    }
}
