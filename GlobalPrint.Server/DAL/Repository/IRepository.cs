using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Server.DAL
{
    /// <summary>
    /// Интерфейс общего репозитория
    /// </summary>
    /// <typeparam name="T">Тип репозитория</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Получить объекты по фильтру
        /// </summary>
        IQueryable<T> Get(Expression<Func<T, bool>> filter);

        /// <summary>
        /// Получить список всех объектов
        /// </summary>
        IQueryable<T> GetAll();

        /// <summary>
        /// Получить объект по ID
        /// </summary>
        T GetByID(object id);

        /// <summary>
        /// Вставить новый объект
        /// </summary>
        void Insert(T entity);

        /// <summary>
        /// Обновить объект
        /// </summary>
        void Update(T entity);

        /// <summary>
        /// Удалить объект по ID
        /// </summary>
        void Delete(object id);

        /// <summary>
        /// Удалить объект
        /// </summary>
        void Delete(T entity);
    }
}
