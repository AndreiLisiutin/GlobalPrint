using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository;
using GlobalPrint.ServerBusinessLogic.Models.Domain;
using GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace GlobalPrint.ServerDataAccess.DataAccessLayer.Repository
{
    /// <summary>
    /// Базовый класс репозиториев проекта. Реализует паттерн "Generic Repository"
    /// http://www.codeproject.com/Articles/70061/Architecture-Guide-ASP-NET-MVC-Framework-N-tier-En
    /// https://chsakell.com/2015/02/15/asp-net-mvc-solution-architecture-best-practices/
    /// </summary>
    /// <typeparam name="T">Тип репозитория</typeparam>
    public class BaseRepository<T> : IRepository<T>
        where T : class, IDomainModel
    {
        /// <summary>
        /// Контекст соединения с базой данных.
        /// </summary>
        private readonly DbConnectionContext _context;

        /// <summary>
        /// Набор сущностей в базе.
        /// </summary>
        private readonly Lazy<DbSet<T>> _entities;

        public BaseRepository(DbConnectionContext context)
        {
            this._context = context;
            this._entities = new Lazy<DbSet<T>>(() => context.DB.Set<T>());
        }

        #region Get

        /// <summary>
        /// Получить список сущностей из базы по фильтру.
        /// </summary>
        /// <param name="filter">Фильтр.</param>
        /// <returns>Список сущностей.</returns>
        public virtual IQueryable<T> Get(Expression<Func<T, bool>> filter)
        {
            if (filter == null)
            {
                return this._entities.Value;
            }
            return this._entities.Value.Where(filter);
        }

        /// <summary>
        /// Получить список всех сущностей в базе.
        /// </summary>
        /// <returns>Список всех сущностей в базе.</returns>
        public virtual IQueryable<T> GetAll()
        {
            return this._entities.Value;
        }

        /// <summary>
        /// Получить сущность по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор сущности.</param>
        /// <returns>Сущность, полученная из базы по идентификатору.</returns>
        public virtual T GetByID(object id)
        {
            return this._entities.Value.Find(id);
        }

        #endregion

        #region CUD

        /// <summary>
        /// Добавить новую сущность в базу.
        /// </summary>
        /// <param name="entity">Новая сущность.</param>
        public virtual void Insert(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            this._entities.Value.Add(entity);
        }

        /// <summary>
        /// Добавить список сущностей в базу.
        /// </summary>
        /// <param name="entities">Список сущностей.</param>
        public void Insert(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entity");
            }
            this._entities.Value.AddRange(entities);
        }

        /// <summary>
        /// Обновить существующую сущность в базе.
        /// </summary>
        /// <param name="entity">Существующая сущность в базе.</param>
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

        /// <summary>
        /// Обновить список сущностей в базе.
        /// </summary>
        /// <param name="entities">Список сущностей.</param>
        public void Update(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }
            List<int> ids = entities.Select(e => e.ID).ToList();
            IEnumerable<T> formDB = this.Get(db => ids.Contains(db.ID))
                .ToList();
            foreach (T old in formDB)
            {
                T @new = entities.First(e => e.ID == old.ID);
                this._context.DB.Entry(old).CurrentValues.SetValues(@new);
            }
        }

        /// <summary>
        /// Удалить сущность из базы по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор сущности.</param>
        public virtual void Delete(object id)
        {
            T entity = this._entities.Value.Find(id);
            if (entity != null)
            {
                this.Delete(entity);
            }
        }

        /// <summary>
        /// Удалить сущность из базы.
        /// </summary>
        /// <param name="entity">Сущность.</param>
        public virtual void Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            T original = this._entities.Value.Find(entity.ID);
            if (original != null)
            {
                this._entities.Value.Remove(original);
            }
        }

        /// <summary>
        /// Удалить список сущностей из базы.
        /// </summary>
        /// <param name="entities">Список сущностей.</param>
        public virtual void Delete(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }
            List<int> ids = entities.ToList().Select(e => e.ID).ToList();
            IEnumerable<T> formDB = this.Get(db => ids.Contains(db.ID))
                .ToList();
            this._entities.Value.RemoveRange(formDB);
        }

       
        /// <summary> 
        /// Merge entity instances - smartly delete/update/insert by IDs.
        /// </summary>
        /// <typeparam name="T">Entity.</typeparam>
        /// <param name="newCollection">Actual list of entities to merge into database.</param>
        /// <param name="mergeScope">Selector for DB entity instances to merge with actual list.</param>
        public void Merge(IEnumerable<T> newCollection, Expression<Func<T, bool>> mergeScope)
        {
            IEnumerable<T> oldCollection = this.Get(mergeScope).ToList();

            //entity instances with IDs not from DB should be inserted there
            IEnumerable<T> toInsert = newCollection.Where(@new => !oldCollection.Any(old => old.ID == @new.ID));
            this.Insert(toInsert);

            //those, which are not inserted - they are to be updated
            IEnumerable<T> toUpdate = newCollection.Where(@new => !toInsert.Contains(@new));
            this.Update(toUpdate);

            //entity instances from DB which not presented in current collection should be deleted
            IEnumerable<T> toDelete = oldCollection.Where(old => !newCollection.Any(@new => @new.ID == old.ID));
            this.Delete(toDelete);
        }

        #endregion

    }
}
