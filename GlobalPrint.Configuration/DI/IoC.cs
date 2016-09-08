using GlobalPrint.Infrastructure.DI;
using GlobalPrint.ServerBusinessLogic.DI;
using GlobalPrint.ServerDataAccess.DI;

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
            ServerDataAccessIoC.InitializeIoC(this);
        }
        public void RegisterInfrastructure()
        {
            InfrastructureIoC.InitializeIoC(this);
        }
    }
}
