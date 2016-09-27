using GlobalPrint.Infrastructure.CommonUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GlobalPrint.ServerBusinessLogic.Models.Business
{
    public class PrintFile
    {
        public PrintFile()
        {

        }
        private PrintFile(HttpPostedFileBase httpPostedFileBase)
        {
            using (Stream inputStream = httpPostedFileBase.InputStream)
            {
                MemoryStream memoryStream = inputStream as MemoryStream;
                if (memoryStream == null)
                {
                    memoryStream = new MemoryStream();
                    inputStream.CopyTo(memoryStream);
                }
                this.SerializedFile = memoryStream.ToArray();
            }
            this.Extension = new MimeTypeUtility().ConvertMimeTypeToExtension(httpPostedFileBase.ContentType);
            this.Name = httpPostedFileBase.FileName;
        }

        public static PrintFile FromHttpPostedFileBase(HttpPostedFileBase httpPostedFileBase)
        {
            return new PrintFile(httpPostedFileBase);
        }

        public byte[] SerializedFile { get; set; }
        public string Extension { get; set; }
        public string Name { get; set; }
    }
}
