using GlobalPrint.ClientWeb.Models.PushNotifications;
using GlobalPrint.Infrastructure.DI;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobalPrint.ClientWeb.DI
{
    public static class ClientWebIoC
    {
        public static void InitializeIoC(BaseIoC ioc)
        {
            IoC.Instance.Initialize(ioc);
            ClientWebIoC.RegisterDependencies(ioc);
        }

        /// <summary>
        /// Register resolver for SignalR
        /// </summary>
        /// <param name="kernel">Ninject kernel</param>
        public static void InitializeSignalRResolver(IKernel kernel)
        {
            var resolver = new NinjectSignalRDependencyResolver(kernel);
            GlobalHost.DependencyResolver = resolver;
            ClientWebIoC.RegisterSignalRDependencies(kernel);
        }

        private static void RegisterDependencies(BaseIoC ioc)
        {

        }

        private static void RegisterSignalRDependencies(IKernel kernel)
        {
            // Find user by ID functionality for SignalR
            var userIdProvider = new UserIdProvider(new ServerBusinessLogic.BusinessLogicLayer.Units.Users.UserUnit());
            kernel.Bind<IUserIdProvider>().ToMethod(context => userIdProvider);
            
            //kernel.Bind<IHubConnectionContext>().ToMethod(context =>
            //    resolver.Resolve<IConnectionManager>().GetHubContext<StockTickerHub>().Clients
            //    ).WhenInjectedInto<IStockTicker>();
        }
    }
}