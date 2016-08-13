using GlobalPrint.Infrastructure.DI;
using GlobalPrint.ServerBusinessLogic.DI;
using GlobalPrint.ServerDataAccess.DI;
using Ninject;
using Ninject.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Configuration.DI
{
    public class IoC : BaseIoC
    {
        protected IoC()
        {
        }
        private static IoC _instance;
        public static IoC Instance
        {
            get
            {
                return IoC._instance ?? (IoC._instance = new IoC());
            }
        }

        public void RegisterServerBusinessLogic()
        {
            ServerBusinessLogicIoC.InitializeIoC(this);
        }
        public void RegisterServerDataAccess()
        {
            ServerDataAccessIoc.InitializeIoC(this);
        }
    }
}
