using GlobalPrint.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobalPrint.ClientWeb
{
    public class PreparedOrder
    {
        public PrintOrder order { get; set; }
        public byte[] serializedFile { get; set; }
    }
}