using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Server
{
    public class PrintOrderBll
    {
        public List<PrintOrder> GetUserPrintOrderList(int UserID)
        {
            using (var db = new DB())
            {
                return db.PrintOrders.Where(e => e.UserID == UserID).ToList();
            }
        }

        public List<PrintOrderInfo> GetUserRecievedPrintOrderList(int UserID)
        {
            using (var db = new DB())
            {
                var printOrderList =
                    from PrintOrder in db.PrintOrders
                    join Printer in db.Printers on PrintOrder.PrinterID equals Printer.PrinterID
                    where Printer.UserID == UserID
                    select new PrintOrderInfo() { PrintOrder = PrintOrder, Printer = Printer };
                return printOrderList.ToList();
            }
        }
    }
}
