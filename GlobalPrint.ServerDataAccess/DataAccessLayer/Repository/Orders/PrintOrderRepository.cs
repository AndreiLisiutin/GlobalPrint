using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Orders;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Orders;
using GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext;

namespace GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Orders
{
    public class PrintOrderRepository : BaseRepository<PrintOrder>, IPrintOrderRepository
    {
        public PrintOrderRepository(DbConnectionContext context) : base(context)
        {
        }
    }
}