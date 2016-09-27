using GlobalPrint.Infrastructure.BankUtility;
using GlobalPrint.Infrastructure.DI;
using GlobalPrint.Infrastructure.EmailUtility;
using GlobalPrint.Infrastructure.LogUtility;
using Ninject;
using System;

namespace GlobalPrint.ServerBusinessLogic.DI
{
    public static class InfrastructureIoC
    {
        public static void InitializeIoC(BaseIoC ioc)
        {
            IoC.Instance.Initialize(ioc);
            InfrastructureIoC.RegisterDependencies(ioc);
        }

        private static void RegisterDependencies(BaseIoC ioc)
        {
            // NLog
            ioc.Kernel.Bind<ILoggerFactory>().To<NLogLoggerFactory>().InSingletonScope();
            ioc.Kernel.Bind<Lazy<ILoggerFactory>>().ToMethod(ctx => new Lazy<ILoggerFactory>(() => ioc.Kernel.Get<ILoggerFactory>()));

            // Email
            ioc.Kernel.Bind<IEmailUtility>().To<EmailUtility>().InSingletonScope();
            ioc.Kernel.Bind<Lazy<IEmailUtility>>().ToMethod(ctx => new Lazy<IEmailUtility>(() => ioc.Kernel.Get<IEmailUtility>()));

            // Bank
            ioc.Kernel.Bind<IBankUtility>().To<BankUtility>().InSingletonScope();
            ioc.Kernel.Bind<Lazy<IBankUtility>>().ToMethod(ctx => new Lazy<IBankUtility>(() => ioc.Kernel.Get<BankUtility>()));
        }
    }
}
