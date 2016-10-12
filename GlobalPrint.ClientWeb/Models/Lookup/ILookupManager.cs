using GlobalPrint.Infrastructure.CommonUtils.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobalPrint.ClientWeb.Models.Lookup
{
    /// <summary>
    /// Lookup manager of certain type.
    /// </summary>
    public interface ILookupManager
    {
        /// <summary>
        /// Get list of entities serialized to a List<LookupResultValue> structure.
        /// </summary>
        /// <param name="searchText">Search text parameter.</param>
        /// <param name="paging">Paging parameter.</param>
        /// <returns>List of entities matching search text and paging criteria.</returns>
        PagedList<List<LookupResultValue>> GetEntitiesList(string searchText, Paging paging);

        /// <summary>
        /// Get entity by its identifier field value.
        /// </summary>
        /// <param name="id">Identifier field value of the entity.</param>
        /// <returns>Found by id entity.</returns>
        List<LookupResultValue> GetEntitityByID(long id);

        /// <summary>
        /// Get columns to define lookup entity properties.
        /// </summary>
        /// <returns>Columns of each entity.</returns>
        List<LookupResultValue> GetColumns();
    }
}