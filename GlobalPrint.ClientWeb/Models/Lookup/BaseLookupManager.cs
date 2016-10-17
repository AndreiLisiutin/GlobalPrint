using GlobalPrint.Infrastructure.CommonUtils.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobalPrint.ClientWeb.Models.Lookup
{
    /// <summary>
    /// Base lookup data manager.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseLookupManager<T> : ILookupManager
    {
        /// <summary>
        /// Get list of entities by search text and paging.
        /// </summary>
        /// <param name="searchText">Search text parameter.</param>
        /// <param name="paging">Pagination info.</param>
        /// <returns>List of matching entities.</returns>
        public abstract List<T> GetEntities(string searchText, Paging paging);

        /// <summary>
        /// Get entity by its identifier.
        /// </summary>
        /// <param name="id">Identifier of the entity.</param>
        /// <returns>Found entity by its identifier.</returns>
        public abstract T GetEntity(long id);

        /// <summary>
        /// Get count of all elements of the lookup set matching searchText parameter.
        /// </summary>
        /// <param name="searchText">Search text parameter.</param>
        /// <returns>Count.</returns>
        public abstract int GetCount(string searchText);

        /// <summary>
        /// Convert entity to List<LookupResultValue> structure.
        /// </summary>
        /// <param name="entity">Business entity got from GetEntities method.</param>
        /// <returns>Structured list of entity properties.</returns>
        public abstract List<LookupResultValue> Convert(T entity);

        /// <summary>
        /// Create an empty entity for GetColumns method.
        /// </summary>
        /// <returns>Empty entity.</returns>
        public virtual T GetEmptyEntity()
        {
            return Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Get list of entities serialized to a List<LookupResultValue> structure.
        /// </summary>
        /// <param name="searchText">Search text parameter.</param>
        /// <param name="paging">Paging parameter.</param>
        /// <returns>List of entities matching search text and paging criteria.</returns>
        public virtual PagedList<List<LookupResultValue>> GetEntitiesList(string searchText, Paging paging)
        {
            List<T> entities = this.GetEntities(searchText, paging);
            List<List<LookupResultValue>> lookupList = entities.Select(e => this.Convert(e)).ToList();
            int count = this.GetCount(searchText);

            return new PagedList<List<LookupResultValue>>(lookupList, count, paging.ItemsPerPage, paging.CurrentPage);
        }

        /// <summary>
        /// Get entity by its identifier field value.
        /// </summary>
        /// <param name="id">Identifier field value of the entity.</param>
        /// <returns>Found by id entity.</returns>
        public List<LookupResultValue> GetEntitityByID(long id)
        {
            T entity = this.GetEntity(id);
            List<LookupResultValue> lookupEntity = this.Convert(entity);
            return lookupEntity;
        }

        /// <summary>
        /// Get columns to define lookup entity properties.
        /// </summary>
        /// <returns>Columns of each entity.</returns>
        public virtual List<LookupResultValue> GetColumns()
        {
            T entity = this.GetEmptyEntity();
            List<LookupResultValue> lookupEntity = this.Convert(entity);
            return lookupEntity;
        }
    }
}