using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.Infrastructure.FileUtility.FileExporters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.FileUtility
{

    /// <summary> Utility to work with different types of files and retreive their information.
    /// </summary>
    public class FileUtility : IFileUtility
    {
        private Dictionary<string, IFileReader> _formats = new Dictionary<string, IFileReader>()
        {
            ["pdf"] = new PdfFileReader(),
            ["doc"] = new DocFileReader(),
            ["docx"] = new DocFileReader(),
            ["jpeg"] = new JpegFileReader(),
            ["jpg"] = new JpegFileReader(),
            ["tif"] = new TiffFileReader(),
            ["tiff"] = new TiffFileReader()
        };

        private Dictionary<FileExporterEnum, string> _fileExporterNames = new Dictionary<FileExporterEnum, string>()
        {
            [FileExporterEnum.Excel] = "Excel",
            [FileExporterEnum.Text] = "Текст"
        };

        /// <summary> Get number of pages in file.
        /// </summary>
        /// <param name="file">File.</param>
        /// <param name="extension">File extension like pdf, doc, docx etc.</param>
        /// <returns>Number of pages in the file.</returns>
        public int GetPagesCount(byte[] file, string extension)
        {
            Argument.NotNullOrWhiteSpace(extension, "Расширение файла не может быть пустым.");
            Argument.NotNull(file, "Файл не может быть пустым.");

            IFileReader fileReader = this.GetFileReader(extension);
            return fileReader.GetPagesCount(file);
        }

        /// <summary> Create specific class for work with a certain type of files.
        /// </summary>
        /// <param name="extension">Files extension.</param>
        /// <returns></returns>
        public IFileReader GetFileReader(string extension)
        {
            Argument.NotNullOrWhiteSpace(extension, "Расширение файла не может быть пустым.");
            extension = extension.Trim().Trim('.').ToLower();

            if (!IsFormatAcceptable(extension))
            {
                throw new ArgumentException(string.Join(
                    "Неопознанный тип файла. Поддерживаются следующие форматы: {0}.",
                        string.Join(", ", this._formats.Keys)
                ));
            }

            return this._formats[extension];
        }

        /// <summary> Check if file with a certain format acceptable for service.
        /// </summary>
        /// <param name="extension">File extension.</param>
        /// <returns></returns>
        public bool IsFormatAcceptable(string extension)
        {
            Argument.NotNullOrWhiteSpace(extension, "Расширение файла не может быть пустым.");
            extension = extension.Trim().Trim('.').ToLower();
            return this._formats.ContainsKey(extension);
        }

        /// <summary>
        /// Get file exporter for reports.
        /// </summary>
        /// <param name="exporter">Type of exporter.</param>
        /// <returns>Actually, the exporter.</returns>
        public IFileExporter GetFileExporter(FileExporterEnum exporter)
        {
            switch (exporter)
            {
                case FileExporterEnum.Excel:
                    return new ExcelExporter();
                case FileExporterEnum.Text:
                    return new TextExporter("|");
                default:
                    throw new InvalidOperationException("Неизвестный формат экспорта файла.");
            }
        }

        /// <summary>
        /// Get file exporter types for reports.
        /// </summary>
        /// <returns>Dictionary of file exporter types with their names.</returns>
        public Dictionary<FileExporterEnum, string> GetFileExporterTypes()
        {
            return this._fileExporterNames.ToDictionary(
                entry => entry.Key,
                entry => entry.Value
            );
        }
    }
}
