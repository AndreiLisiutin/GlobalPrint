using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.LogUtility
{
    public interface ILogger
    {
        void Trace(string message);
        void Debug(string message);
        void Info(string message);
        void Warn(string message);
        void Error(string message);
        void Fatal(string message);
        void Trace(Exception ex, string message, params object[] args);
        void Debug(Exception ex, string message, params object[] args);
        void Info(Exception ex, string message, params object[] args);
        void Warn(Exception ex, string message, params object[] args);
        void Error(Exception ex, string message, params object[] args);
        void Fatal(Exception ex, string message, params object[] args);
    }
}
