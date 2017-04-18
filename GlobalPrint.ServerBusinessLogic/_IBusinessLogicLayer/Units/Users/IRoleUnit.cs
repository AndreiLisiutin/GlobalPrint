using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using System;
using System.Linq.Expressions;

namespace GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Users
{
    /// <summary>
    /// Интерфейс модуля для работы с ролями пользователя.
    /// </summary>
    public interface IRoleUnit
    {
        /// <summary>
        /// Получить роль по идентификатору.
        /// </summary>
        /// <param name="roleID">Идентификатор роли.</param>
        /// <returns>Найденная роль.</returns>
        Role GetByID(int roleID);

        /// <summary>
        /// Получить роль по фильтру.
        /// </summary>
        /// <param name="filter">Фильтр ролей.</param>
        /// <returns>Найденная роль.</returns>
        Role GetByFilter(Expression<Func<Role, bool>> filter);

        /// <summary>
        /// Получить идентификатор роли по ее названию.
        /// </summary>
        /// <param name="roleName">Название роли.</param>
        /// <returns>Идентификатор роли.</returns>
        int GetRoleID(string roleName);

        /// <summary>
        /// Сохранить роль в базе данных.
        /// </summary>
        /// <param name="role">Роль для сохранения.</param>
        /// <returns>Сохраненная роль.</returns>
        Role Save(Role role);

        /// <summary>
        /// Удалить роль из базы данных.
        /// </summary>
        /// <param name="role">Роль для удаления.</param>
        void Delete(Role role);
    }
}
