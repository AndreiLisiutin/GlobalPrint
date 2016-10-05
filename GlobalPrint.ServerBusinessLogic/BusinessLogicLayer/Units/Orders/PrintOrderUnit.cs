using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.Infrastructure.EmailUtility;
using GlobalPrint.Infrastructure.FileUtility;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Orders;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Printers;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.Models.Business;
using GlobalPrint.ServerBusinessLogic.Models.Business.Orders;
using GlobalPrint.ServerBusinessLogic.Models.Business.Printers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Orders;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Printers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Utilities;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Payment;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Users;
using GlobalPrint.ServerBusinessLogic.DI;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Printers;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.UnitsOfWork.Order
{
    public class PrintOrderUnit : BaseUnit
    {
        private Lazy<IEmailUtility> _emailUtility { get; set; }
        private Lazy<IFileUtility> _fileUtility { get; set; }
        private Lazy<PaymentActionUnit> _paymentUnit { get; set; }
        private Lazy<PrintServicesUnit> _printServiceUnit { get; set; }
        private Lazy<PrinterUnit> _printerUnit { get; set; }

        [DebuggerStepThrough]
        public PrintOrderUnit(Lazy<IEmailUtility> emailUtility)
            : base()
        {
            this._emailUtility = emailUtility;
            this._fileUtility = IoC.Instance.Resolve<Lazy<IFileUtility>>();
            this._paymentUnit = new Lazy<PaymentActionUnit>(() => new PaymentActionUnit());
            this._printServiceUnit = new Lazy<PrintServicesUnit>(() => new PrintServicesUnit());
            this._printerUnit = new Lazy<PrinterUnit>(() => new PrinterUnit());
        }

        /// <summary>
        /// Get print order by it's identifier.
        /// </summary>
        /// <param name="printOrderID">Print order ID.</param>
        /// <returns>Returns print order object.</returns>
        public PrintOrder GetByID(int printOrderID)
        {
            Argument.NotNull(printOrderID, "Ключ заказа на печать должен быть положительным.");

            using (IDataContext context = this.Context())
            {
                return this.Repository<IPrintOrderRepository>(context)
                    .GetByID(printOrderID);
            }
        }

        public List<PrintOrderInfo> GetUserPrintOrderList(int userID, string printOrderID)
        {
            using (IDataContext context = this.Context())
            {
                IPrintOrderRepository orderRepo = this.Repository<IPrintOrderRepository>(context);
                IUserRepository userRepo = this.Repository<IUserRepository>(context);
                IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);
                IPrintOrderStatusRepository statusRepo = this.Repository<IPrintOrderStatusRepository>(context);

                var printOrderList =
                    from PrintOrder in orderRepo.GetAll()
                    join Printer in printerRepo.GetAll() on PrintOrder.PrinterID equals Printer.ID
                    join Status in statusRepo.GetAll() on PrintOrder.PrintOrderStatusID equals Status.PrintOrderStatusID
                    where PrintOrder.UserID == userID
                    orderby PrintOrder.OrderedOn descending
                    select new PrintOrderInfo() { PrintOrder = PrintOrder, Printer = Printer, Status = Status };
                return printOrderList
                    .ToList()
                    .Where(x => string.IsNullOrEmpty(printOrderID) ||
                        x.PrintOrder.ID.ToString().IndexOf(printOrderID) >= 0)
                    .ToList();
            }
        }

        public PrintOrderInfo GetPrintOrderInfoByID(int printOrderID)
        {
            using (IDataContext context = this.Context())
            {
                IPrintOrderRepository orderRepo = this.Repository<IPrintOrderRepository>(context);
                IUserRepository userRepo = this.Repository<IUserRepository>(context);
                IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);
                IPrintOrderStatusRepository statusRepo = this.Repository<IPrintOrderStatusRepository>(context);

                var printOrderList =
                   from PrintOrder in orderRepo.GetAll()
                   join Printer in printerRepo.GetAll() on PrintOrder.PrinterID equals Printer.ID
                   join Status in statusRepo.GetAll() on PrintOrder.PrintOrderStatusID equals Status.PrintOrderStatusID
                   where PrintOrder.ID == printOrderID
                   orderby PrintOrder.OrderedOn descending
                   select new PrintOrderInfo() { PrintOrder = PrintOrder, Printer = Printer, Status = Status };
                return printOrderList.First();
            }
        }

        public List<PrintOrderInfo> GetUserRecievedPrintOrderList(int userID, string printOrderID)
        {
            using (IDataContext context = this.Context())
            {
                IPrintOrderRepository orderRepo = this.Repository<IPrintOrderRepository>(context);
                IUserRepository userRepo = this.Repository<IUserRepository>(context);
                IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);
                IPrintOrderStatusRepository statusRepo = this.Repository<IPrintOrderStatusRepository>(context);

                var printOrderList =
                    from PrintOrder in orderRepo.GetAll()
                    join Printer in printerRepo.GetAll() on PrintOrder.PrinterID equals Printer.ID
                    join Status in statusRepo.GetAll() on PrintOrder.PrintOrderStatusID equals Status.PrintOrderStatusID
                    where Printer.OwnerUserID == userID
                    orderby PrintOrder.OrderedOn descending
                    select new PrintOrderInfo() { PrintOrder = PrintOrder, Printer = Printer, Status = Status };
                return printOrderList
                    .ToList()
                    .Where(x => string.IsNullOrEmpty(printOrderID) ||
                        x.PrintOrder.ID.ToString().IndexOf(printOrderID) >= 0)
                    .ToList();
            }
        }

        public void UpdateStatus(int printOrderID, PrintOrderStatusEnum statusID, int userID)
        {
            PrintOrderStatus newStatus = null;
            User client = null;
            User printerOwner = null;
            PrintOrder order = null;

            using (IDataContext context = this.Context())
            {
                IPrintOrderRepository orderRepo = this.Repository<IPrintOrderRepository>(context);
                IUserRepository userRepo = this.Repository<IUserRepository>(context);
                IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);
                IPrintOrderStatusRepository statusRepo = this.Repository<IPrintOrderStatusRepository>(context);

                order = orderRepo.GetByID(printOrderID);
                client = userRepo.GetByID(order.UserID);
                printerOwner = printerRepo.Get(e => e.ID == order.PrinterID)
                    .Join(userRepo.GetAll(), e => e.OwnerUserID, e => e.ID, (p, u) => u)
                    .First();
                newStatus = statusRepo.GetByID((int)statusID);

                if (order.PrintOrderStatusID == (int)statusID
                    || order.PrintOrderStatusID == (int)PrintOrderStatusEnum.Printed
                    || order.PrintOrderStatusID == (int)PrintOrderStatusEnum.Rejected)
                {
                    return;
                }

                if (statusID == PrintOrderStatusEnum.Printed && order.PrintedOn == null)
                {
                    Argument.Require(printerOwner.ID == userID, "Выполнить заказ может только владелец принтера.");
                    this._paymentUnit.Value.CommitPrintOrder(printOrderID);
                }
                else if (statusID == PrintOrderStatusEnum.Rejected)
                {
                    Argument.Require(printerOwner.ID == userID, "Отменить заказ может только владелец принтера.");
                    this._paymentUnit.Value.RollbackPrintOrder(printOrderID);
                }
                else if (statusID == PrintOrderStatusEnum.Accepted)
                {
                    Argument.Require(printerOwner.ID == userID, "Принять заказ может только владелец принтера.");
                    order.PrintOrderStatusID = (int)PrintOrderStatusEnum.Accepted;
                    orderRepo.Update(order);
                    context.Save();
                }
            }

            // send email to client about order statuc change
            MailAddress userMail = new MailAddress(client.Email, client.UserName);
            string userMessageBody = string.Format(
                "Ваш заказ № {0} {1}.",
                order.ID,
                newStatus.Status.ToLower()
            );
            _emailUtility.Value.Send(userMail, "Global Print - Изменение статуса заказа", userMessageBody);
        }

        /// <summary>
        /// Createt new order object from already existing order.
        /// </summary>
        /// <param name="printOrderID">Order identifier.</param>
        /// <param name="userID">Current user identifier.</param>
        /// <returns>New order with the same details as existing one.</returns>
        public NewOrder FromExisting(int printOrderID, int userID)
        {
            Argument.Positive(printOrderID, "Ключ исходного заказа меньше нуля.");
            using (IDataContext context = this.Context())
            {
                IPrintOrderRepository orderRepo = this.Repository<IPrintOrderRepository>(context);
                PrintServicesUnit serviceUnit = new PrintServicesUnit();
                PrinterUnit printerUnit = new PrinterUnit();

                PrintOrder existingOrder = orderRepo.GetByID(printOrderID);
                Argument.NotNull(existingOrder,
                    $"Указанный исходный заказ не найден (PrintOrderID={printOrderID}).");
                Argument.Require(userID == existingOrder.UserID, "Нельзя клонировать чужие заказы.");

                IEnumerable<PrinterServiceExtended> services = serviceUnit
                    .GetPrinterServices(existingOrder.PrinterID);

                //service associated with the order
                PrinterServiceExtended orderService = services
                    .FirstOrDefault(e => e.PrintService.PrintService.ID == existingOrder.PrintServiceID);

                //or default of the printer
                orderService = orderService ?? services.FirstOrDefault();

                NewOrder newOrder = new NewOrder()
                {
                    Comment = existingOrder.Comment,
                    CopiesCount = existingOrder.CopiesCount,
                    UserID = existingOrder.UserID,
                    FileToPrint = Guid.NewGuid(),
                    IsColored = orderService?.PrintService.PrintService.IsColored ?? false,
                    IsTwoSided = orderService?.PrintService.PrintService.IsTwoSided ?? false,
                    PrintSizeID = orderService?.PrintService.PrintSize.ID ?? 0,
                    PrintTypeID = orderService?.PrintService.PrintType.ID ?? 0,
                    SecretCode = existingOrder.SecretCode,
                    PrinterID = existingOrder.PrinterID
                };

                return newOrder;
            }
        }

        /// <summary>
        /// Get user order's file from the database.
        /// </summary>
        /// <param name="printOrderID">Order identifier.</param>
        /// <param name="baseDirectory">App_Data directory location.</param>
        /// <returns>File of the order and its info.</returns>
        public DocumentBusinessInfo GetPrintOrderDocument(int printOrderID, int userID, string baseDirectory)
        {
            Argument.NotNull(printOrderID, "Ключ заказа на печать должен быть положительным.");

            PrintOrder order = this.GetByID(printOrderID);
            Argument.NotNull(order, $"Заказ на печать не найден (PrintOrderID={printOrderID}).");
            Argument.Require(order.UserID == userID, "Нельзя скачивать чужие заказы.");

            string _physicalPathToFile = PrintOrderUnit.PRINT_ORDER_FILE_PATH(baseDirectory, order.UserID, order.InternalDocumentName);
            FileInfo physicalFile = new FileInfo(_physicalPathToFile);
            if (!physicalFile.Directory.Exists)
            {
                throw new FileNotFoundException($"Файл заказа № {printOrderID} по пути {_physicalPathToFile} не найден.");
            }
            byte[] fileArray = File.ReadAllBytes(_physicalPathToFile);
            DocumentBusinessInfo fileInfo = new DocumentBusinessInfo()
            {
                SerializedFile = fileArray,
                Name = order.DocumentName,
                Extension = order.DocumentExtension
            };

            return fileInfo;
        }

        /// <summary>
        /// Validate incoming print order model.
        /// </summary>
        /// <param name="newOrder">Print order model.</param>
        /// <param name="printFile">Print order file model.</param>
        /// <returns>Validation object with errors.</returns>
        public Validation Validate(NewOrder newOrder, DocumentBusinessInfo printFile)
        {
            Validation validation = new Validation();

            validation.NotNull(printFile, "Файл для печати не может быть пустым.");
            validation.NotNull(printFile.SerializedFile, "Файл для печати не может быть пустым.");
            validation.NotNullOrWhiteSpace(printFile.Name, "Название файла для печати не может быть пустым.");
            validation.NotNullOrWhiteSpace(printFile.Extension, "Расширение файла для печати не может быть пустым.");
            validation.NotNull(newOrder, "Новый заказ не может быть пустым.");
            validation.Positive(newOrder.PrinterID, "Принтер не может быть пустым.");
            validation.NotNullOrWhiteSpace(newOrder.SecretCode, "Секретный код не может быть пустым.");
            validation.Positive(newOrder.PrintSizeID, "Размер печати не может быть пустым.");
            validation.Positive(newOrder.PrintTypeID, "Тип печати не может быть пустым.");
            validation.Require(newOrder.FileToPrint != Guid.Empty, "Для нового заказа не задан .");
            validation.Positive(newOrder.CopiesCount, "Количество копий не может быть меньше 1.");
            validation.Positive(newOrder.UserID, "Заказчик не может быть пустым.");

            bool isExtensionAcceptable = this._fileUtility.Value.IsFormatAcceptable(printFile.Extension);
            validation.Require(isExtensionAcceptable, "Формат выбранного файла не поддерживается системой.");

            PrinterServiceExtended service = this.GetPrintService(newOrder);
            validation.NotNull(service, "Выбранная услуга печати не поддерживается принтером.");

            PrinterFullInfoModel printer = this._printerUnit.Value.GetFullByID(newOrder.PrinterID);
            validation.NotNull(printer, "Принтер не найден.");

            validation.Require(printer.IsAvailableNow, "В данный момент принтер недоступен.");

            return validation;
        }

        /// <summary>
        /// Calculate pages count of the document.
        /// </summary>
        /// <param name="document">Document to calculate pages.</param>
        /// <returns>Pages count.</returns>
        public int CalculatePagesCount(DocumentBusinessInfo document)
        {
            Argument.NotNull(document, "Документ не может быть пустым.");
            Argument.NotNull(document.SerializedFile, "Документ не может быть пустым.");
            Argument.NotNullOrWhiteSpace(document.Extension, "Расширение документа не может быть пустым.");
            bool isFormatAcceptable = this._fileUtility.Value.IsFormatAcceptable(document.Extension);
            Argument.Require(isFormatAcceptable, $"Выбранное расширение документа ({document.Extension}) не поддерживается системой.");

            IFileReader fileReader = this._fileUtility.Value.GetFileReader(document.Extension);
            int pagesCount = fileReader.GetPagesCount(document.SerializedFile);
            return pagesCount;
        }

        /// <summary>
        /// Retrieves print service information by new order. If doesn't found - returns null.
        /// </summary>
        /// <param name="newOrder">New print order model.</param>
        /// <returns>Print service information. If doesn't found - returns null.</returns>
        public PrinterServiceExtended GetPrintService(NewOrder newOrder)
        {
            Argument.NotNull(newOrder, "Модель заказа не может быть пустой.");
            Argument.Positive(newOrder.PrintSizeID, "Размер печати не может быть пустым.");
            Argument.Positive(newOrder.PrintTypeID, "Тип печати не может быть пустой.");

            IEnumerable<PrinterServiceExtended> services = _printServiceUnit.Value.GetPrinterServices(newOrder.PrinterID);
            PrinterServiceExtended service = services.Where(e =>
                e.PrintService.PrintSize.ID == newOrder.PrintSizeID
                && e.PrintService.PrintType.ID == newOrder.PrintTypeID
                && e.PrintService.PrintService.IsColored == newOrder.IsColored
                && e.PrintService.PrintService.IsTwoSided == newOrder.IsTwoSided
            )
                .FirstOrDefault();

            return service;
        }

        private PrintOrder _ConvertNewOrderToDomainOrder(NewOrder newOrder, string baseDirectory, DocumentBusinessInfo document)
        {
            DirectoryInfo directory = new DirectoryInfo(baseDirectory);
            if (!directory.Exists)
            {
                directory.Create();
            }

            string fileName = $"{newOrder.FileToPrint.ToString()}.{document.Extension}";
            PrintOrder order = new PrintOrder()
            {
                Comment = newOrder.Comment,
                CopiesCount = newOrder.CopiesCount,
                InternalDocumentName = fileName,
                OrderedOn = DateTime.Now,
                PagesCount = -1, //is computed below
                PricePerPage = -1,//is computed below
                PrintedOn = null,
                PrinterID = newOrder.PrinterID,
                PrintOrderStatusID = (int)PrintOrderStatusEnum.Waiting,
                PrintServiceID = -1,//is computed below
                SecretCode = newOrder.SecretCode,
                UserID = newOrder.UserID,
                DocumentName = document.Name,
                DocumentExtension = document.Extension,
                PaymentTransactionID = 0 //will be defined while save
            };

            PrinterServiceExtended service = this.GetPrintService(newOrder);
            Argument.Require(service != null, "Выбранная услуга печати не поддерживается принтером.");

            order.PrintServiceID = service.PrinterService.PrintServiceID;
            order.PricePerPage = service.PrinterService.PricePerPage;
            order.PagesCount = this.CalculatePagesCount(document);
            return order;
        }

        public PrintOrder Create(NewOrder newOrder, int userID, string baseDirectory, DocumentBusinessInfo printFile)
        {
            Validation validation = this.Validate(newOrder, printFile);
            validation.ThrowExceptionIfNotValid();

            PrintOrder order = this._ConvertNewOrderToDomainOrder(newOrder, baseDirectory, printFile);

            Argument.NotNull(order, "Заказ на печать не может быть пустым.");
            Argument.Require(order.UserID == userID, "Нельзя сохранять заказы за другого пользователя.");
            Argument.Positive(order.CopiesCount, "Количество копий должно быть положительным.");
            Argument.NotNullOrWhiteSpace(order.InternalDocumentName, "Документ должен быть определен.");
            Argument.Positive(order.PagesCount, "Количество страниц должно быть положительным.");
            Argument.Positive(order.PricePerPage, "Цена за страницу должна быть положительной.");
            Argument.Positive(order.PrinterID, "Принтер не может быть неопределенным.");
            Argument.NotNullOrWhiteSpace(order.DocumentName, "Название документа не может быть пустым.");
            Argument.NotNullOrWhiteSpace(order.DocumentExtension, "Расширение документа не может быть пустым.");
            Argument.Positive(order.PrintServiceID, "Услуга не может быть неопределенной.");
            Argument.NotNullOrWhiteSpace(order.SecretCode, "Секретный код не может быть пустым.");
            Argument.Positive(order.UserID, "Заказчик должен быть определен.");
            Argument.NotNull(printFile, "Файл для печати не может быть пустым.");
            Argument.NotNull(printFile.SerializedFile, "Файл для печати не может быть пустым.");

            PrinterUnit printerUnit = new PrinterUnit();
            PrinterFullInfoModel printer = printerUnit.GetFullByID(order.PrinterID);
            Argument.NotNull(printer, "Принтер не найден.");
            Argument.Require(printer.IsAvailableNow, "Принтер не доступен.");

            string _physicalPathToFile = PrintOrderUnit.PRINT_ORDER_FILE_PATH(baseDirectory, newOrder.UserID, order.InternalDocumentName);
            FileInfo physicalFile = new FileInfo(_physicalPathToFile);
            if (!physicalFile.Directory.Exists)
            {
                physicalFile.Directory.Create();
            }
            File.WriteAllBytes(_physicalPathToFile, printFile.SerializedFile);

            //perform main business logic
            order = this._paymentUnit.Value.InitializePrintOrder(order);

            User client = IoC.Instance.Resolve<UserUnit>().GetUserByID(order.UserID);
            User printerOperator = printerUnit.GetPrinterOperator(order.PrinterID);

            // send email to user about his new order
            MailAddress userMail = new MailAddress(client.Email, client.UserName);
            string userMessageBody = string.Format(
                "Ваш новый заказ № {0} от {1} успешно зарегистрирован.",
                order.ID,
                order.OrderedOn.ToString("dd.MM.yyyy HH:mm")
            );
            _emailUtility.Value.Send(userMail, "Global Print - Новый заказ на печать", userMessageBody);
            // send email to printer operator about new order
            MailAddress userOperatorMail = new MailAddress(printerOperator.Email, printerOperator.UserName);
            string userOperatorMessageBody = string.Format(
                "На Ваш принтер поступил новый заказ № {0} от {1}.",
                order.ID,
                order.OrderedOn.ToString("dd.MM.yyyy HH:mm")
            );
            _emailUtility.Value.Send(userOperatorMail, "Global Print - Входящий заказ на печать", userOperatorMessageBody);

            return order;
        }

        /// <summary>
        /// Rate print order.
        /// </summary>
        /// <param name="printOrderID">Identifier of the order.</param>
        /// <param name="rating">Rating with stars of the order.</param>
        /// <param name="comment">Comment for the order.</param>
        /// <param name="userID">Current system's user.</param>
        /// <returns>Updated order.</returns>
        public PrintOrder Rate(int printOrderID, float? rating, string comment, int userID)
        {
            Argument.Positive(printOrderID, $"Заказ на печать не найден (ID={printOrderID}).");
            Argument.Require(!rating.HasValue || rating > 0, "Рейтинг заказа не может быть отрицательным.");

            using (IDataContext context = this.Context())
            {
                IPrintOrderRepository orderRepo = this.Repository<IPrintOrderRepository>(context);

                PrintOrder order = orderRepo.GetByID(printOrderID);
                Argument.Require(order.UserID == userID, "Редактировать можно только свои заказы.");

                order.Comment = comment;
                order.Rating = rating;
                orderRepo.Update(order);

                context.Save();

                return order;
            }
        }

        /// <summary>
        /// Calculate full price of the order.
        /// </summary>
        /// <param name="pricePerPage">Price per page of the print service.</param>
        /// <param name="pagesCount">Order document pages count.</param>
        /// <param name="copiesCount">Order document copies count.</param>
        /// <returns></returns>
        public static decimal CALCULATE_FULL_PRICE(decimal pricePerPage, int pagesCount, int copiesCount)
        {
            Argument.Positive(pricePerPage, "Цена за страницу должна быть положительным числом.");
            Argument.Positive(pagesCount, "Количество страниц должно быть положительным числом.");
            Argument.Positive(copiesCount, "Количество копий должно быть положительным числом.");

            return pricePerPage * pagesCount * copiesCount;
        }

        /// <summary>
        /// Calculate path to user's file from print order.
        /// </summary>
        /// <param name="app_data">App_Data folder path.</param>
        /// <param name="userID">User identifier.</param>
        /// <param name="fileName">File name with extension. Example: myFile.txt.</param>
        /// <returns>String as full path to save the file.</returns>
        public static string PRINT_ORDER_FILE_PATH(string app_data, int userID, string fileName)
        {
            Argument.NotNullOrWhiteSpace(app_data, "Путь к App_Data не задан.");
            Argument.Positive(userID, "Пользователь-хозяин файла не задан.");
            Argument.NotNullOrWhiteSpace(fileName, "Название файла с расширением не задано.");

            return Path.Combine(app_data, userID.ToString(), fileName);
        }
    }
}
