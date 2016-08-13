
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Users;
using GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext;
using GlobalPrint.ServerDataAccess.DataAccessLayer.Repository;

namespace GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Users
{
    /// <summary>
    /// Интерфейс репозитория логов для пользовательских действий
    /// </summary>
    public class UserActionLogRepository : BaseRepository<UserActionLog>, IUserActionLogRepository
    {
        public UserActionLogRepository(DbConnectionContext context) : base(context)
        {
        }
    }
}
