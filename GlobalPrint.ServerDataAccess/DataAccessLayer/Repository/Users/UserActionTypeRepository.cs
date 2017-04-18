using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext;

namespace GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Users
{
    /// <summary>
    /// Репозиторий типов пользовательских действий.
    /// </summary>
    public class UserActionTypeRepository : BaseRepository<UserActionType>, IUserActionTypeRepository
    {
        public UserActionTypeRepository(DbConnectionContext context) : base(context)
        {
        }
    }
}
