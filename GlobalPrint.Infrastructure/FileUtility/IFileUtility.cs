using GlobalPrint.Infrastructure.FileUtility.FileExporters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.FileUtility
{
    public interface IFileUtility
    {
        /// <summary> Get number of pages in file.
        /// </summary>
        /// <param name="file">File.</param>
        /// <param name="extension">File extension like pdf, doc, docx etc.</param>
        /// <returns>Number of pages in the file.</returns>
        int GetPagesCount(byte[] file, string extension);

        /// <summary> Create specific class for work with a certain type of files.
        /// </summary>
        /// <param name="extension">Files extension.</param>
        /// <returns></returns>
        IFileReader GetFileReader(string extension);

        /// <summary> Check if file with a certain format acceptable for service.
        /// </summary>
        /// <param name="extension">File extension.</param>
        /// <returns></returns>
        bool IsFormatAcceptable(string extension);

        /// <summary>
        /// Get file exporter for reports.
        /// </summary>
        /// <param name="exporter">Type of exporter.</param>
        /// <returns>Actually, the exporter.</returns>
        IFileExporter GetFileExporter(FileExporterEnum exporter);

        /// <summary>
        /// Get file exporter types for reports.
        /// </summary>
        /// <returns>Dictionary of file exporter types with their names.</returns>
        Dictionary<FileExporterEnum, string> GetFileExporterTypes();
    }
}
