using GlobalPrint.Configuration.DI;
using GlobalPrint.Infrastructure.CommonUtils.Pagination;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Users;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobalPrint.ClientWeb.Models.Lookup.LookupManagers
{
    /// <summary>
    /// Lookup manager for User entity.
    /// </summary>
    public class UserLookupManager : BaseLookupManager<User>
    {
        private UserUnit _userUnit;
        public UserLookupManager()
            : this(IoC.Instance.Resolve<UserUnit>())
        {
        }
        public UserLookupManager(UserUnit userUnit)
        {
            this._userUnit = userUnit;
        }

        /// <summary>
        /// Get list of entities by search text and paging.
        /// </summary>
        /// <param name="searchText">Search text parameter.</param>
        /// <param name="paging">Pagination info.</param>
        /// <returns>List of matching entities.</returns>
        public override List<User> GetEntities(string searchText, Paging paging)
        {
            return this._userUnit.GetByFilter(searchText, paging);
        }

        /// <summary>
        /// Get entity by its identifier.
        /// </summary>
        /// <param name="id">Identifier of the entity.</param>
        /// <returns>Found entity by its identifier.</returns>
        public override User GetEntity(long id)
        {
            return this._userUnit.GetByID((int)id);
        }

        /// <summary>
        /// Get count of all elements of the lookup set matching searchText parameter.
        /// </summary>
        /// <param name="searchText">Search text parameter.</param>
        /// <returns>Count.</returns>
        public override int GetCount(string searchText)
        {
            return this._userUnit.CountByFilter(searchText);
        }

        /// <summary>
        /// Convert entity to List<LookupResultValue> structure.
        /// </summary>
        /// <param name="entity">Business entity got from GetEntities method.</param>
        /// <returns>Structured list of entity properties.</returns>
        public override List<LookupResultValue> Convert(User entity)
        {
            return new List<LookupResultValue>()
            {
                new LookupResultValue("ID", entity.ID.ToString(), 0, isIdentifier: true),
                new LookupResultValue("Email", entity.Email, 5, isText: true),
                new LookupResultValue("Логин", entity.UserName, 4),
                new LookupResultValue("Дата последней активности", entity.LastActivityDate.ToString("dd.MM.yyyy HH:mm:ss"), 2),
            };
        }
    }
}