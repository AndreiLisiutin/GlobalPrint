using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Payment;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Payment;
using GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext;

namespace GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Payment
{
    public class PaymentActionTypeRepository : BaseRepository<PaymentActionType>, IPaymentActionTypeRepository
    {
        public PaymentActionTypeRepository(DbConnectionContext context) : base(context)
        {
        }
    }
}