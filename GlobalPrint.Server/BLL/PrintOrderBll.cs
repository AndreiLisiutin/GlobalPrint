using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Server
{
    public class PrintOrderBll
    {
        public List<PrintOrderInfo> GetUserPrintOrderList(int UserID)
        {
            using (var db = new DB())
            {
                var printOrderList =
                    from PrintOrder in db.PrintOrders
                    join Printer in db.Printers on PrintOrder.PrinterID equals Printer.PrinterID
                    join Status in db.PrintOrderStatuses on PrintOrder.PrintOrderStatusID equals Status.PrintOrderStatusID
                    where PrintOrder.UserID == UserID
                    orderby PrintOrder.OrderedOn descending
                    select new PrintOrderInfo() { PrintOrder = PrintOrder, Printer = Printer, Status = Status };
                return printOrderList.ToList();
            }
        }
        public PrintOrderInfo GetPrintOrderInfoByID(int printOrderID)
        {
            using (var db = new DB())
            {
                var printOrderList =
                    from PrintOrder in db.PrintOrders
                    join Printer in db.Printers on PrintOrder.PrinterID equals Printer.PrinterID
                    join Status in db.PrintOrderStatuses on PrintOrder.PrintOrderStatusID equals Status.PrintOrderStatusID
                    where PrintOrder.PrintOrderID == printOrderID
                    orderby PrintOrder.OrderedOn descending
                    select new PrintOrderInfo() { PrintOrder = PrintOrder, Printer = Printer, Status = Status };
                return printOrderList.First();
            }
        }

        public List<PrintOrderInfo> GetUserRecievedPrintOrderList(int UserID)
        {
            using (var db = new DB())
            {
                var printOrderList =
                    from PrintOrder in db.PrintOrders
                    join Printer in db.Printers on PrintOrder.PrinterID equals Printer.PrinterID
                    join Status in db.PrintOrderStatuses on PrintOrder.PrintOrderStatusID equals Status.PrintOrderStatusID
                    where Printer.UserID == UserID
                    orderby PrintOrder.OrderedOn descending
                    select new PrintOrderInfo() { PrintOrder = PrintOrder, Printer = Printer, Status = Status };
                return printOrderList.ToList();
            }
        }

        public void UpdateStatus(int printOrderID, PrintOrderStatusEnum statusID, SmsUtility.Parameters smsParams)
        {
            PrintOrderStatus status = null;
            User client = null;
            User printerOwner = null;
            PrintOrder order = null;
            using (var db = new DB())
            {
                order = db.PrintOrders.First(e => e.PrintOrderID == printOrderID);
                client = db.Users.First(e => e.UserID == order.UserID);
                printerOwner = db.Printers.Where(e => e.PrinterID == order.PrinterID)
                    .Join(db.Users, e => e.UserID, e => e.UserID, (p, u) => u).
                    First();
                if (order.PrintOrderStatusID == (int)statusID)
                {
                    return;
                }
                order.PrintOrderStatusID = (int)statusID;
                if (statusID == PrintOrderStatusEnum.Printed && order.PrintedOn == null)
                {
                    order.PrintedOn = DateTime.Now;
                    printerOwner.AmountOfMoney += order.Price;
                }
                else if (statusID == PrintOrderStatusEnum.Rejected)
                {
                    client.AmountOfMoney += order.Price;
                }
                status = db.PrintOrderStatuses.First(e => e.PrintOrderStatusID == (int)statusID);

                db.SaveChanges();
            }

            if (!string.IsNullOrEmpty(client.PhoneNumber))
            {
                new SmsUtility(smsParams).Send(client.PhoneNumber, "Заказ №" + order.PrintOrderID + " " + status.Status.ToLower());
            }
        }
    }
}
