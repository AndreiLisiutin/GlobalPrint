using GlobalPrint.ClientWeb.Models.PushNotifications;
using GlobalPrint.Configuration.DI;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Users;
using Microsoft.AspNet.SignalR;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobalPrint.ClientWeb.DI
{
    public static class SignalRIoC
    {
        public static void InitializeIoC(IDependencyResolver dependencyResolver)
        {
            GlobalHost.DependencyResolver = dependencyResolver;
        }

        public static void RegisterDependencies(IKernel kernel)
        {
            UserUnit userUnit = IoC.Instance.Resolve<UserUnit>();

            // Find user by ID functionality for SignalR
            var userIdProvider = new UserIdProvider(userUnit);
            kernel.Bind<IUserIdProvider>().ToMethod(context => userIdProvider);
        }
    }
}