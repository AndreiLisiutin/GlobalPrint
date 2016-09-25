using GlobalPrint.ServerBusinessLogic.Models.Business.Orders;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Orders;

namespace GlobalPrint.ClientWeb
{
    public class Order_ConfirmViewModel
    {
        public PrintOrder PreparedOrder { get; set; }
        public NewOrder NewOrder { get; set; }
    }
}