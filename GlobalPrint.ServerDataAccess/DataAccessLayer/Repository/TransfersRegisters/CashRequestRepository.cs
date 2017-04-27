using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Printers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Printers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.TransfersRegisters;
using GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext;

namespace GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.TransfersRegisters
{
    public class CashRequestRepository : BaseRepository<CashRequest>, ICashRequestRepository
    {
        public CashRequestRepository(DbConnectionContext context) : base(context)
        {
        }
    }
}
