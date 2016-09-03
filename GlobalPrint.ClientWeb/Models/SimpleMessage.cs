using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobalPrint.ClientWeb.Models
{
    public class SimpleMessage
    {
        public string Title { get; set; }
        public string Message { get; set; }
    }

    public class MyModel
    {
        public HttpPostedFileBase MyFile { get; set; }
    }
}