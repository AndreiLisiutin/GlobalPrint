using GlobalPrint.ServerBusinessLogic.Models.Business.Orders;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Printers;

namespace GlobalPrint.ClientWeb
{
    public class Order_NewViewModel : Order_NewPostModel
    {
        public Order_NewViewModel()
        {
        }
        public Order_NewViewModel(NewOrder newOrder, Printer printer)
            : base(newOrder)
        {
            this.Printer = printer;
        }

        public Printer Printer { get; set; }
    }
    
    public class Order_NewPostModel
    {
        public Order_NewPostModel()
        {
            this.Order = new NewOrder();
        }
        public Order_NewPostModel(NewOrder newOrder)
        {
            this.Order = newOrder;
        }
        public NewOrder Order { get; set; }
    }
}