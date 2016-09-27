using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Offers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Offers;
using GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext;

namespace GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Offers
{
    public class OfferTypeRepository : BaseRepository<OfferType>, IOfferTypeRepository
    {
        public OfferTypeRepository(DbConnectionContext context) : base(context)
        {
        }
    }
}