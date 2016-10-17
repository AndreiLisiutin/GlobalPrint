using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Orders;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Printers;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Utilities;
using GlobalPrint.ServerBusinessLogic.Models.Business.Orders;
using GlobalPrint.ServerBusinessLogic.Models.Business.Printers;
using GlobalPrint.ServerBusinessLogic.Models.Domain;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Orders;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Printers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Configuration;

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

        public Printer GetByID(int printerID)
        {
            using (IDataContext context = this.Context())
            {
                return this.Repository<IPrinterRepository>(context)
                    .GetByID(printerID);
            }
        }

        public PrinterEditionModel GetPrinterEditionModel(int printerID, int userID)
        {
            using (IDataContext context = this.Context())
            {
                IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);
                IPrinterScheduleRepository printerScheduleRepo = this.Repository<IPrinterScheduleRepository>(context);
                IPrinterServiceRepository printerServiceRepo = this.Repository<IPrinterServiceRepository>(context);

                Printer printer = printerRepo.GetByID(printerID);
                Argument.Require(userID == printer.OperatorUserID || userID ==  printer.OwnerUserID, 
                    "Редактировать принтер может только хозяин или оператор.");

                IEnumerable<PrinterSchedule> schedule = printerScheduleRepo
                    .Get(e => e.PrinterID == printerID)
                    .ToList();
                IEnumerable<PrinterService> services = printerServiceRepo
                    .Get(e => e.PrinterID == printerID)
                    .ToList();

                PrinterEditionModel model = new PrinterEditionModel();
                model.Printer = printer;
                model.PrinterSchedule = schedule;
                model.PrinterServices = services;

                return model;
            }
        }

        public IEnumerable<PrinterFullInfoModel> GetFullByFilter(PrinterSearchFilter filter)
        {
            filter = filter ?? new PrinterSearchFilter();
            using (IDataContext context = this.Context())
            {
                return this.GetFullByFilter(filter, context);
            }
        }
        public IEnumerable<PrinterFullInfoModel> GetFullByFilter(PrinterSearchFilter filter, IDataContext context)
        {
            Argument.NotNull(context, "Контекст подключения к базе данных не может быть пустым.");
            filter = filter ?? new PrinterSearchFilter();
            IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);
            IPrinterScheduleRepository printerScheduleRepo = this.Repository<IPrinterScheduleRepository>(context);
            IUserRepository userRepo = this.Repository<IUserRepository>(context);

            var printers =
            (
                from printer in printerRepo.Get(e =>
                    (filter.UserID == null || e.OperatorUserID == filter.UserID || e.OwnerUserID == filter.UserID) &&
                    (filter.PrinterID == null || e.ID == filter.PrinterID) &&
                    (filter.IsDisabled == null || e.IsDisabled == filter.IsDisabled)
                )
                join @operator in userRepo.GetAll() on printer.OperatorUserID equals @operator.ID
                select new { printer = printer, @operator = @operator }
            )
                .ToList();
            IEnumerable<int> printerIDs = printers.Select(p => p.printer.ID);

            IEnumerable<PrinterSchedule> schedules = printerScheduleRepo
                .Get(s => printerIDs.Contains(s.PrinterID))
                .ToList();
            IEnumerable<PrinterServiceExtended> services = new PrintServicesUnit()
                .GetPrinterServices(s => printerIDs.Contains(s.PrinterService.PrinterID))
                .ToList();

            IEnumerable<PrinterFullInfoModel> models = printers
                .Select(p =>
                new PrinterFullInfoModel(p.printer,
                    p.@operator,
                    schedules.Where(e => e.PrinterID == p.printer.ID).ToList(),
                    services.Where(e => e.PrinterService.PrinterID == p.printer.ID)
                        .OrderBy(e => e.PrintService.PrintType.Name)
                        .ThenBy(e => e.PrintService.PrintSize.Name)
                        .ThenBy(e => e.PrintService.PrintService.IsColored)
                        .ThenBy(e => e.PrintService.PrintService.IsTwoSided)
                        .ToList()
                ))
                .ToList();

            return models;
        }
        public PrinterFullInfoModel GetFullByID(int printerID, IDataContext context)
        {
            Argument.Require(printerID > 0, "Ключ принтера должен быть больше 0.");
            PrinterSearchFilter filter = new PrinterSearchFilter()
            {
                PrinterID = printerID
            };
            return this.GetFullByFilter(filter, context)
                .FirstOrDefault();
        }
        public PrinterFullInfoModel GetFullByID(int printerID)
        {
            Argument.Require(printerID > 0, "Ключ принтера должен быть больше 0.");
            PrinterSearchFilter filter = new PrinterSearchFilter()
            {
                PrinterID = printerID
            };
            return this.GetFullByFilter(filter)
                .FirstOrDefault();
        }

        public PrinterFullInfoModel GetClosest(float latitude, float longtitude)
        {
            using (IDataContext context = this.Context())
            {
                IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);
                IPrinterScheduleRepository printerScheduleRepo = this.Repository<IPrinterScheduleRepository>(context);
                IUserRepository userRepo = this.Repository<IUserRepository>(context);

                int today = (int)DateTime.Now.DayOfWeek;
                TimeSpan now = DateTime.Now.TimeOfDay;
                TimeSpan threshold = TimeSpan.FromMinutes(double.Parse(WebConfigurationManager.AppSettings["ActivityCheckerThreshold"]));
                DateTime activityOkDate = DateTime.Now.Subtract(threshold);

                var printers =
                (
                    from printer in printerRepo.Get(e => !e.IsDisabled)
                    join schedule in printerScheduleRepo.Get(e => e.DayOfWeek == today && now >= e.OpenTime && now <= e.CloseTime)
                        on printer.ID equals schedule.PrinterID
                    join @operator in userRepo.Get(o => o.LastActivityDate > activityOkDate) on printer.OperatorUserID equals @operator.ID
                    select new { printer = printer, @operator = @operator, range = Math.Pow(printer.Latitude - latitude, 2) + Math.Pow(printer.Longtitude - longtitude, 2) }
                );

                var closest = printers.
                    Where(e => e.range == printers.Min(x => x.range))
                    .FirstOrDefault();
                
                if (closest == null)
                {
                    return null;
                }
                
                IEnumerable<PrinterSchedule> schedules = printerScheduleRepo
                    .Get(s => s.PrinterID == closest.printer.ID)
                    .ToList();
                IEnumerable<PrinterServiceExtended> services = new PrintServicesUnit()
                    .GetPrinterServices(s => s.PrinterService.PrinterID == closest.printer.ID)
                    .ToList();

                PrinterFullInfoModel model = new PrinterFullInfoModel(closest.printer, closest.@operator, schedules, services);
                return model;
            }
        }

        /// <summary> Retrieve printers which are owned orr operated by user.
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

            User @operator = printerRepo.Get(e => e.ID == printerID)
                   .Join(userRepo.GetAll(), e => e.OperatorUserID, e => e.ID, (p, u) => u)
                   .FirstOrDefault();

            return @operator;
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

        private Validation _ValidatePrinter(Printer printer)
        {
            Validation validation = new Validation();

            validation.NotNull(printer, "Модель принтера не может быть пустым.");
            validation.NotNullOrWhiteSpace(printer.Name, "Название принтера не может быть пустым.");
            validation.NotNullOrWhiteSpace(printer.Location, "Расположение принтера не может быть пустым.");
            validation.Positive(printer.OwnerUserID, "Владелец принтера не может быть пустым.");
            validation.Positive(printer.OperatorUserID, "Оператор принтера не может быть пустым.");
            validation.Positive(printer.Latitude, "Широта расположения принтера не может быть пустой.");
            validation.Positive(printer.Longtitude, "Долгота расположения принтера не может быть пустой.");

            return validation;
        }
        private Validation _ValidatePrinterSchedule(IEnumerable<PrinterSchedule> printerSchedule)
        {
            Validation validation = new Validation();

            IEnumerable<int> allDays = Enum.GetValues(typeof(DayOfWeek))
                .Cast<DayOfWeek>()
                .Select(e => (int)e);

            validation.NotNull(printerSchedule, "Расписание принтера не может быть пустым.");
            validation.Require(printerSchedule.Count() > 0, "Расписание принтера не может быть пустым.");

            foreach (PrinterSchedule day in printerSchedule)
            {
                validation.Require(allDays.Contains(day.DayOfWeek), "День расписания принтера некорректно задан.");
                validation.Require(day.OpenTime >= TimeSpan.FromHours(0) && day.OpenTime <= TimeSpan.FromHours(24),
                    "Начало работы в расписании работы принтера должен быть в промежутке от 0:00 до 24:00.");
                validation.Require(day.CloseTime >= TimeSpan.FromHours(0) && day.OpenTime <= TimeSpan.FromHours(24),
                    "Начало работы в расписании работы принтера должен быть в промежутке от 0:00 до 24:00.");
                validation.Require(day.CloseTime >= day.OpenTime,
                    "Начало работы в расписании работы принтера не может быть позже конца работы.");
            }

            bool periodsAreIntersected = printerSchedule
                .Join(printerSchedule, ps1 => true, ps2 => true, (ps1, ps2) => new { ps1, ps2 })
                .Where(e => e.ps1.DayOfWeek == e.ps2.DayOfWeek
                    && e.ps1 != e.ps2
                    && e.ps1.OpenTime < e.ps2.CloseTime
                    && e.ps1.CloseTime > e.ps2.CloseTime
                )
                .Any();

            validation.Require(!periodsAreIntersected, "Расписание принтера не должно пересекаться.");

            return validation;
        }
        private Validation _ValidatePrinterServices(IEnumerable<PrinterService> printerServices)
        {
            Validation validation = new Validation();

            validation.NotNull(printerServices, "Список сервисов принтера не может быть пустым.");
            validation.Require(printerServices.Count() > 0, "Список сервисов принтера не может быть пустым.");
            validation.Require(printerServices.Select(e => e.PrintServiceID).Distinct().Count() == printerServices.Count()
                , "Сервисы принтера должны быть уникальны.");
            foreach (PrinterService service in printerServices)
            {
                validation.Positive(service.PricePerPage, "Цены на услуги принтера должны быть положительными.");
                validation.Positive(service.PrintServiceID, "Услуга для принтера не может быть неопределенной.");
            }

            return validation;
        }

        /// <summary>
        /// Validate printer save model.
        /// </summary>
        /// <param name="model">Printer save model to validate.</param>
        /// <returns>Validation results for printer.</returns>
        public Validation Validate(PrinterEditionModel model)
        {
            Validation validation = new Validation();
            validation.NotNull(model, "Модель для сохранения принтера пустая.");

            validation.Merge(this._ValidatePrinter(model.Printer));
            validation.Merge(this._ValidatePrinterSchedule(model.PrinterSchedule));
            validation.Merge(this._ValidatePrinterServices(model.PrinterServices));

            return validation;
        }

        /// <summary>
        /// Save printer with its schedule and services.
        /// </summary>
        /// <param name="model">Printer edition model.</param>
        /// <returns>Updated printer edition model.</returns>
        public PrinterEditionModel Save(PrinterEditionModel model)
        {
            bool isEdit = (model?.Printer?.ID ?? 0) > 0;
            if (isEdit)
            {
                return this._Update(model);
            }
            else
            {
                return this._Insert(model);
            }
        }

        private PrinterEditionModel _Insert(PrinterEditionModel model)
        {
            Argument.NotNull(model, "Значение NULL невозможно сохранить как принтер.");
            this.Validate(model).ThrowExceptionIfNotValid();
            int printerOwnerID = model.Printer.OwnerUserID;

            using (IDataContext context = this.Context())
            {
                IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);
                IPrinterScheduleRepository printerScheduleRepo = this.Repository<IPrinterScheduleRepository>(context);
                IPrinterServiceRepository printerServiceRepo = this.Repository<IPrinterServiceRepository>(context);
                //UserOfferUnit userOfferUnit = new UserOfferUnit();

                //var latestPrinterOwnerOffer = new UserOfferUnit().GetLatestUserOfferByUserID(printerOwnerID, OfferTypeEnum.PrinterOwnerOffer);
                //bool needCreateOffer = !latestPrinterOwnerOffer.HasUserOffer;

                context.BeginTransaction();
                try
                {
                    printerRepo.Insert(model.Printer);
                    context.Save();

                    // Create user offer
                    //if (needCreateOffer)
                    //{
                    //    userOfferUnit.CreateUserOfferInTransaction(printerOwnerID, OfferTypeEnum.PrinterOwnerOffer, context);
                    //}

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

        private PrinterEditionModel _Update(PrinterEditionModel model)
        {
            Argument.NotNull(model, "Значение NULL невозможно сохранить как принтер.");
            this.Validate(model).ThrowExceptionIfNotValid();

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

            var printOrderList = printOrderRepository.Get(e => e.PrinterID == printerID);
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
