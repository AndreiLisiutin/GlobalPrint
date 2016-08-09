using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Server.DAL
{
    /// <summary>
    /// Базовый класс репозиториев проекта. Реализует паттерн "Generic Repository"
    /// http://www.codeproject.com/Articles/70061/Architecture-Guide-ASP-NET-MVC-Framework-N-tier-En
    /// https://chsakell.com/2015/02/15/asp-net-mvc-solution-architecture-best-practices/
    /// </summary>
    /// <typeparam name="T">Тип репозитория</typeparam>
    public class BaseRepository<T> : IRepository<T> 
        where T : class
    {
        private readonly DbContext _context;
        private readonly IDbSet<T> _entities;

        public BaseRepository(DbContext context)
        {
            this._context = context;
            this._entities = context.Set<T>();
        }

        #region Get

        public virtual IQueryable<T> Get(Expression<Func<T, bool>> filter)
        {
            if (filter == null)
            {
                return this._entities;
            }
            return this._entities.Where(filter);
        }
        public virtual IQueryable<T> GetAll()
        {
            return this._entities;
        }
        public virtual T GetByID(object id)
        {
            return this._entities.Find(id);
        }

        #endregion

        #region CUD

        public virtual void Insert(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            this._entities.Add(entity);
        }

        public virtual void Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            //this._entities.Attach(entity);
            this._context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(object id)
        {
            T entity = this._entities.Find(id);
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

            this._entities.Remove(entity);
        }

        #endregion

    }
}
