using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.Infrastructure.EmailUtility;
using GlobalPrint.Infrastructure.FileUtility;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Orders;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Printers;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Orders;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Printers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Orders;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Users;
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

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.UnitsOfWork.Order
{
    public class PrintOrderUnit : BaseUnit
    {
        private Lazy<IEmailUtility> _emailUtility { get; set; }

        [DebuggerStepThrough]
        public PrintOrderUnit(Lazy<IEmailUtility> emailUtility)
            : base()
        {
            _emailUtility = emailUtility;
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
                    .Join(userRepo.GetAll(), e => e.OwnerUserID, e => e.UserID, (p, u) => u)
                    .First();
                newStatus = statusRepo.GetByID((int)statusID);

                if (order.PrintOrderStatusID == (int)statusID)
                {
                    return;
                }

                context.BeginTransaction();
                try
                {
                    order.PrintOrderStatusID = (int)statusID;
                    if (statusID == PrintOrderStatusEnum.Printed && order.PrintedOn == null)
                    {
                        order.PrintedOn = DateTime.Now;
                        printerOwner.AmountOfMoney += order.PricePerPage;
                    }
                    else if (statusID == PrintOrderStatusEnum.Rejected)
                    {
                        client.AmountOfMoney += order.PricePerPage;
                    }

                    context.Save();
                    context.CommitTransaction();
                }
                catch (Exception)
                {
                    context.RollbackTransaction();
                    throw;
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

            //if (!string.IsNullOrEmpty(client.PhoneNumber))
            //{
            //    new SmsUtility(smsParams).Send(client.PhoneNumber, "Заказ №" + order.ID + " " + status.Status.ToLower());
            //}
        }
        
        public PrintOrder New(NewOrder newOrder, string baseDirectory, PrintFile printFile)
        {
            Argument.NotNull(newOrder, "Новый заказ не может быть пустым.");
            Argument.NotNull(printFile, "Файл для печати не может быть пустым.");
            Argument.Require(newOrder.CopiesCount >= 1, "Количество копий не может быть меньше 1.");
            Argument.NotNull(newOrder.FileToPrint, "Файл дл печатм не может быть пустым.");
            Argument.Require(newOrder.PrinterID > 0, "Принтер не может быть пустым.");
            Argument.Require(newOrder.PrintSizeID > 0, "Размер печати не может быть пустым.");
            Argument.Require(newOrder.PrintTypeID > 0, "Тип печати не может быть пустым.");
            Argument.Require(newOrder.UserID > 0, "Заказчик не может быть пустым.");
            Argument.NotNullOrWhiteSpace(newOrder.SecretCode, "Секретный код не может быть пустым.");

            Argument.Require(new FileUtility().IsFormatAcceptable(printFile.Extension), "Формат выбранного файла не поддкрживается системой.");
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
                UserID = newOrder.UserID
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
            order.PagesCount = new FileUtility().GetPagesCount(printFile.SerializedFile, printFile.Extension);
            return order;
        }

        public PrintOrder Create(NewOrder newOrder, string baseDirectory, PrintFile printFile)
        {
            PrintOrder order = this.New(newOrder, baseDirectory, printFile);

            Argument.NotNull(order, "Заказ на печать не может быть пустым.");
            Argument.Positive(order.CopiesCount, "Количество копий должно быть положительным.");
            Argument.NotNullOrWhiteSpace(order.Document, "Документ должнен быть определен.");
            Argument.Positive(order.PagesCount, "Количество страниц должно быть положительным.");
            Argument.Positive(order.PricePerPage, "Цена за страницу должна быть положительной.");
            Argument.Positive(order.PrinterID, "Принтер не может быть неопределенным.");
            Argument.Positive(order.PrintServiceID, "Услуга не может быть неопределенной.");
            Argument.NotNullOrWhiteSpace(order.SecretCode, "Секретный код не может быть пустым.");
            Argument.Positive(order.UserID, "Заказчик должен быть определен.");
            Argument.NotNull(printFile, "Файл для печати не может быть пустым.");
            Argument.NotNull(printFile.SerializedFile, "Файл для печати не может быть пустым.");

            PrinterFullInfoModel printer = new PrinterUnit().GetFullByID(order.PrinterID);
            Argument.NotNull(printer, "Принтер не найден.");
            Argument.Require(printer.IsAvailableNow, "Принтер не доступен.");

            FileInfo file = new FileInfo(order.Document);
            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }
            File.WriteAllBytes(order.Document, printFile.SerializedFile);

            using (IDataContext context = this.Context())
            {
                IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);
                IUserRepository userRepo = this.Repository<IUserRepository>(context);
                IPrintOrderRepository orderRepo = this.Repository<IPrintOrderRepository>(context);
                PrinterUnit printerUnit = new PrinterUnit();

                User client = userRepo.GetByID(order.UserID);
                User printerOperator = printerUnit.GetPrinterOperator(order.PrinterID, context);

                context.BeginTransaction();
                try
                {
                    orderRepo.Insert(order);
                    client.AmountOfMoney -= order.FullPrice;
                    userRepo.Update(client);

                    context.Save();
                    context.CommitTransaction();
                }
                catch (Exception)
                {
                    context.RollbackTransaction();
                    throw;
                }
                
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
        }
    }
}
