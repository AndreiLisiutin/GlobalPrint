using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Orders;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Printers;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Orders;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Orders;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Printers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Printers
{
    public class PrinterUnit : BaseUnit
    {
        public PrinterUnit()
            : base()
        {
        }

        #region Printer

        public Printer GetPrinterByID(int printerID)
        {
            using (IDataContext context = this.Context())
            {
                return this.Repository<IPrinterRepository>(context)
                    .GetByID(printerID);
            }
        }

        public List<Printer> GetPrinters()
        {
            using (IDataContext context = this.Context())
            {
                return this.Repository<IPrinterRepository>(context)
                    .GetAll()
                    .ToList();
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
            if (printer.OwnerUserID <= 0)
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

            using (IDataContext context = this.Context())
            {
                IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);
                IPrinterScheduleRepository printerScheduleRepo = this.Repository<IPrinterScheduleRepository>(context);

                List<PrinterSchedule> schedules = new List<PrinterSchedule>();
                for (int i = 1; i <= 7; i++)
                {
                    schedules.Add(new PrinterSchedule()
                    {
                        CloseTime = new TimeSpan(18, 0, 0),
                        DayOfWeek = i,
                        OpenTime = new TimeSpan(9, 0, 0),
                        PrinterID = printer.PrinterID
                    });
                }

                context.BeginTransaction();
                try
                {
                    printerRepo.Insert(printer);
                    printerScheduleRepo.Insert(schedules.ToArray());

                    context.Save();
                    context.CommitTransaction();
                }
                catch (Exception ex)
                {
                    context.RollbackTransaction();
                    throw;
                }
                //db.SaveChanges();
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
            if (printer.OwnerUserID <= 0)
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

            using (IDataContext context = this.Context())
            {
                IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);
                var originalPrinter = printerRepo.GetByID(printer.PrinterID);
                if (originalPrinter != null)
                {
                    printerRepo.Update(printer);
                    context.Save();
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

            using (IDataContext context = this.Context())
            {
                IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);
                IPrinterScheduleRepository printerScheduleRepo = this.Repository<IPrinterScheduleRepository>(context);
                var originalPrinter = printerRepo.GetByID(printer.PrinterID);
                if (originalPrinter != null)
                {
                    IQueryable<PrinterSchedule> printSchedules = printerScheduleRepo
                        .Get(e => e.PrinterID == printer.PrinterID);

                    context.BeginTransaction();
                    try
                    {
                        printerScheduleRepo.Delete(printSchedules);
                        printerRepo.Delete(printer);

                        context.Save();
                        context.CommitTransaction();
                    }
                    catch (Exception ex)
                    {
                        context.RollbackTransaction();
                        throw;
                    }
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
            using (IDataContext context = this.Context())
            {
                return this.Repository<IPrinterRepository>(context)
                    .Get(e => e.OwnerUserID == UserID)
                    .ToList();
            }
        }

        #endregion

        #region PrinterOrder

        public PrinterInfo GetPrinterInfoByID(int printerID)
        {
            using (IDataContext context = this.Context())
            {
                IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);
                IPrinterScheduleRepository printerScheduleRepo = this.Repository<IPrinterScheduleRepository>(context);

                var printerInfos = printerRepo.Get(e => e.PrinterID == printerID)
                    .Join(printerScheduleRepo.GetAll(), e => e.PrinterID, e => e.PrinterID,
                        (p, s) => new { printer = p, schedule = s })
                    .ToList();
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
            using (IDataContext context = this.Context())
            {
                return this.Repository<IPrintOrderRepository>(context)
                    .GetByID(printOrderID);
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

            using (IDataContext context = this.Context())
            {
                IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);
                IUserRepository userRepo = this.Repository<IUserRepository>(context);
                IPrintOrderRepository orderRepo = this.Repository<IPrintOrderRepository>(context);

                User client = userRepo.GetByID(order.UserID);
                User printerOwner = printerRepo.Get(e => e.PrinterID == order.PrinterID)
                    .Join(userRepo.GetAll(), e => e.OwnerUserID, e => e.UserID, (p, u) => u)
                    .First();

                context.BeginTransaction();
                try
                {
                    orderRepo.Insert(order);
                    client.AmountOfMoney -= order.PricePerPage;
                    userRepo.Update(client);

                    context.Save();
                    context.CommitTransaction();
                }
                catch (Exception ex)
                {
                    context.RollbackTransaction();
                    throw;
                }



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
