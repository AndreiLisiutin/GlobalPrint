using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext;

namespace GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Users
{
    /// <summary>
    /// Репозиторий логов пользовательских действий.
    /// </summary>
    public class UserActionLogRepository : BaseRepository<UserActionLog>, IUserActionLogRepository
    {
        public UserActionLogRepository(DbConnectionContext context) : base(context)
        {
        }
    }
}
