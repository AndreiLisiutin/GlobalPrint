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
    public class UploadFilesRepository : IUploadFilesRepository
    {
        private const string _key = "_UploadFiles";
        private Func<Dictionary<Guid, DocumentBusinessInfo>> _sessionDictionary;
        private readonly object _locker = new object();
        public UploadFilesRepository(Func<Dictionary<Guid, DocumentBusinessInfo>> sessionDictionary)
        {
            this._sessionDictionary = sessionDictionary;
            Timer timer = new Timer(60 * 60 * 1000);
            timer.Elapsed += ClearOldFiles;
            timer.Start();
        }

        /// <summary>
        /// Clear files older than hour. Is to be executed every hour.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearOldFiles(object sender, ElapsedEventArgs e)
        {
            try
            {
                lock (_locker)
                {
                    DateTime treshold = DateTime.Now.AddHours(-1);
                    IEnumerable<Guid> oldFilesIdentifiers = this._Uploaded
                        .Where(f => f.Value.LoadedOn < treshold)
                        .Select(f => f.Key);
                    foreach (Guid fileIdentifier in oldFilesIdentifiers)
                    {
                        this._Uploaded.Remove(fileIdentifier);
                    }
                }
            }
            catch (Exception ex)
            {
                //???
                throw;
            }
        }

        /// <summary>
        /// Uploaded files exact location.
        /// </summary>
        protected Dictionary<Guid, DocumentBusinessInfo> _Uploaded
        {
            get
            {
                return this._sessionDictionary();
            }
        }

        /// <summary>
        /// Add new document.
        /// </summary>
        /// <param name="document">Document to add.</param>
        /// <returns>Identifier of the file inside storage.</returns>
        public Guid Add(DocumentBusinessInfo document)
        {
            return this.Add(Guid.NewGuid(), document);
        }

        /// <summary>
        /// Add new document with known identifier.
        /// </summary>
        /// <param name="fileIdentifier">Identifier to add.</param>
        /// <param name="document">Document to add.</param>
        /// <returns>Identifier of the file inside storage.</returns>
        public Guid Add(Guid? fileIdentifier, DocumentBusinessInfo document)
        {
            Argument.NotNull(document, "Документ для загрузки на сервер пустой.");
            fileIdentifier = fileIdentifier ?? Guid.NewGuid();
            lock (_locker)
            {
                this._Uploaded.Add(fileIdentifier.Value, document);
            }
            return fileIdentifier.Value;
        }

        /// <summary>
        /// Add new document.
        /// </summary>
        /// <param name="postedFile">Http posted file from web-form.</param>
        /// <param name="userID">Current user identifier.</param>
        /// <returns>Identifier of the file inside storage.</returns>
        public Guid Add(HttpPostedFileBase postedFile, int userID)
        {
            Argument.NotNull(postedFile, "Документ для загрузки на сервер пустой.");
            Argument.Positive(userID, "Пользователь, загружающий файл, пустой.");
            DocumentBusinessInfo document = DocumentBusinessInfo.FromHttpPostedFileBase(postedFile, userID);
            return this.Add(document);
        }

        /// <summary>
        /// Check if repository contains the file with specified identifier.
        /// </summary>
        /// <param name="fileIdentifier">Identifier of the file inside the storage.</param>
        /// <returns>Boolean value, TRUE if contains.</returns>
        public bool Contains(Guid? fileIdentifier)
        {
            lock (_locker)
            {
                return fileIdentifier.HasValue && this._Uploaded.ContainsKey(fileIdentifier.Value);
            }
        }

        /// <summary>
        /// Get file by its identifier. If identifier is not found or equals to NULL, returns NULL.
        /// </summary>
        /// <param name="fileIdentifier">Identifier of the file inside the storage.</param>
        /// <returns>Found file.</returns>
        public DocumentBusinessInfo Get(Guid? fileIdentifier)
        {
            if (!this.Contains(fileIdentifier))
            {
                return null;
            }
            lock (_locker)
            {
                return this._Uploaded[fileIdentifier.Value];
            }
        }

        /// <summary>
        /// Remove the file from storage by its identifier.
        /// </summary>
        /// <param name="fileIdentifier">Identifier of the file.</param>
        public void Remove(Guid? fileIdentifier)
        {
            if (!this.Contains(fileIdentifier))
            {
                return;
            }
            lock (_locker)
            {
                this._Uploaded.Remove(fileIdentifier.Value);
            }
        }
    }
}