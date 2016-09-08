using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace GlobalPrint.Infrastructure.LogUtility
{
    /// <summary>
    /// Log utility of NLog
    /// </summary>
    public class NLogLogger : NLog.Logger, ILogger
    {
        public NLogLogger()
            :base()
        {
        }
    }
}