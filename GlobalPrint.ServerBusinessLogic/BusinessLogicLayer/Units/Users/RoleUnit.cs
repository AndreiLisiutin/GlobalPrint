using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.Infrastructure.CommonUtils.Pagination;
using GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Users;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Users
{
    /// <summary>
    /// Identity roles unit.
    /// </summary>
    public class RoleUnit : BaseUnit, IRoleUnit
    {
        [DebuggerStepThrough]
        public RoleUnit()
            : base()
        {

        }

        /// <summary>
        /// Get role by ID.
        /// </summary>
        /// <param name="roleID">Role identifier.</param>
        /// <returns>Found role.</returns>
        public Role GetByID(int roleID)
        {
            using (IDataContext context = this.Context())
            {
                return this.Repository<IRoleRepository>(context)
                    .GetByID(roleID);
            }
        }

        /// <summary>
        /// Get role by custom filter.
        /// </summary>
        /// <param name="filter">Filter to role.</param>
        /// <returns>Found roles by filter criteria.</returns>
        public Role GetByFilter(Expression<Func<Role, bool>> filter)
        {
            using (IDataContext context = this.Context())
            {
                return this.Repository<IRoleRepository>(context)
                    .Get(filter)
                    .SingleOrDefault();
            }
        }

        /// <summary>
        /// Get role identifier by role name.
        /// </summary>
        /// <param name="roleName">Role name.</param>
        /// <returns>Corresponding role identifier.</returns>
        public int GetRoleID(string roleName)
        {
            using (IDataContext context = this.Context())
            {
                var role = this.Repository<IRoleRepository>(context)
                    .Get(x => x.Name == roleName)
                    .FirstOrDefault();
                if (role == null)
                {
                    throw new Exception("Не найдена роль по названию \"" + roleName + "\".");
                }
                return role.ID;
            }
        }

        /// <summary>
        /// Validate user role save.
        /// </summary>
        /// <param name="model">Role instance.</param>
        /// <returns>Is role valid - object with errors.</returns>
        private Validation Validate(Role model)
        {
            Validation validation = new Validation();

            validation.NotNull(model, "Значение NULL невозможно сохранить как роль.");
            validation.NotNullOrWhiteSpace(model.Name, "Ошибка сохранения роли. Наименование роли должно быть непустым.");
            validation.Positive(model.ID, "Ошибка сохранения роли. Идентификатор роли должен быть положительным.");

            return validation;
        }
        /// <summary>
        /// Create brand new role.
        /// </summary>
        /// <param name="role">Role to create.</param>
        /// <returns>Created role.</returns>
        public Role _Insert(Role role)
        {
            this.Validate(role).ThrowExceptionIfNotValid();

            using (IDataContext context = this.Context())
            {
                IRoleRepository roleRepository = this.Repository<IRoleRepository>(context);
                roleRepository.Insert(role);
                context.Save();
                return role;
            }
        }
        /// <summary>
        /// Update role.
        /// </summary>
        /// <param name="role">Role to update.</param>
        /// <returns>Updated role.</returns>
        public Role _Update(Role role)
        {
            this.Validate(role).ThrowExceptionIfNotValid();

            using (IDataContext context = this.Context())
            {
                IRoleRepository roleRepository = this.Repository<IRoleRepository>(context);
                Role originalRole = roleRepository.GetByID(role.ID);

                if (originalRole != null)
                {
                    roleRepository.Update(role);
                    context.Save();
                    return role;
                }
                else
                {
                    throw new Exception("Не найдена роль [ID=" + originalRole.ID + "]");
                }
            }
        }

        /// <summary>
        /// Save role.
        /// </summary>
        /// <param name="role">Role to save.</param>
        /// <returns>Saved role.</returns>
        public Role Save(Role role)
        {
            if (role?.ID > 0)
            {
                return this._Insert(role);
            }
            else
            {
                return this._Update(role);
            }
        }

        /// <summary>
        /// Delete role.
        /// </summary>
        /// <param name="role">Role to delete.</param>
        public void Delete(Role role)
        {
            using (IDataContext context = this.Context())
            {
                IRoleRepository roleRepository = this.Repository<IRoleRepository>(context);
                roleRepository.Delete(role);
                context.Save();
            }
        }
    }
}
