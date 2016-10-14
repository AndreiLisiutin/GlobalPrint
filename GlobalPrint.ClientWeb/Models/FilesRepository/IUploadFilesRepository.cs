using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.ServerBusinessLogic.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;

namespace GlobalPrint.ClientWeb.Models.FilesRepository
{
    /// <summary>
    /// Storage in memory for uploaded files.
    /// </summary>
    public interface IUploadFilesRepository
    {

        /// <summary>
        /// Add new document.
        /// </summary>
        /// <param name="document">Document to add.</param>
        /// <returns>Identifier of the file inside storage.</returns>
        Guid Add(DocumentBusinessInfo document);

        /// <summary>
        /// Add new document with known identifier.
        /// </summary>
        /// <param name="fileIdentifier">Identifier to add.</param>
        /// <param name="document">Document to add.</param>
        /// <returns>Identifier of the file inside storage.</returns>
        Guid Add(Guid? fileIdentifier, DocumentBusinessInfo document);

        /// <summary>
        /// Add new document.
        /// </summary>
        /// <param name="postedFile">Http posted file from web-form.</param>
        /// <param name="userID">Current user identifier.</param>
        /// <returns>Identifier of the file inside storage.</returns>
        Guid Add(HttpPostedFileBase postedFile, int userID);

        /// <summary>
        /// Check if repository contains the file with specified identifier.
        /// </summary>
        /// <param name="fileIdentifier">Identifier of the file inside the storage.</param>
        /// <returns>Boolean value, TRUE if contains.</returns>
        bool Contains(Guid? fileIdentifier);

        /// <summary>
        /// Get file by its identifier. If identifier is not found or equals to NULL, returns NULL.
        /// </summary>
        /// <param name="fileIdentifier">Identifier of the file inside the storage.</param>
        /// <returns>Found file.</returns>
        DocumentBusinessInfo Get(Guid? fileIdentifier);

        /// <summary>
        /// Remove the file from storage by its identifier.
        /// </summary>
        /// <param name="fileIdentifier">Identifier of the file.</param>
        void Remove(Guid? fileIdentifier);
    }
}