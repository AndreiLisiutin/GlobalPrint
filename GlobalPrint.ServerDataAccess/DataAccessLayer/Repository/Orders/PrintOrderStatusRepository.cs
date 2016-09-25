using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Orders;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Orders;
using GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext;

namespace GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Orders
{
    public class PrintOrderStatusRepository : BaseRepository<PrintOrderStatus>, IPrintOrderStatusRepository
    {
        public PrintOrderStatusRepository(DbConnectionContext context) : base(context)
        {
        }
    }
}
