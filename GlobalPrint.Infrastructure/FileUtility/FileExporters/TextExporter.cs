using GlobalPrint.Infrastructure.FileUtility.FileExporters.Exportable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.FileUtility.FileExporters
{

    /// <summary>
    /// Txt format register exporter. 
    /// Every object in list lays on the new file row and its properties are separated by a certain delimiter.
    /// </summary>
    public class TextExporter : IFileExporter
    {
        /// <summary>
        /// Delimiter for objects' properties.
        /// </summary>
        public string Delimiter { get; set; }
        public TextExporter(string delimiter = "\t")
        {
            this.Delimiter = delimiter;
        }

        /// <summary>
        /// Exports List of objects into text format and serializes it.
        /// </summary>
        /// <param name="exportable">List of objects to export.</param>
        /// <returns>Serialized text file of register.</returns>
        public byte[] ExportToMemory(IEnumerable<ExportableEntity> exportable)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (TextWriter tw = new StreamWriter(memoryStream))
            {
                ExportableEntity firstEntity = exportable.FirstOrDefault();
                if (firstEntity == null)
                {
                    return this._Serialize(memoryStream, tw);
                }

                tw.WriteLine(string.Join(this.Delimiter, firstEntity.Properties.Select(e => e.PropertyName ?? "--")));
                tw.WriteLine();

                foreach (ExportableEntity entity in exportable)
                {
                    tw.WriteLine(string.Join(this.Delimiter, entity.Properties.Select(e => e.Value ?? "--")));
                }
                return this._Serialize(memoryStream, tw);
            }
        }

        private byte[] _Serialize(MemoryStream memoryStream, TextWriter tw)
        {
            tw.Flush();
            return memoryStream.ToArray();
        }

        /// <summary>
        /// Determines file extension for the format.
        /// </summary>
        /// <returns>String file extension.</returns>
        public string GetFileExtension()
        {
            return "txt";
        }
    }
}
