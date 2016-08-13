using GlobalPrint.Infrastructure.DI;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Orders;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Printers;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext;
using GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Orders;
using GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Printers;
using GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Users;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerDataAccess.DI
{
    public static class ServerDataAccessIoc
    {
        public static void InitializeIoC(BaseIoC ioc)
        {
            IoC.Instance.Initialize(ioc);
            ServerDataAccessIoc.RegisterDependencies(ioc);
        }

        private static void RegisterDependencies(BaseIoC ioc)
        {
            //DataContext
            ioc.Kernel.Bind<IDataContextFactory>().To<DbConnectionContextFactory>().InTransientScope();
            
            //Repository/Orders
            ioc.Kernel.Bind<IPrintOrderRepository>().To<PrintOrderRepository>().InTransientScope();
            ioc.Kernel.Bind<IPrintOrderStatusRepository>().To<PrintOrderStatusRepository>().InTransientScope();

            //Repository/Printers
            ioc.Kernel.Bind<IPrinterRepository>().To<PrinterRepository>().InTransientScope();
            ioc.Kernel.Bind<IPrinterScheduleRepository>().To<PrinterScheduleRepository>().InTransientScope();
            ioc.Kernel.Bind<IPrinterServiceRepository>().To<PrinterServiceRepository>().InTransientScope();
            ioc.Kernel.Bind<IPrintServiceRepository>().To<PrintServiceRepository>().InTransientScope();
            ioc.Kernel.Bind<IPrintSizePrintTypeRepository>().To<PrintSizePrintTypeRepository>().InTransientScope();
            ioc.Kernel.Bind<IPrintSizeRepository>().To<PrintSizeRepository>().InTransientScope();
            ioc.Kernel.Bind<IPrintTypeRepository>().To<PrintTypeRepository>().InTransientScope();

            //Repository/Users
            ioc.Kernel.Bind<IRoleRepository>().To<RoleRepository>().InTransientScope();
            ioc.Kernel.Bind<IUserActionLogRepository>().To<UserActionLogRepository>().InTransientScope();
            ioc.Kernel.Bind<IUserActionTypeRepository>().To<UserActionTypeRepository>().InTransientScope();
            ioc.Kernel.Bind<IUserRepository>().To<UserRepository>().InTransientScope();
            ioc.Kernel.Bind<IUserRoleRepository>().To<UserRoleRepository>().InTransientScope();

        }
    }
}
