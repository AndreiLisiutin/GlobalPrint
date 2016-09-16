using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Offers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Offers;
using GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext;

namespace GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Offers
{
    public class OfferRepository : BaseRepository<Offer>, IOfferRepository
    {
        public OfferRepository(DbConnectionContext context) : base(context)
        {
        }
    }
}