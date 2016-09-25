using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Orders;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Printers;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Offers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Utilities;
using GlobalPrint.ServerBusinessLogic.Models.Business.Orders;
using GlobalPrint.ServerBusinessLogic.Models.Business.Printers;
using GlobalPrint.ServerBusinessLogic.Models.Domain;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Offers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Orders;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Printers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Printers
{
    public class PrinterUnit : BaseUnit
    {
        [DebuggerStepThrough]
        public PrinterUnit()
            : base()
        {
        }
        
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

        public IEnumerable<PrinterFullInfoModel> GetPrinters(PrinterSearchFilter filter)
        {
            filter = filter ?? new PrinterSearchFilter();
            using (IDataContext context = this.Context())
            {
                IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);
                IPrinterScheduleRepository printerScheduleRepo = this.Repository<IPrinterScheduleRepository>(context);

                List<Printer> printers = printerRepo
                    .Get(e =>
                        (filter.UserID == null || e.OperatorUserID == filter.UserID || e.OwnerUserID == filter.UserID) &&
                        (filter.PrinterID == null || e.ID == filter.PrinterID)
                    ).ToList();
                IEnumerable<int> printerIDs = printers.Select(p => p.ID);

                IEnumerable<PrinterSchedule> schedules = printerScheduleRepo
                    .Get(s => printerIDs.Contains(s.PrinterID))
                    .ToList();
                IEnumerable<PrinterServiceExtended> services = new PrintServicesUnit()
                    .GetPrinterServices(s => printerIDs.Contains(s.PrinterService.PrinterID))
                    .ToList();

                IEnumerable<PrinterFullInfoModel> models = printers
                    .Select(p =>
                    new PrinterFullInfoModel(p,
                        schedules.Where(e => e.PrinterID == p.ID).ToList(),
                        services.Where(e => e.PrinterService.PrinterID == p.ID).ToList()
                    ))
                    .ToList();

                return models;
            }
        }
        public PrinterFullInfoModel GetFullByID(int printerID)
        {
            Argument.Require(printerID > 0, "Ключ принтера должен быть больше 0.");
            PrinterSearchFilter filter = new PrinterSearchFilter()
            {
                PrinterID = printerID
            };
            return this.GetPrinters(filter).FirstOrDefault();
        }

        public PrinterFullInfoModel GetClosestPrinter(float latitude, float longtitude)
        {
            using (IDataContext context = this.Context())
            {
                IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);
                IPrinterScheduleRepository printerScheduleRepo = this.Repository<IPrinterScheduleRepository>(context);

                IQueryable<Printer> all = printerRepo
                        .GetAll();
                Printer closest = printerRepo
                    .GetAll()
                    .Where(p => Math.Pow(p.Latitude - latitude, 2) + Math.Pow(p.Longtitude - longtitude, 2) ==
                        all.Min(e => Math.Pow(e.Latitude - latitude, 2) + Math.Pow(e.Longtitude - longtitude, 2))
                    )
                    .FirstOrDefault();

                IEnumerable<PrinterSchedule> schedules = printerScheduleRepo
                    .Get(s => s.PrinterID == closest.ID)
                    .ToList();
                IEnumerable<PrinterServiceExtended> services = new PrintServicesUnit()
                    .GetPrinterServices(s => s.PrinterService.PrinterID == closest.ID)
                    .ToList();

                PrinterFullInfoModel model = new PrinterFullInfoModel(closest, schedules, services);
                return model;
            }
        }

        /// <summary> Retreive printers which are owned orr operated by user.
        /// </summary>
        /// <param name="userID">Identifier of the user.</param>
        /// <returns>List of printers which belong to the user.</returns>
        public List<Printer> GetUserPrinterList(int userID)
        {
            using (IDataContext context = this.Context())
            {
                return this.Repository<IPrinterRepository>(context)
                    .Get(e => e.OwnerUserID == userID || e.OperatorUserID == userID)
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
                return GetPrinterOperator(printerID, context);
            }
        }
        public User GetPrinterOperator(int printerID, IDataContext context)
        {
            IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);
            IUserRepository userRepo = this.Repository<IUserRepository>(context);

            User printerOwner = printerRepo.Get(e => e.ID == printerID)
                   .Join(userRepo.GetAll(), e => e.OperatorUserID, e => e.UserID, (p, u) => u)
                   .FirstOrDefault();

            if (printerOwner == null)
            {
                printerOwner = printerRepo.Get(e => e.ID == printerID)
                   .Join(userRepo.GetAll(), e => e.OwnerUserID, e => e.UserID, (p, u) => u)
                   .First();
            }

            return printerOwner;
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
                return GetPrinterOwner(printerID, context);
            }
        }
        public User GetPrinterOwner(int printerID, IDataContext context)
        {
            IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);
            IUserRepository userRepo = this.Repository<IUserRepository>(context);

            User printerOwner = printerRepo.Get(e => e.ID == printerID)
                   .Join(userRepo.GetAll(), e => e.OwnerUserID, e => e.UserID, (p, u) => u)
                   .First();
            return printerOwner;
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
                      .Join(printerOrderRepo.GetAll(), e => e.ID, e => e.PrinterID, (p, o) => o)
                      .Where(e => e.PrintOrderStatusID == (int)PrintOrderStatusEnum.Waiting)
                      .Count();
            }
        }

        #endregion

        #region Save

        private void _ValidatePrinter(Printer printer)
        {
            Argument.NotNull(printer, "Модель принтера не может быть пустым.");
            Argument.NotNullOrWhiteSpace(printer.Name, "Название принтера не может быть пустым.");
            Argument.NotNullOrWhiteSpace(printer.Location, "Расположение принтера не может быть пустым.");
            Argument.Positive(printer.OwnerUserID, "Владелец принтера не может быть пустым.");
            Argument.Positive(printer.OperatorUserID, "Оператор принтера не может быть пустым.");
            Argument.Positive(printer.Latitude, "Широта расположения принтера не может быть пустым.");
            Argument.Positive(printer.Longtitude, "Долгота расположения принтера не может быть пустым.");
        }
        private void _ValidatePrinterSchedule(IEnumerable<PrinterSchedule> printerSchedule)
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
        private void _ValidatePrinterServices(IEnumerable<PrinterService> printerServices)
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
            bool isEdit = (model?.Printer?.ID ?? 0) > 0;
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
            int printerOwnerID = model.Printer.OwnerUserID;

            using (IDataContext context = this.Context())
            {
                IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);
                IPrinterScheduleRepository printerScheduleRepo = this.Repository<IPrinterScheduleRepository>(context);
                IPrinterServiceRepository printerServiceRepo = this.Repository<IPrinterServiceRepository>(context);
                UserOfferUnit userOfferUnit = new UserOfferUnit();
                
                var latestPrinterOwnerOffer = new UserOfferUnit().GetLatestUserOfferByUserID(printerOwnerID, OfferTypeEnum.PrinterOwnerOffer);
                bool needCreateOffer = !latestPrinterOwnerOffer.HasUserOffer;

                context.BeginTransaction();
                try
                {
                    printerRepo.Insert(model.Printer);
                    context.Save();

                    // Create user offer
                    if (needCreateOffer)
                    {
                        userOfferUnit.CreateUserOfferInTransaction(printerOwnerID, OfferTypeEnum.PrinterOwnerOffer, context);
                    }

                    foreach (var schedule in model.PrinterSchedule)
                    {
                        schedule.PrinterID = model.Printer.ID;
                    }
                    foreach (var service in model.PrinterServices)
                    {
                        service.PrinterID = model.Printer.ID;
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
                        schedule.PrinterID = model.Printer.ID;
                    }
                    foreach (var service in model.PrinterServices)
                    {
                        service.PrinterID = model.Printer.ID;
                    }
                    printerScheduleRepo.Merge(model.PrinterSchedule, e => e.PrinterID == model.Printer.ID);
                    printerServiceRepo.Merge(model.PrinterServices, e => e.PrinterID == model.Printer.ID);

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

        #endregion

        #region Delete

        /// <summary>
        /// Delete printer validation.
        /// </summary>
        /// <param name="printer">Printer to delete.</param>
        private void _ValidateDeletePrinter(int printerID, IDataContext context)
        {
            Argument.Positive(printerID, "Выберите принтер для удаления.");
            IPrintOrderRepository printOrderRepository = this.Repository<IPrintOrderRepository>(context);

            var printOrderList = printOrderRepository.GetAll();
            Argument.Require(!printOrderList.Any(), "Существуют заказы, связанные с этим принтером.");
        }

        /// <summary>
        /// Delete printer by ID.
        /// </summary>
        /// <param name="printerID">Printer ID.</param>
        public void DeletePrinter(int printerID)
        {
            using (IDataContext context = this.Context())
            {
                this._ValidateDeletePrinter(printerID, context);

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
                catch (Exception)
                {
                    context.RollbackTransaction();
                    throw;
                }
            }
        }

        #endregion
        
    }

}
