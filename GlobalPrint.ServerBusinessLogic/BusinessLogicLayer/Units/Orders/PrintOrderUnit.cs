using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Orders;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Printers;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Orders;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Orders;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.UnitsOfWork.Order
{
    public class PrintOrderUnit : BaseUnit
    {
        [DebuggerStepThrough]
        public PrintOrderUnit()
            :base()
        {
        }

        public List<PrintOrderInfo> GetUserPrintOrderList(int UserID)
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
                    where PrintOrder.UserID == UserID
                    orderby PrintOrder.OrderedOn descending
                    select new PrintOrderInfo() { PrintOrder = PrintOrder, Printer = Printer, Status = Status };
                return printOrderList.ToList();
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
                   where PrintOrder.PrintOrderID == printOrderID
                   orderby PrintOrder.OrderedOn descending
                   select new PrintOrderInfo() { PrintOrder = PrintOrder, Printer = Printer, Status = Status };
                return printOrderList.First();
            }
        }

        public List<PrintOrderInfo> GetUserRecievedPrintOrderList(int UserID)
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
                    where Printer.OwnerUserID == UserID
                    orderby PrintOrder.OrderedOn descending
                    select new PrintOrderInfo() { PrintOrder = PrintOrder, Printer = Printer, Status = Status };
                return printOrderList.ToList();
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
                new SmsUtility(smsParams).Send(client.PhoneNumber, "Заказ №" + order.PrintOrderID + " " + status.Status.ToLower());
            }
        }
    }
}
