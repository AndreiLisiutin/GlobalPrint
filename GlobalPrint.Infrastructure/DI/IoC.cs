using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.DI
{
    internal class IoC : BaseIoC
    {
        protected IoC()
        {
        }
        private static IoC _instance;
        internal static IoC Instance
        {
            get
            {
                return IoC._instance ?? (IoC._instance = new IoC());
            }
        }
    }
}
