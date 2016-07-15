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

        #region Printer

        public Printer GetPrinterByID(int printerID)
        {
            using (var db = new DB())
            {
                return db.Printers.First(e => e.PrinterID == printerID);
            }
        }
        public Printer AddPrinter(Printer printer)
        {
            if (string.IsNullOrEmpty(printer.Name))
            {
                throw new Exception("Не указано название принтера");
            }
            if (string.IsNullOrEmpty(printer.Location))
            {
                throw new Exception("Не указано расположение принтера");
            }
            if (printer.UserID <= 0)
            {
                throw new Exception("Не указан владелец принтера");
            }
            if (printer.Latitude <= 0)
            {
                throw new Exception("Не указана долгота расположения принтера");
            }
            if (printer.Longtitude <= 0)
            {
                throw new Exception("Не указана широта расположения принтера");
            }
            if (string.IsNullOrEmpty(printer.Phone))
            {
                throw new Exception("Не указан телефон принтера");
            }
            if (printer.BlackWhitePrintPrice <= 0)
            {
                throw new Exception("Не указана стоимость распечатки");
            }



            using (var db = new DB())
            {
                db.Printers.Add(printer);
                db.SaveChanges();
                List<PrinterSchedule> schedule = new List<PrinterSchedule>();
                for (int i = 1; i <= 7; i++)
                {
                    schedule.Add(new PrinterSchedule()
                    {
                        CloseTime = new TimeSpan(18, 0, 0),
                        DayOfWeek = i,
                        OpenTime = new TimeSpan(9, 0, 0),
                        PrinterID = printer.PrinterID
                    });
                }
                db.PrintSchedules.AddRange(schedule);
                db.SaveChanges();
                return printer;
            }
        }
        public Printer EditPrinter(Printer printer)
        {
            if (string.IsNullOrEmpty(printer.Name))
            {
                throw new Exception("Не указано название принтера");
            }
            if (string.IsNullOrEmpty(printer.Location))
            {
                throw new Exception("Не указано расположение принтера");
            }
            if (printer.UserID <= 0)
            {
                throw new Exception("Не указан владелец принтера");
            }
            if (printer.Latitude <= 0)
            {
                throw new Exception("Не указана долгота расположения принтера");
            }
            if (printer.Longtitude <= 0)
            {
                throw new Exception("Не указана широта расположения принтера");
            }
            if (string.IsNullOrEmpty(printer.Phone))
            {
                throw new Exception("Не указан телефон принтера");
            }
            if (printer.BlackWhitePrintPrice <= 0)
            {
                throw new Exception("Не указана стоимость распечатки");
            }

            using (var db = new DB())
            {
                var original = db.Printers.SingleOrDefault(x => x.PrinterID == printer.PrinterID);
                if (original != null)
                {
                    db.Entry(original).CurrentValues.SetValues(printer);
                    db.SaveChanges();
                }
                else
                {
                    throw new Exception("Не найден принтер");
                }
                return printer;
            }
        }

        public Printer DelPrinter(Printer printer)
        {

            using (var db = new DB())
            {
                var original = db.Printers.SingleOrDefault(x => x.PrinterID == printer.PrinterID);
                if (original != null)
                {
                    var printSchedules = db.PrintSchedules.Where(e => e.PrinterID == printer.PrinterID).ToList();


                    db.PrintSchedules.RemoveRange(printSchedules);
                    db.SaveChanges();
                    db.Printers.Remove(original);
                    db.SaveChanges();
                }
                else
                {
                    throw new Exception("Не найден принтер");
                }
                return printer;
            }
        }


        public List<Printer> GetUserPrinterList(int UserID)
        {
            using (var db = new DB())
            {
                return db.Printers.Where(e => e.UserID == UserID).ToList();
            }
        }

        #endregion

        #region PrinterOrder

        public PrinterInfo GetPrinterInfoByID(int printerID)
        {
            using (var db = new DB())
            {
                var printerInfos = db.Printers.Where(e => e.PrinterID == printerID)
                    .Join(db.PrintSchedules, e => e.PrinterID, e => e.PrinterID,
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

                if (!string.IsNullOrEmpty(printerOwner.PhoneNumber))
                {
                    string message = "Поступил новый заказ №" + order.PrintOrderID + ".";
                    new SmsUtility(smsParams).Send(printerOwner.PhoneNumber, message);
                }
                return order;
            }
        }

        #endregion

    }
}
