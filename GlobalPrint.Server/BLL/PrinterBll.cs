using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Server
{
    public class PrinterBll
    {
        public class PrinterScheduleJoin
        {
            public Printer printer { get; set; }
            public PrinterSchedule schedule { get; set; }
        }
        public Printer GetPrinterByID(int printerID)
        {
            using (var db = new DB())
            {
                return db.Printers.First(e => e.PrinterID == printerID);
            }
        }
        public PrinterInfo GetPrinterInfoByID(int printerID)
        {
            using (var db = new DB())
            {
                var printerInfos = db.Printers.Join(db.PrintSchedules, e => e.PrinterID, e => e.PrinterID,
                    (p, s) => new PrinterScheduleJoin() { printer = p, schedule = s }).ToList();
                if (printerInfos.Count == 0)
                {
                    throw new Exception("Не найден принтер или его расписание работы");
                }

                PrinterInfo info = new PrinterInfo()
                {
                    Printer = printerInfos.First().printer,
                    Schedule = printerInfos.Select(e => e.schedule).ToList()
                };
                return info;
            }
        }
        public PrintOrder GetPrintOrderByID(int printOrderID)
        {
            using (var db = new DB())
            {
                return db.PrintOrders.First(e => e.PrintOrderID == printOrderID);
            }
        }
        public PrintOrder SavePrintOrder(byte[] fileToPrint, PrintOrder order, SmsUtility.Parameters smsParams)
        {
            FileInfo file = new FileInfo(order.Document);
            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }
            File.WriteAllBytes(order.Document, fileToPrint);

            using (var db = new DB())
            {
                var client = db.Users.First(e => e.UserID == order.UserID);
                var printerOwner = db.Printers.Where(e => e.PrinterID == order.PrinterID)
                    .Join(db.Users, e => e.UserID, e => e.UserID, (p, u) => u)
                    .First();

                db.PrintOrders.Add(order);
                client.AmountOfMoney -= order.Price;
                db.SaveChanges();

                if (false && !string.IsNullOrEmpty(printerOwner.Phone))
                {
                    string message = "Поступил новый заказ №" + order.PrintOrderID + ".";
                    new SmsUtility(smsParams).Send(printerOwner.Phone, message);
                }
                return order;
            }
        }

        public List<Printer> GetUserPrinterList(int UserID)
        {
            using (var db = new DB())
            {
                return db.Printers.Where(e => e.UserID == UserID).ToList();
            }
        }
    }
}
