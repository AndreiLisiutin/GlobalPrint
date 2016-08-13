using Ninject;
using Ninject.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.DI
{
    public class BaseIoC
    {
        protected IKernel _kernel;
        protected BaseIoC()
        {
        }
        public void Initialize(BaseIoC ioc)
        {
            this._kernel = ioc._kernel;
        }
        public void Initialize(IKernel kernel)
        {
            this._kernel = kernel;
        }

        public IKernel Kernel
        {
            get
            {
                return this._kernel;
            }
        }

        public S Resolve<S>(params _Argument[] arguments)
        {
            return this._kernel.Get<S>(arguments);
        }

        public static _Argument Argument(string name, object value)
        {
            return new _Argument(name, value);
        }

        public class _Argument : ConstructorArgument
        {
            public _Argument(string name, object value)
                : base(name, value)
            {
            }
        }
    }
}
