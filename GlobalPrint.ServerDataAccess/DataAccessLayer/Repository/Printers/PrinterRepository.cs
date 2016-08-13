using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Printers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Orders;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Printers;
using GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext;
using GlobalPrint.ServerDataAccess.DataAccessLayer.Repository;

namespace GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Printers
{
    public class PrinterRepository : BaseRepository<Printer>, IPrinterRepository
    {
        public PrinterRepository(DbConnectionContext context) : base(context)
        {
        }
    }
}
