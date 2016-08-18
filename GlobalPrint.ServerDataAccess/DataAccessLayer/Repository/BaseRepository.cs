using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository;
using GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerDataAccess.DataAccessLayer.Repository
{
    /// <summary>
    /// Базовый класс репозиториев проекта. Реализует паттерн "Generic Repository"
    /// http://www.codeproject.com/Articles/70061/Architecture-Guide-ASP-NET-MVC-Framework-N-tier-En
    /// https://chsakell.com/2015/02/15/asp-net-mvc-solution-architecture-best-practices/
    /// </summary>
    /// <typeparam name="T">Тип репозитория</typeparam>
    public class BaseRepository<T> : IRepository<T> 
        where T : class, ServerBusinessLogic.BusinessLogicLayer.Models.Domain.IDomainModel
    {
        private readonly DbConnectionContext _context;
        private readonly Lazy<DbSet<T>> _entities;

        public BaseRepository(DbConnectionContext context)
        {
            this._context = context;
            this._entities = new Lazy<DbSet<T>>(() => context.DB.Set<T>());
        }

        #region Get

        public virtual IQueryable<T> Get(Expression<Func<T, bool>> filter)
        {
            if (filter == null)
            {
                return this._entities.Value;
            }
            return this._entities.Value.Where(filter);
        }
        public virtual IQueryable<T> GetAll()
        {
            return this._entities.Value;
        }
        public virtual T GetByID(object id)
        {
            return this._entities.Value.Find(id);
        }

        #endregion

        #region CUD

        public virtual void Insert(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            this._entities.Value.Add(entity);
        }

        public void Insert(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entity");
            }
            this._entities.Value.AddRange(entities);
        }

        public virtual void Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            T original = this._entities.Value.Find(entity.ID);
            if (original != null)
            {
                this._context.DB.Entry(original).CurrentValues.SetValues(entity);
            }
        }

        public virtual void Delete(object id)
        {
            T entity = this._entities.Value.Find(id);
            if (entity != null)
            {
                this.Delete(entity);
            }
        }
        public virtual void Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            this._entities.Value.Remove(entity);
        }
        public virtual void Delete(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entity");
            }

            this._entities.Value.RemoveRange(entities);
        }

        #endregion

    }
}
