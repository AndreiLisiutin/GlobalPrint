using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Printers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Printers;
using GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext;

namespace GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Printers
{
    /// <summary>
    /// Репозиторий типов распечатки.
    /// </summary>
    public class PrintTypeRepository : BaseRepository<PrintType>, IPrintTypeRepository
    {
        public PrintTypeRepository(DbConnectionContext context) : base(context)
        {
        }
    }
}
