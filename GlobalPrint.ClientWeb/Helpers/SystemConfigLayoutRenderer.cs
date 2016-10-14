using NLog;
using NLog.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace GlobalPrint.ClientWeb.Helpers
{
    [LayoutRenderer("system-configuration")]
    public class SystemConfigurationLayoutRenderer : LayoutRenderer
    {
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
#if DEBUG
            builder.Append("DEBUG");
#else
            builder.Append("RELEASE"); 
#endif

        }
    }
}