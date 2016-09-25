using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Payment;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Payment;
using GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext;

namespace GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Payment
{
    public class PaymentTransactionStatusRepository : BaseRepository<PaymentTransactionStatus>, IPaymentTransactionStatusRepository
    {
        public PaymentTransactionStatusRepository(DbConnectionContext context) : base(context)
        {
        }
    }
}