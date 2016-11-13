using iTextSharp.text.pdf;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
            object saveOption = WdSaveOptions.wdDoNotSaveChanges;
            object originalFormat = WdOriginalFormat.wdOriginalDocumentFormat;
            object routeDocument = false;
            Document document = null;
            var application = new Application();
            try
            {
                application.DisplayAlerts = WdAlertLevel.wdAlertsNone;
                var tmpFile = Path.GetTempFileName();
                using (var tmpFileStream = File.OpenWrite(tmpFile))
                {
                    tmpFileStream.Write(file, 0, file.Length);
                    tmpFileStream.Close();
                }
                document = application.Documents.Open(tmpFile);
                int numberOfPages = document.ComputeStatistics(WdStatistic.wdStatisticPages, false);

                document.Close(ref saveOption, ref originalFormat, ref routeDocument);
                return numberOfPages;
            }
            finally
            {
                application.Quit(saveOption, originalFormat, routeDocument);
                _ReleaseObject(document);
                _ReleaseObject(application);
            }
        }

        /// <summary>
        /// Release managed Word resource.
        /// </summary>
        /// <param name="obj">Resource.</param>
        private void _ReleaseObject(object obj)
        {
            if (obj == null)
            {
                return;
            }
            try
            {
                Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch
            {
                obj = null;
            }
        }
    }
}
