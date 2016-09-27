using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext;

namespace GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Users
{
    public class UserActionTypeRepository : BaseRepository<UserActionType>, IUserActionTypeRepository
    {
        public UserActionTypeRepository(DbConnectionContext context) : base(context)
        {
        }
    }
}
