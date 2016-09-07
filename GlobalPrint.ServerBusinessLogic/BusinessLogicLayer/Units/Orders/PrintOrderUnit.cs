using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Orders;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Printers;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Orders;
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
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.UnitsOfWork.Order
{
    public class PrintOrderUnit : BaseUnit
    {
        [DebuggerStepThrough]
        public PrintOrderUnit()
            : base()
        {
        }

        public PrintOrder New(NewOrder newOrder, string baseDirectory)
        {
            Argument.Require(newOrder != null, "Новый заказ не может быть пустым.");
            Argument.Require(newOrder.CopiesCount >= 1, "Количество копий не может быть меньше 1.");
            Argument.NotNullOrWhiteSpace(newOrder.FileToPrint, "Файл дл печатм не может быть пустым.");
            Argument.Require(newOrder.PrinterID > 0, "Принтер не может быть пустым.");
            Argument.Require(newOrder.PrintSizeID > 0, "Размер печати не может быть пустым.");
            Argument.Require(newOrder.PrintTypeID > 0, "Тип печати не может быть пустым.");
            Argument.NotNullOrWhiteSpace(newOrder.SecretCode, "Секретный код не может быть пустым.");

            PrintOrder order = new PrintOrder()
            {
                Comment = newOrder.Comment,
                CopiesCount = newOrder.CopiesCount,
                Document = newOrder.FileToPrint,
                OrderedOn = DateTime.Now,
                PagesCount = -1, //compute
                PricePerPage = -1,//compute
                PrintedOn = null,
                PrinterID = newOrder.PrinterID,
                PrintOrderStatusID = (int)PrintOrderStatusEnum.Waiting,
                PrintServiceID = -1,//compute
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

            order.PrintServiceID = service.PrinterService.ID;
            order.PricePerPage = service.PrinterService.PricePerPage;

            Argument.Require(newOrder.FileToPrint.ToLower().EndsWith(".pdf"), "Только PDF.");

            //string usersFolder = Path.Combine(baseFolder, newOrder.UserID.ToString());
            //string pathFoFile = Path.Combine(usersFolder, newOrder.FileToPrint);

            //byte[] serializedFile = null;
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    newOrder.FileToPrint.InputStream.Seek(0, SeekOrigin.Begin);
            //    newOrder.FileToPrint.InputStream.CopyTo(ms);
            //    serializedFile = ms.ToArray();
            //}
            //PdfReader pdfReader = new PdfReader(serializedFile);
            //int numberOfPages = pdfReader.NumberOfPages;
            order.PagesCount = 10;
            return order;
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
            PrintOrderStatus status = null;
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
                    .Join(userRepo.GetAll(), e => e.OwnerUserID, e => e.UserID, (p, u) => u).
                    First();
                status = statusRepo.GetByID((int)statusID);

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
                catch (Exception ex)
                {
                    context.RollbackTransaction();
                    throw;
                }
            }

            if (!string.IsNullOrEmpty(client.PhoneNumber))
            {
                new SmsUtility(smsParams).Send(client.PhoneNumber, "Заказ №" + order.ID + " " + status.Status.ToLower());
            }
        }
    }
}
