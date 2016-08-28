using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Orders;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Printers;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Orders;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Printers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Orders;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Printers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Printers
{
    public class PrinterUnit : BaseUnit
    {
        public PrinterUnit()
            : base()
        {
        }

        #region Printer

        #region Get

        public Printer GetPrinterByID(int printerID)
        {
            using (IDataContext context = this.Context())
            {
                return this.Repository<IPrinterRepository>(context)
                    .GetByID(printerID);
            }
        }

        public PrinterEditionModel GetPrinterEditionModel(int printerID)
        {
            using (IDataContext context = this.Context())
            {
                IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);
                IPrinterScheduleRepository printerScheduleRepo = this.Repository<IPrinterScheduleRepository>(context);
                IPrinterServiceRepository printerServiceRepo = this.Repository<IPrinterServiceRepository>(context);

                Printer printer = printerRepo.GetByID(printerID);
                IEnumerable<PrinterSchedule> schedule = printerScheduleRepo
                    .Get(e => e.PrinterID == printerID)
                    .ToList();
                IEnumerable<PrinterService> services = printerServiceRepo
                    .Get(e => e.PrinterID == printerID)
                    .ToList();

                PrinterEditionModel model = this.CreatePrinterEditionModel();
                model.Printer = printer;
                model.PrinterSchedule = schedule;
                model.PrinterServices = services;

                return model;
            }
        }

        public PrinterEditionModel CreatePrinterEditionModel()
        {
            PrinterEditionModel model = new PrinterEditionModel();
            return model;
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

        public List<Printer> GetUserPrinterList(int UserID)
        {
            using (IDataContext context = this.Context())
            {
                return this.Repository<IPrinterRepository>(context)
                    .Get(e => e.OwnerUserID == UserID)
                    .ToList();
            }
        }

        /// <summary>
        /// Get operator of chosen printer
        /// </summary>
        /// <param name="printerID">Printer identifier</param>
        /// <returns>User instance for printer operator or owner (if operator doesn't exists)</returns>
        public User GetPrinterOperator(int printerID)
        {
            using (IDataContext context = this.Context())
            {
                IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);
                IUserRepository userRepo = this.Repository<IUserRepository>(context);

                User printerOwner = printerRepo.Get(e => e.PrinterID == printerID)
                       .Join(userRepo.GetAll(), e => e.OperatorUserID, e => e.UserID, (p, u) => u)
                       .FirstOrDefault();

                if (printerOwner == null)
                {
                    printerOwner = printerRepo.Get(e => e.PrinterID == printerID)
                       .Join(userRepo.GetAll(), e => e.OwnerUserID, e => e.UserID, (p, u) => u)
                       .First();
                }

                return printerOwner;
            }
        }

        /// <summary>
        /// Get owner of chosen printer
        /// </summary>
        /// <param name="printerID">Printer identifier</param>
        /// <returns>User instance for printer owner</returns>
        public User GetPrinterOwner(int printerID)
        {
            using (IDataContext context = this.Context())
            {
                IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);
                IUserRepository userRepo = this.Repository<IUserRepository>(context);

                User printerOwner = printerRepo.Get(e => e.PrinterID == printerID)
                       .Join(userRepo.GetAll(), e => e.OwnerUserID, e => e.UserID, (p, u) => u)
                       .First();
                return printerOwner;
            }
        }

        /// <summary>
        /// Get waiting orders count of specified user
        /// </summary>
        /// <param name="userID">User identifier (printer owner/operator)</param>
        /// <returns>Number of waiting orders, not processed by current user</returns>
        public int GetWaitingIncomingOrdersCount(int userID)
        {
            using (IDataContext context = this.Context())
            {
                IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);
                IPrintOrderRepository printerOrderRepo = this.Repository<IPrintOrderRepository>(context);

                return printerRepo.Get(e => e.OperatorUserID == userID)
                      .Join(printerOrderRepo.GetAll(), e => e.PrinterID, e => e.PrinterID, (p, o) => o)
                      .Where(e => e.PrintOrderStatusID == (int)PrintOrderStatusEnum.Waiting)
                      .Count();
            }
        }

        #endregion

        #region Save

        public void _ValidatePrinter(Printer printer)
        {
            Argument.NotNull(printer, "Модель принтера не может быть пустым.");
            Argument.NotNullOrWhiteSpace(printer.Name, "Название принтера не может быть пустым.");
            Argument.NotNullOrWhiteSpace(printer.Location, "Расположение принтера не может быть пустым.");
            Argument.Positive(printer.OwnerUserID, "Владелец принтера не может быть пустым.");
            Argument.Positive(printer.OperatorUserID, "Оператор принтера не может быть пустым.");
            Argument.Positive(printer.Latitude, "Широта расположения принтера не может быть пустым.");
            Argument.Positive(printer.Longtitude, "Долгота расположения принтера не может быть пустым.");
        }
        public void _ValidatePrinterSchedule(IEnumerable<PrinterSchedule> printerSchedule)
        {
            IEnumerable<int> allDays = Enum.GetValues(typeof(DayOfWeek))
                .Cast<DayOfWeek>()
                .Select(e => (int)e);

            Argument.NotNull(printerSchedule, "Расписание принтера не может быть пустым.");
            Argument.Require(printerSchedule.Count() > 0, "Расписание принтера не может быть пустым.");

            foreach (PrinterSchedule day in printerSchedule)
            {
                Argument.Require(allDays.Contains(day.DayOfWeek), "День расписания принтера не может быть пустым.");
                Argument.Require(day.OpenTime >= TimeSpan.FromHours(0) && day.OpenTime <= TimeSpan.FromHours(24),
                    "Начало работы в расписании работы принтера должен быть в промежутке от 0:00 до 24:00.");
                Argument.Require(day.CloseTime >= TimeSpan.FromHours(0) && day.OpenTime <= TimeSpan.FromHours(24),
                    "Начало работы в расписании работы принтера должен быть в промежутке от 0:00 до 24:00.");
            }

            bool periodsAreIntersected = printerSchedule
                .Join(printerSchedule, ps1 => true, ps2 => true, (ps1, ps2) => new { ps1, ps2 })
                .Where(e => e.ps1.DayOfWeek == e.ps2.DayOfWeek
                    && e.ps1 != e.ps2
                    && e.ps1.OpenTime < e.ps2.CloseTime
                    && e.ps1.CloseTime > e.ps2.CloseTime
                )
                .Any();

            Argument.Require(!periodsAreIntersected, "Расписание принтера не должно пересекаться.");
        }
        public void _ValidatePrinterServices(IEnumerable<PrinterService> printerServices)
        {
            Argument.NotNull(printerServices, "Список сервисов принтера не может быть пустым.");
            Argument.Require(printerServices.Count() > 0, "Список сервисов принтера не может быть пустым.");
            Argument.Require(printerServices.Select(e => e.PrintServiceID).Distinct().Count() == printerServices.Count()
                , "Сервисы принтера должны быть уникальны.");
            foreach (PrinterService service in printerServices)
            {
                Argument.Positive(service.PricePerPage, "Цены на услуги принтера должны быть положительными.");
                Argument.Positive(service.PrintServiceID, "Услуга для принтера не может быть неопределенной.");
            }
        }

        public PrinterEditionModel SavePrinter(PrinterEditionModel model)
        {
            bool isEdit = (model?.Printer.PrinterID ?? 0) > 0;
            if (isEdit)
            {
                return this.EditPrinter(model);
            }
            else
            {
                return this.CreatePrinter(model);
            }
        }

        public PrinterEditionModel CreatePrinter(PrinterEditionModel model)
        {
            Argument.NotNull(model, "Значение NULL невозможно сохранить как принтер.");
            this._ValidatePrinter(model.Printer);
            this._ValidatePrinterSchedule(model.PrinterSchedule);
            this._ValidatePrinterServices(model.PrinterServices);

            using (IDataContext context = this.Context())
            {
                IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);
                IPrinterScheduleRepository printerScheduleRepo = this.Repository<IPrinterScheduleRepository>(context);
                IPrinterServiceRepository printerServiceRepo = this.Repository<IPrinterServiceRepository>(context);

                context.BeginTransaction();
                try
                {
                    printerRepo.Insert(model.Printer);
                    context.Save();

                    foreach (var schedule in model.PrinterSchedule)
                    {
                        schedule.PrinterID = model.Printer.PrinterID;
                    }
                    foreach (var service in model.PrinterServices)
                    {
                        service.PrinterID = model.Printer.PrinterID;
                    }
                    printerScheduleRepo.Insert(model.PrinterSchedule);
                    printerServiceRepo.Insert(model.PrinterServices);

                    context.Save();
                    context.CommitTransaction();
                }
                catch (Exception)
                {
                    context.RollbackTransaction();
                    throw;
                }
                return model;
            }
        }

        public PrinterEditionModel EditPrinter(PrinterEditionModel model)
        {
            Argument.NotNull(model, "Значение NULL невозможно сохранить как принтер.");
            this._ValidatePrinter(model.Printer);
            this._ValidatePrinterSchedule(model.PrinterSchedule);
            this._ValidatePrinterServices(model.PrinterServices);

            using (IDataContext context = this.Context())
            {
                IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);
                IPrinterScheduleRepository printerScheduleRepo = this.Repository<IPrinterScheduleRepository>(context);
                IPrinterServiceRepository printerServiceRepo = this.Repository<IPrinterServiceRepository>(context);

                context.BeginTransaction();
                try
                {
                    printerRepo.Update(model.Printer);

                    foreach (var schedule in model.PrinterSchedule)
                    {
                        schedule.PrinterID = model.Printer.PrinterID;
                    }
                    foreach (var service in model.PrinterServices)
                    {
                        service.PrinterID = model.Printer.PrinterID;
                    }
                    printerScheduleRepo.Merge(model.PrinterSchedule, e => e.PrinterID == model.Printer.PrinterID);
                    printerServiceRepo.Merge(model.PrinterServices, e => e.PrinterID == model.Printer.PrinterID);

                    context.Save();
                    context.CommitTransaction();
                }
                catch (Exception ex)
                {
                    context.RollbackTransaction();
                    throw;
                }
                return model;
            }
        }

        #endregion

        #region Delete

        public void DeletePrinter(int printerID)
        {

            using (IDataContext context = this.Context())
            {
                IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);
                IPrinterScheduleRepository printerScheduleRepo = this.Repository<IPrinterScheduleRepository>(context);
                IPrinterServiceRepository printerServiceRepo = this.Repository<IPrinterServiceRepository>(context);
                var originalPrinter = printerRepo.GetByID(printerID);
                if (originalPrinter == null)
                {
                    return;
                }

                IQueryable<PrinterSchedule> printSchedules = printerScheduleRepo
                    .Get(e => e.PrinterID == printerID);
                IQueryable<PrinterService> printerServices = printerServiceRepo
                    .Get(e => e.PrinterID == printerID);

                context.BeginTransaction();
                try
                {
                    printerScheduleRepo.Delete(printSchedules);
                    printerServiceRepo.Delete(printerServices);
                    context.Save();
                    printerRepo.Delete(printerID);

                    context.Save();
                    context.CommitTransaction();
                }
                catch (Exception ex)
                {
                    context.RollbackTransaction();
                    throw;
                }
            }
        }

        #endregion

        #endregion

        #region PrinterOrder

        public PrinterScheduled GetPrinterInfoByID(int printerID)
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

                PrinterScheduled info = new PrinterScheduled()
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
