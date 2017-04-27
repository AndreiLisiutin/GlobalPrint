using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Orders;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Orders;
using GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext;

namespace GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Orders
{
    /// <summary>
    /// Репозиторий статуса заказа на печать.
    /// </summary>
    public class PrintOrderStatusRepository : BaseRepository<PrintOrderStatus>, IPrintOrderStatusRepository
    {
        public PrintOrderStatusRepository(DbConnectionContext context) : base(context)
        {
        }
    }
}
