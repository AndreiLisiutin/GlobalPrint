using GlobalPrint.Infrastructure.DI;
using GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Users;

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
            // Units/User
            ioc.Kernel.Bind<IUserUnit>().To<UserUnit>().InTransientScope();
            ioc.Kernel.Bind<IRoleUnit>().To<RoleUnit>().InTransientScope();
            ioc.Kernel.Bind<IUserRoleUnit>().To<UserRoleUnit>().InTransientScope();
        }
    }
}
