using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobalPrint.ClientWeb.DI
{
    public class NinjectDependencyResolver : 
        Microsoft.AspNet.SignalR.DefaultDependencyResolver, 
        System.Web.Mvc.IDependencyResolver
    {
        private readonly IKernel _kernel;

        public NinjectDependencyResolver(IKernel kernel)
        {
            _kernel = kernel;
            
            Configuration.DI.IoC.Instance.Initialize(kernel);
            RegisterClientDependencies(kernel);
            Configuration.DI.IoC.Instance.RegisterInfrastructure();
            Configuration.DI.IoC.Instance.RegisterServerBusinessLogic();
            Configuration.DI.IoC.Instance.RegisterServerDataAccess();

            // InitializeSignalRResolver
            SignalRIoC.InitializeIoC(this);
            SignalRIoC.RegisterDependencies(this._kernel);
        }

        public override object GetService(Type serviceType)
        {
            return _kernel.TryGet(serviceType) ?? base.GetService(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType).Concat(base.GetServices(serviceType));
        }

        private void RegisterClientDependencies(IKernel kernel)
        {

        }
    }
}