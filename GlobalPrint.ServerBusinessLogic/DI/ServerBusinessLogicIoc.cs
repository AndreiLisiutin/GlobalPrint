using GlobalPrint.Infrastructure.DI;
using GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Users;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Orders;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Printers;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Users;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.DI
{
    public static class ServerBusinessLogicIoC
    {
        public static void InitializeIoC(BaseIoC ioc)
        {
            IoC.Instance.Initialize(ioc);
            ServerBusinessLogicIoC.RegisterDependencies(ioc);
        }

        private static void RegisterDependencies(BaseIoC ioc)
        {
            // Units/Offer
            //ioc.Kernel.Bind<IUserOfferUnit>().To<UserOfferUnit>().InTransientScope();
            //ioc.Kernel.Bind<IOfferUnit>().To<OfferUnit>().InTransientScope();

            // Units/User
            ioc.Kernel.Bind<IUserUnit>().To<UserUnit>().InTransientScope();

        }
    }
}
