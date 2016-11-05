using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Users;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Printers;
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
    public class UserRoleUnit : BaseUnit, IUserRoleUnit
    {
        [DebuggerStepThrough]
        public UserRoleUnit()
            : base()
        {

        }

        /// <summary>
        /// Get user role by ID.
        /// </summary>
        /// <param name="userRoleID">User role identifier.</param>
        /// <returns>Found user role.</returns>
        public UserRole GetByID(int userRoleID)
        {
            using (IDataContext context = this.Context())
            {
                return this.Repository<IUserRoleRepository>(context)
                    .GetByID(userRoleID);
            }
        }

        /// <summary>
        /// Get user role by custom filter.
        /// </summary>
        /// <param name="filter">Filter to user role.</param>
        /// <returns>Found user roles by filter criteria.</returns>
        public List<UserRole> GetByFilter(Expression<Func<UserRole, bool>> filter)
        {
            using (IDataContext context = this.Context())
            {
                return this.Repository<IUserRoleRepository>(context)
                    .Get(filter)
                    .ToList();
            }
        }

        /// <summary>
        /// Get user role by custom filter.
        /// </summary>
        /// <param name="filter">Filter to user role.</param>
        /// <returns>Found user roles by filter criteria.</returns>
        public List<string> GetUserRoles(int userID)
        {
            using (IDataContext context = this.Context())
            {
                IUserRoleRepository userRoleRepository = this.Repository<IUserRoleRepository>(context);
                IRoleRepository roleRepository = this.Repository<IRoleRepository>(context);

                List<string> roles = (
                    from userRole in userRoleRepository.Get(e => e.UserID == userID)
                    join role in roleRepository.GetAll() on userRole.RoleID equals role.ID
                    select role.Name
                ).ToList();

                return roles;
            }
        }

        /// <summary>
        /// Validate save user role.
        /// </summary>
        /// <param name="model">User role instance.</param>
        /// <returns>Is user role valid - object with errors.</returns>
        private Validation Validate(UserRole model)
        {
            Validation validation = new Validation();

            validation.NotNull(model, "Значение NULL невозможно сохранить как роль пользователя.");
            validation.Positive(model.UserID, "Ошибка сохранения роли пользователя. Идентификатор пользователя должен быть положительным.");
            validation.Positive(model.RoleID, "Ошибка сохранения роли пользователя. Идентификатор роли должен быть положительным.");

            return validation;
        }

        /// <summary>
        /// Insert user role.
        /// </summary>
        /// <param name="model">User role instance.</param>
        /// <returns>Saved user role.</returns>
        private UserRole _Insert(UserRole model)
        {
            this.Validate(model).ThrowExceptionIfNotValid();

            using (IDataContext context = this.Context())
            {
                IUserRoleRepository userRoleRepo = this.Repository<IUserRoleRepository>(context);
                userRoleRepo.Insert(model);
                context.Save();
                return model;
            }
        }

        /// <summary>
        /// Update user role.
        /// </summary>
        /// <param name="model">User role instance.</param>
        /// <returns>Saved user role.</returns>
        private UserRole _Update(UserRole model)
        {
            this.Validate(model).ThrowExceptionIfNotValid();

            using (IDataContext context = this.Context())
            {
                IUserRoleRepository userRoleRepo = this.Repository<IUserRoleRepository>(context);
                UserRole originalRole = userRoleRepo.GetByID(model.ID);

                if (originalRole != null)
                {
                    userRoleRepo.Update(model);
                    context.Save();
                    return model;
                }
                else
                {
                    throw new Exception("Не найдена роль пользователя [ID=" + originalRole.ID + "]");
                }
            }
        }

        /// <summary>
        /// Save user role.
        /// </summary>
        /// <param name="model">User role instance.</param>
        /// <returns>Saved user role.</returns>
        public UserRole Save(UserRole model)
        {
            bool isEdit = (model?.ID ?? 0) > 0;
            if (isEdit)
            {
                return this._Update(model);
            }
            else
            {
                return this._Insert(model);
            }
        }

        /// <summary>
        /// Delete user role.
        /// </summary>
        /// <param name="userRoleID">User role identifier.</param>
        public void Delete(int userRoleID)
        {
            using (IDataContext context = this.Context())
            {
                IUserRoleRepository userRoleRepo = this.Repository<IUserRoleRepository>(context);
                userRoleRepo.Delete(userRoleID);
                context.Save();
            }
        }
    }
}
