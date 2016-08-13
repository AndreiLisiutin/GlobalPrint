using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Core.IoC
{
    public class IoC
    {
        IKernel _kernel = new StandardKernel();
        public IKernel Kernel
        {
            get
            {
                return this._kernel;
            }
        }
    }
}
