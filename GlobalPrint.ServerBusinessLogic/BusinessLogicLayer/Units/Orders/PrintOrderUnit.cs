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
        public Lazy<PaymentActionUnit> _paymentUnit { get; set; }

        [DebuggerStepThrough]
        public PrintOrderUnit(Lazy<IEmailUtility> emailUtility)
            : base()
        {
            this._emailUtility = emailUtility;
            this._fileUtility = IoC.Instance.Resolve<Lazy<IFileUtility>>();
            this._paymentUnit = new Lazy<PaymentActionUnit>(() => new PaymentActionUnit());
        }

        /// <summary>
        /// Get print order by it's identifier.
        /// </summary>
        /// <param name="printOrderID">Print order ID.</param>
        /// <returns>Returns print order object.</returns>
        public PrintOrder GetPrintOrderByID(int printOrderID)
        {
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

        public void UpdateStatus(int printOrderID, PrintOrderStatusEnum statusID, SmsUtility.Parameters smsParams)
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
                    this._paymentUnit.Value.CommitPrintOrder(printOrderID);
                }
                else if (statusID == PrintOrderStatusEnum.Rejected)
                {
                    this._paymentUnit.Value.RollbackPrintOrder(printOrderID);
                }
                else if (statusID == PrintOrderStatusEnum.Accepted)
                {
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
        /// <returns></returns>
        public Tuple<NewOrder, DocumentBusinessInfo> FromExisting(int printOrderID, string baseDirectory)
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

                PrinterFullInfoModel printer = printerUnit.GetFullByID(existingOrder.PrinterID, context);
                Argument.Require(printer != null && printer.IsAvailableNow,
                    $"Принтер, на котором был выполнен заказ, не доступен в данный момент.");

                IEnumerable<PrinterServiceExtended> services = serviceUnit
                    .GetPrinterServices(existingOrder.PrinterID);
                Argument.Require(services.Count() > 0,
                    $"Принтер, на котором был выполнен заказ, не оказывает услуг (PrintOrderID={printOrderID}).");

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
                    IsColored = orderService.PrintService.PrintService.IsColored,
                    IsTwoSided = orderService.PrintService.PrintService.IsTwoSided,
                    PrintSizeID = orderService.PrintService.PrintSize.ID,
                    PrintTypeID = orderService.PrintService.PrintType.ID,
                    SecretCode = existingOrder.SecretCode,
                    PrinterID = existingOrder.PrinterID
                };

                DocumentBusinessInfo businessFile = new DocumentBusinessInfo()
                {
                    Name = existingOrder.DocumentName,
                    Extension = existingOrder.DocumentExtension,
                    SerializedFile = null
                };
                string usersDirectory = Path.Combine(baseDirectory, existingOrder.UserID.ToString());
                string filePath = Path.Combine(usersDirectory, existingOrder.Document);
                FileInfo fileInfo = new FileInfo(filePath);
                if (!fileInfo.Exists)
                {
                    throw new FileNotFoundException($"Не найден файл заказа № {printOrderID} по пути {filePath}.");
                }
                businessFile.SerializedFile = File.ReadAllBytes(filePath);
                return new Tuple<NewOrder, DocumentBusinessInfo>(newOrder, businessFile);
            }
        } 

        public PrintOrder New(NewOrder newOrder, string baseDirectory, DocumentBusinessInfo printFile)
        {
            Argument.NotNull(newOrder, "Новый заказ не может быть пустым.");
            Argument.NotNull(printFile, "Файл для печати не может быть пустым.");
            Argument.Require(newOrder.CopiesCount >= 1, "Количество копий не может быть меньше 1.");
            Argument.NotNull(newOrder.FileToPrint, "Файл для печати не может быть пустым.");
            Argument.Require(newOrder.PrinterID > 0, "Принтер не может быть пустым.");
            Argument.Require(newOrder.PrintSizeID > 0, "Размер печати не может быть пустым.");
            Argument.Require(newOrder.PrintTypeID > 0, "Тип печати не может быть пустым.");
            Argument.Require(newOrder.UserID > 0, "Заказчик не может быть пустым.");
            Argument.NotNullOrWhiteSpace(newOrder.SecretCode, "Секретный код не может быть пустым.");

            Argument.Require(this._fileUtility.Value.IsFormatAcceptable(printFile.Extension), "Формат выбранного файла не поддерживается системой.");
            DirectoryInfo directory = new DirectoryInfo(baseDirectory);
            if (!directory.Exists)
            {
                directory.Create();
            }

            string usersDirectory = Path.Combine(baseDirectory, newOrder.UserID.ToString());
            string filePath = Path.Combine(usersDirectory, printFile.Name);
            PrintOrder order = new PrintOrder()
            {
                Comment = newOrder.Comment,
                CopiesCount = newOrder.CopiesCount,
                Document = filePath,
                OrderedOn = DateTime.Now,
                PagesCount = -1, //is computed below
                PricePerPage = -1,//is computed below
                PrintedOn = null,
                PrinterID = newOrder.PrinterID,
                PrintOrderStatusID = (int)PrintOrderStatusEnum.Waiting,
                PrintServiceID = -1,//is computed below
                SecretCode = newOrder.SecretCode,
                UserID = newOrder.UserID,
                DocumentName = printFile.Name,
                DocumentExtension = printFile.Extension,
                PaymentTransactionID = 0 //will be defined while save
            };

            var services = new PrintServicesUnit().GetPrinterServices(newOrder.PrinterID);
            var service = services.Where(e => e.PrintService.PrintSize.ID == newOrder.PrintSizeID
                && e.PrintService.PrintType.ID == newOrder.PrintTypeID
                && e.PrintService.PrintService.IsColored == newOrder.IsColored
                && e.PrintService.PrintService.IsTwoSided == newOrder.IsTwoSided)
                .FirstOrDefault();

            Argument.Require(service != null, "Выбранная услуга печати не поддерживается принтером.");

            order.PrintServiceID = service.PrinterService.PrintServiceID;
            order.PricePerPage = service.PrinterService.PricePerPage;
            order.PagesCount = this._fileUtility.Value.GetPagesCount(printFile.SerializedFile, printFile.Extension);
            return order;
        }

        public PrintOrder Create(NewOrder newOrder, string baseDirectory, DocumentBusinessInfo printFile)
        {
            PrintOrder order = this.New(newOrder, baseDirectory, printFile);

            Argument.NotNull(order, "Заказ на печать не может быть пустым.");
            Argument.Positive(order.CopiesCount, "Количество копий должно быть положительным.");
            Argument.NotNullOrWhiteSpace(order.Document, "Документ должен быть определен.");
            Argument.Positive(order.PagesCount, "Количество страниц должно быть положительным.");
            Argument.Positive(order.PricePerPage, "Цена за страницу должна быть положительной.");
            Argument.Positive(order.PrinterID, "Принтер не может быть неопределенным.");
            Argument.Positive(order.PrintServiceID, "Услуга не может быть неопределенной.");
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

            FileInfo file = new FileInfo(order.Document);
            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }
            File.WriteAllBytes(order.Document, printFile.SerializedFile);

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

            using (IDataContext context = this.Context())
            {
                IPrintOrderRepository orderRepo = this.Repository<IPrintOrderRepository>(context);

                PrintOrder order = orderRepo.GetByID(printOrderID);
                Argument.Require(order.UserID == userID, "Редактировать можно только свои заказы.");
                Argument.Require(!rating.HasValue || rating > 0, "Рейтинг заказа не может быть отрицательным.");

                order.Comment = comment;
                order.Rating = rating;
                orderRepo.Update(order);

                context.Save();

                return order;
            }
        }
    }
}
