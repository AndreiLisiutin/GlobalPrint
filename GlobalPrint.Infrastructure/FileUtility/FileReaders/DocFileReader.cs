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
    /// <summary> Class working with MS Word files.
    /// </summary>
    public class DocFileReader : IFileReader
    {
        /// <summary> Get number of pages in file.
        /// </summary>
        /// <param name="file">File.</param>
        /// <returns>Number of pages in the file.</returns>
        public int GetPagesCount(byte[] file)
        {
            var application = new Application();
            var tmpFile = Path.GetTempFileName();
            try
            {
                using (var tmpFileStream = File.OpenWrite(tmpFile))
                {
                    tmpFileStream.Write(file, 0, file.Length);
                    tmpFileStream.Close();
                }
                Document document = application.Documents.Open(tmpFile);
                int numberOfPages = document.ComputeStatistics(WdStatistic.wdStatisticPages, false);
                return numberOfPages;
            }
            finally
            {
            }
        }
    }
}
