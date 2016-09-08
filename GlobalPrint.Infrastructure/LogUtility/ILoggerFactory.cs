
namespace GlobalPrint.Infrastructure.LogUtility
{
    public interface ILoggerFactory
    {
        /// <summary>
        /// Creates a logger with the Callsite of the given Type T
        /// </summary>
        /// <typeparam name="T">Callsite type</typeparam>
        /// <returns>Logger</returns>
        ILogger GetLogger<T>() where T : class;

        /// <summary>
        /// Creates a logger with the Callsite of current type
        /// </summary>
        /// <returns>Logger</returns>
        ILogger GetCurrentClassLogger();
    }
}
