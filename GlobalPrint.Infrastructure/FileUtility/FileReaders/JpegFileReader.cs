using iTextSharp.text.pdf;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.FileUtility
{
    /// <summary> Class working with Jpeg picture files.
    /// </summary>
    public class JpegFileReader : IFileReader
    {
        /// <summary> Get number of pages in file.
        /// </summary>
        /// <param name="file">File.</param>
        /// <returns>Number of pages in the file.</returns>
        public int GetPagesCount(byte[] file)
        {
            return 1;
        }
    }
}
