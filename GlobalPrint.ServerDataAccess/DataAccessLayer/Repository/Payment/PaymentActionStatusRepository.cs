using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Payment;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Payment;
using GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext;

namespace GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Payment
{
    public class PaymentActionStatusRepository : BaseRepository<PaymentActionStatus>, IPaymentActionStatusRepository
    {
        public PaymentActionStatusRepository(DbConnectionContext context) : base(context)
        {
        }
    }
}