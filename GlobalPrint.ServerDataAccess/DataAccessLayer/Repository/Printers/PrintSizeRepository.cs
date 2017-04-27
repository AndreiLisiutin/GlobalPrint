using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Printers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Printers;
using GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext;

namespace GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Printers
{
    /// <summary>
    /// Репозиторий размеров распечатки.
    /// </summary>
    public class PrintSizeRepository : BaseRepository<PrintSize>, IPrintSizeRepository
    {
        public PrintSizeRepository(DbConnectionContext context) : base(context)
        {
        }
    }
}
