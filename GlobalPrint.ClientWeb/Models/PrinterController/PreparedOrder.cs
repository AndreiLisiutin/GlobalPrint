using GlobalPrint.ServerBusinessLogic.Models.Domain.Orders;

namespace GlobalPrint.ClientWeb
{
    public class PreparedOrder
    {
        public PrintOrder order { get; set; }
        public byte[] serializedFile { get; set; }
    }
}