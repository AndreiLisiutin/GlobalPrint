using GlobalPrint.Infrastructure.DI;
using GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Payment;
using GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.TransfersRegisters;
using GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Payment;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.TransfersRegisters;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Users;

namespace GlobalPrint.ServerBusinessLogic.DI
{
    /// <summary>
    /// Класс для регистрации зависимостей слоя бизнес-логики.
    /// </summary>
    public static class ServerBusinessLogicIoC
    {
        public static void InitializeIoC(BaseIoC ioc)
        {
            IoC.Instance.Initialize(ioc);
            RegisterDependencies(ioc);
        }

        /// <summary>
        /// Зарегистрировать зависимости слоя бизнес-логики - модулей бизнес логики.
        /// </summary>
        /// <param name="ioc">Контейнер зависимостей.</param>
        private static void RegisterDependencies(BaseIoC ioc)
        {
            // Units/Payment
            ioc.Kernel.Bind<IPaymentActionUnit>().To<PaymentActionUnit>().InTransientScope();

            // Units/TransfersRegisters
            ioc.Kernel.Bind<ITransfersRegisterUnit>().To<TransfersRegisterUnit>().InTransientScope();

            // Units/User
            ioc.Kernel.Bind<IUserUnit>().To<UserUnit>().InTransientScope();
            ioc.Kernel.Bind<IRoleUnit>().To<RoleUnit>().InTransientScope();
            ioc.Kernel.Bind<IUserRoleUnit>().To<UserRoleUnit>().InTransientScope();
        }
    }
}
