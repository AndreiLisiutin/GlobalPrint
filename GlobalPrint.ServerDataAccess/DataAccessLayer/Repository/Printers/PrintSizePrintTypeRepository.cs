using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Printers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Printers;
using GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext;

namespace GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Printers
{
    public class PrintSizePrintTypeRepository : BaseRepository<PrintSizePrintType>, IPrintSizePrintTypeRepository
    {
        public PrintSizePrintTypeRepository(DbConnectionContext context) : base(context)
        {
        }
    }
}
