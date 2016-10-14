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
    /// <summary>
    /// Information about document.
    /// </summary>
    public class DocumentBusinessInfo
    {
        public DocumentBusinessInfo()
        {
        }

        /// <summary>
        /// Create business document info from uploaded by http file.
        /// </summary>
        /// <param name="httpPostedFileBase">Uploaded by http file.</param>
        /// <param name="userID">User who loaded this file.</param>
        private DocumentBusinessInfo(HttpPostedFileBase httpPostedFileBase, int userID)
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
            this.Extension = new MimeTypeUtility().ConvertMimeTypeToExtension(httpPostedFileBase.ContentType)
                .Trim('.');
            this.Name = httpPostedFileBase.FileName;
            this.LoadedOn = DateTime.Now;
            this.UserID = userID;
        }
        
        /// <summary>
        /// File serialized to byte array.
        /// </summary>
        public byte[] SerializedFile { get; set; }
        /// <summary>
        /// File extension. Example: txt.
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// File name with extension. Example: MyFile.txt.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// User who loaded the file.
        /// </summary>
        public int UserID { get; set; }
        /// <summary>
        /// When the file was loaded.
        /// </summary>
        public DateTime LoadedOn { get; set; }

        /// <summary>
        /// Create business document info from uploaded by http file.
        /// </summary>
        /// <param name="httpPostedFileBase">Uploaded by http file.</param>
        /// <returns></returns>
        public static DocumentBusinessInfo FromHttpPostedFileBase(HttpPostedFileBase httpPostedFileBase, int userID)
        {
            return new DocumentBusinessInfo(httpPostedFileBase, userID);
        }
    }
}
