using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Offers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Offers;
using GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext;

namespace GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Offers
{
    public class UserOfferRepository : BaseRepository<UserOffer>, IUserOfferRepository
    {
        public UserOfferRepository(DbConnectionContext context) : base(context)
        {
        }
    }
}