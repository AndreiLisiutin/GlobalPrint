using GlobalPrint.Infrastructure.DI;

namespace GlobalPrint.ServerDataAccess.DI
{
    /// <summary>
    /// Локальный контейнер зависимостей.
    /// </summary>
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
