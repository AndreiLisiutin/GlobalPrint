using GlobalPrint.Infrastructure.CommonUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.FileUtility
{
    /// <summary> Utility to work with different types of files and retreive their information.
    /// </summary>
    public class FileUtility
    {
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
            switch (extension.Trim().Trim('.').ToLower())
            {
                case "pdf":
                    return new PdfFileReader();
                default:
                    throw new ArgumentException("Неопознанный тип файла. Поддерживаются только PDF.");
            }
        }
    }
}
