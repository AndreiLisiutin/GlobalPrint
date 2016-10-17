using GlobalPrint.Infrastructure.BankUtility;
using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.Infrastructure.DI;
using GlobalPrint.Infrastructure.EmailUtility;
using GlobalPrint.Infrastructure.FileUtility;
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

            // File utility
            ioc.Kernel.Bind<IFileUtility>().To<FileUtility>().InSingletonScope();
            ioc.Kernel.Bind<Lazy<IFileUtility>>().ToMethod(ctx => new Lazy<IFileUtility>(() => ioc.Kernel.Get<IFileUtility>()));
            // Mime types utility
            ioc.Kernel.Bind<IMimeTypeUtility>().To<MimeTypeUtility>().InSingletonScope();
            ioc.Kernel.Bind<Lazy<IMimeTypeUtility>>().ToMethod(ctx => new Lazy<IMimeTypeUtility>(() => ioc.Kernel.Get<IMimeTypeUtility>()));

        }
    }
}
