namespace GlobalPrint.Infrastructure.LogUtility
{
    public class NLogLoggerFactory : ILoggerFactory
    {
        private NLog.LogFactory _logFactory { get; set; }

        public NLogLoggerFactory()
        {
            _logFactory = new NLog.LogFactory();
        }

        public ILogger GetLogger<T>() where T : class
        {
            return _logFactory.GetLogger<NLogLogger>(typeof(T).FullName);
        }

        public ILogger GetCurrentClassLogger()
        {
            return _logFactory.GetCurrentClassLogger<NLogLogger>();
        }
    }
}
