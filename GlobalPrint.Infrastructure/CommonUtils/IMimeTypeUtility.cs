using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.CommonUtils
{
    /// <summary>
    /// Utility for processing mime types and file extensions.
    /// </summary>
    public interface IMimeTypeUtility
    {
        /// <summary>
        /// Get file extension from mime type.
        /// </summary>
        /// <param name="mimeType">Mime type for file extensions search.</param>
        /// <returns>Found file extension.</returns>
        string ConvertMimeTypeToExtension(string mimeType);

        /// <summary>
        /// Get mime type from file extension.
        /// </summary>
        /// <param name="extension">File extension for mime types search.</param>
        /// <returns>Found mime type.</returns>
        string ConvertExtensionToMimeType(string extension);
    }
}
