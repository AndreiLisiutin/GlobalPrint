using GlobalPrint.Infrastructure.FileUtility.FileExporters.Exportable;
using System;
using System.Collections.Generic;

namespace GlobalPrint.Infrastructure.FileUtility.FileExporters
{
    /// <summary>
    /// Simple registers exporter. 
    /// </summary>
    public interface IFileExporter
    {
        /// <summary>
        /// Exports List of objects into certain format and serializes it.
        /// </summary>
        /// <param name="exportable">List of objects to export.</param>
        /// <returns>Serialized file of register.</returns>
        byte[] ExportToMemory(IEnumerable<ExportableEntity> exportable);

        /// <summary>
        /// Determines file extension for the format.
        /// </summary>
        /// <returns>String file extension.</returns>
        string GetFileExtension();
    }
}
