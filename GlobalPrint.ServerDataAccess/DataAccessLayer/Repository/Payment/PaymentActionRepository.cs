using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Payment;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Payment;
using GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext;

namespace GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Payment
{
    public class PaymentActionRepository : BaseRepository<PaymentAction>, IPaymentActionRepository
    {
        public PaymentActionRepository(DbConnectionContext context) : base(context)
        {
        }
    }
}