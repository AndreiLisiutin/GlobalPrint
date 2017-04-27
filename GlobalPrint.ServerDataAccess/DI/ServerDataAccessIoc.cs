using GlobalPrint.Infrastructure.DI;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Orders;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Payment;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Printers;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext;
using GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Orders;
using GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Payment;
using GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Printers;
using GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.TransfersRegisters;
using GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Users;

namespace GlobalPrint.ServerDataAccess.DI
{
    /// <summary>
    /// Класс для регистрации зависимостей проекта.
    /// </summary>
    public static class ServerDataAccessIoC
    {
        /// <summary>
        /// Инициализировать локальный контейнер зависимостей.
        /// </summary>
        /// <param name="ioc">Базовый контейнер зависимостей.</param>
        public static void InitializeIoC(BaseIoC ioc)
        {
            IoC.Instance.Initialize(ioc);
            ServerDataAccessIoC.RegisterDependencies(ioc);
        }

        /// <summary>
        /// Регистрация зависимостей проекта.
        /// </summary>
        /// <param name="ioc">Контейнер зависимостей.</param>
        private static void RegisterDependencies(BaseIoC ioc)
        {
            // DataContext
            ioc.Kernel.Bind<IDataContextFactory>().To<DbConnectionContextFactory>().InTransientScope();
            
            // Repository/Orders
            ioc.Kernel.Bind<IPrintOrderRepository>().To<PrintOrderRepository>().InTransientScope();
            ioc.Kernel.Bind<IPrintOrderStatusRepository>().To<PrintOrderStatusRepository>().InTransientScope();

            // Repository/Printers
            ioc.Kernel.Bind<IPrinterRepository>().To<PrinterRepository>().InTransientScope();
            ioc.Kernel.Bind<IPrinterScheduleRepository>().To<PrinterScheduleRepository>().InTransientScope();
            ioc.Kernel.Bind<IPrinterServiceRepository>().To<PrinterServiceRepository>().InTransientScope();
            ioc.Kernel.Bind<IPrintServiceRepository>().To<PrintServiceRepository>().InTransientScope();
            ioc.Kernel.Bind<IPrintSizePrintTypeRepository>().To<PrintSizePrintTypeRepository>().InTransientScope();
            ioc.Kernel.Bind<IPrintSizeRepository>().To<PrintSizeRepository>().InTransientScope();
            ioc.Kernel.Bind<IPrintTypeRepository>().To<PrintTypeRepository>().InTransientScope();

            // Repository/Users
            ioc.Kernel.Bind<IRoleRepository>().To<RoleRepository>().InTransientScope();
            ioc.Kernel.Bind<IUserActionLogRepository>().To<UserActionLogRepository>().InTransientScope();
            ioc.Kernel.Bind<IUserActionTypeRepository>().To<UserActionTypeRepository>().InTransientScope();
            ioc.Kernel.Bind<IUserRepository>().To<UserRepository>().InTransientScope();
            ioc.Kernel.Bind<IUserRoleRepository>().To<UserRoleRepository>().InTransientScope();
            
            // Repository/Payment
            ioc.Kernel.Bind<IPaymentActionRepository>().To<PaymentActionRepository>().InTransientScope();
            ioc.Kernel.Bind<IPaymentActionTypeRepository>().To<PaymentActionTypeRepository>().InTransientScope();
            ioc.Kernel.Bind<IPaymentActionStatusRepository>().To<PaymentActionStatusRepository>().InTransientScope();
            ioc.Kernel.Bind<IPaymentTransactionRepository>().To<PaymentTransactionRepository>().InTransientScope();
            ioc.Kernel.Bind<IPaymentTransactionStatusRepository>().To<PaymentTransactionStatusRepository>().InTransientScope();

            // Repository/TransfersRegisters
            ioc.Kernel.Bind<ICashRequestRepository>().To<CashRequestRepository>().InTransientScope();
            ioc.Kernel.Bind<ICashRequestStatusRepository>().To<CashRequestStatusRepository>().InTransientScope();
            ioc.Kernel.Bind<ITransfersRegisterRepository>().To<TransfersRegisterRepository>().InTransientScope();
        }
    }
}
