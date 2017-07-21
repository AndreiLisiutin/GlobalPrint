using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Users;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Users;
using GlobalPrint.Infrastructure.CommonUtils.ExtensionMethods;

namespace GlobalPrint.ClientWeb
{
    public class UserStore :
        IUserStore<ApplicationUser, int>,
        IUserLockoutStore<ApplicationUser, int>,
        IUserPasswordStore<ApplicationUser, int>,
        IUserTwoFactorStore<ApplicationUser, int>,
        IUserPhoneNumberStore<ApplicationUser, int>,
        IUserEmailStore<ApplicationUser, int>,
        IUserSecurityStampStore<ApplicationUser, int>,
        IUserRoleStore<ApplicationUser, int>
    {
        private UserUnit _userUnit { get; set; }
        private IRoleUnit _roleUnit { get; set; }
        private IUserRoleUnit _userRoleUnit { get; set; }

        public UserStore(UserUnit userUnit, IRoleUnit roleUnit, IUserRoleUnit userRoleUnit)
        {
            this._userUnit = userUnit;
            this._roleUnit = roleUnit;
            this._userRoleUnit = userRoleUnit;
        }

        #region IUserStore

        public Task CreateAsync(ApplicationUser user)
        {
            if (user == null || user.User == null)
            {
                throw new ArgumentNullException("user");
            }

            this._userUnit.InsertUser(user.User);
            return Task.FromResult<object>(user);
        }

        public Task DeleteAsync(ApplicationUser user)
        {
            if (user == null || user.User == null)
            {
                throw new ArgumentNullException("user");
            }

            var userToRemove = this._userUnit.GetByID(user.Id);
            if (userToRemove != null)
            {
                this._userUnit.DeleteUser(userToRemove);
            }

            return Task.FromResult<object>(null);
        }

        public Task<ApplicationUser> FindByIdAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("userId");
            }

            User result = this._userUnit.GetByID(userId);
            if (result != null)
            {
                ApplicationUser appUser = new ApplicationUser(result);
                return Task.FromResult<ApplicationUser>(appUser);
            }
            return Task.FromResult<ApplicationUser>(null);
        }

        public Task<ApplicationUser> FindByNameAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("userName");
            }

            return this.FindByEmailAsync(userName);
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            if (user == null || user.User == null)
            {
                throw new ArgumentNullException("user");
            }

            this._userUnit.UpdateUser(user.User);

            return Task.FromResult<object>(user);
        }

        public void Dispose()
        {
        }

        #endregion

        #region IUserLockoutStore

        public Task<int> GetAccessFailedCountAsync(ApplicationUser user)
        {
            return Task.FromResult(0);
        }

        public Task<bool> GetLockoutEnabledAsync(ApplicationUser user)
        {
            return Task.Factory.StartNew<bool>(() => false);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task<int> IncrementAccessFailedCountAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task ResetAccessFailedCountAsync(ApplicationUser user)
        {
            return Task.FromResult(0);
        }

        public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset lockoutEnd)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IUserPasswordStore

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
        {
            if (user == null || user.User == null)
            {
                throw new ArgumentNullException("user");
            }
            if (string.IsNullOrEmpty(passwordHash))
            {
                throw new ArgumentNullException("passwordHash");
            }

            user.User.PasswordHash = passwordHash;
            //UpdateAsync(user);

            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user)
        {
            if (user == null || user.User == null)
            {
                throw new ArgumentException("user");
            }

            User result = this._userUnit.GetByID(user.Id);
            if (result != null)
            {
                return Task.FromResult<string>(result.PasswordHash);
            }
            return Task.FromResult<string>(null);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            if (user == null || user.User == null)
            {
                throw new ArgumentException("user");
            }

            User result = this._userUnit.GetByID(user.Id);
            bool hasPassword = !string.IsNullOrEmpty(result.PasswordHash);
            return Task.FromResult<bool>(hasPassword);
        }

        #endregion

        #region IUserTwoFactorStore

        public Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user)
        {
            return Task.FromResult(false);
        }

        public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IUserPhoneNumberStore

        public Task SetPhoneNumberAsync(ApplicationUser user, string phoneNumber)
        {
            if (user == null || user.User == null)
            {
                throw new ArgumentException("user");
            }

            user.User.PhoneNumber = phoneNumber;
            UpdateAsync(user);

            return Task.FromResult(user);
        }

        public Task<string> GetPhoneNumberAsync(ApplicationUser user)
        {
            if (user == null || user.User == null)
            {
                throw new ArgumentException("user");
            }

            return Task.FromResult(user.User.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user)
        {
            if (user == null || user.User == null)
            {
                throw new ArgumentException("user");
            }

            return Task.FromResult(user.User.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed)
        {
            if (user == null || user.User == null)
            {
                throw new ArgumentException("user");
            }

            user.User.PhoneNumberConfirmed = confirmed;
            UpdateAsync(user);

            return Task.FromResult(0);
        }

        #endregion

        #region IUserEmailStore

        public Task SetEmailAsync(ApplicationUser user, string email)
        {
            if (user == null || user.User == null)
            {
                throw new ArgumentException("user");
            }

            user.User.Email = email;
            UpdateAsync(user);

            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(ApplicationUser user)
        {
            if (user == null || user.User == null)
            {
                throw new ArgumentException("user");
            }

            return Task.FromResult(user.User.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user)
        {
            if (user == null || user.User == null)
            {
                throw new ArgumentException("user");
            }

            return Task.FromResult(user.User.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed)
        {
            if (user == null || user.User == null)
            {
                throw new ArgumentException("user");
            }

            user.User.EmailConfirmed = confirmed;
            UpdateAsync(user);

            return Task.FromResult(0);
        }

        public Task<ApplicationUser> FindByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("email");
            }

            User result = _userUnit.GetByFilter(x => x.Email != null && email.ToUpper() == x.Email.ToUpper());
            if (result != null)
            {
                ApplicationUser appUser = new ApplicationUser(result);
                return Task.FromResult<ApplicationUser>(appUser);
            }

            return Task.FromResult<ApplicationUser>(null);
        }

        #endregion

        #region IUserSecurityStampStore

        public Task<string> GetSecurityStampAsync(ApplicationUser user)
        {
            if (user == null || user.User == null)
            {
                throw new ArgumentException("user");
            }

            return Task.FromResult<string>(user.User.SecurityStamp);
        }

        public Task SetSecurityStampAsync(ApplicationUser user, string stamp)
        {
            if (user == null || user.User == null)
            {
                throw new ArgumentException("user");
            }

            user.User.SecurityStamp = stamp;
            //UpdateAsync(user);

            return Task.FromResult(0);
        }

        #endregion

        #region IUserRoleStore

        /// <summary>
        /// Add role to user.
        /// </summary>
        /// <param name="user">User to add role.</param>
        /// <param name="roleName">Role name to add.</param>
        /// <returns>Nothing.</returns>
        public Task AddToRoleAsync(ApplicationUser user, string roleName)
        {
            if (user == null || user.User == null)
            {
                throw new ArgumentNullException("user");
            }
            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("roleName");
            }

            int roleID = _roleUnit.GetRoleID(roleName);
            UserRole userRole = new UserRole()
            {
                RoleID = roleID,
                UserID = user.Id
            };

            _userRoleUnit.Save(userRole);

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Get role names of user.
        /// </summary>
        /// <param name="user">User to find roles.</param>
        /// <returns>Role names list.</returns>
        public Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            if (user == null || user.User == null)
            {
                throw new ArgumentNullException("user");
            }

            List<string> roles = _userRoleUnit.GetUserRoles(user.Id);
            if (roles != null)
            {
                return Task.FromResult<IList<string>>(roles);
            }

            return Task.FromResult<IList<string>>(null);
        }

        /// <summary>
        /// Is user has role.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="roleName">Role name.</param>
        /// <returns>Flag - is user has role.</returns>
        public Task<bool> IsInRoleAsync(ApplicationUser user, string roleName)
        {
            if (user == null || user.User == null)
            {
                throw new ArgumentNullException("user");
            }
            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException("roleName");
            }

            List<string> roles = _userRoleUnit.GetUserRoles(user.Id);
            if (roles != null && roles.Contains(roleName))
            {
                return Task.FromResult<bool>(true);
            }

            return Task.FromResult<bool>(false);
        }

        /// <summary>
        /// Remove user roles.
        /// </summary>
        /// <param name="user">User to remove role.</param>
        /// <param name="roleName">Role name.</param>
        /// <returns></returns>
        public Task RemoveFromRoleAsync(ApplicationUser user, string roleName)
        {
            if (user == null || user.User == null)
            {
                throw new ArgumentNullException("user");
            }
            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("roleName");
            }

            int roleID = _roleUnit.GetRoleID(roleName);
            var userRole = _userRoleUnit.GetByFilter(x => x.UserID == user.Id && x.RoleID == roleID).ToList();
            if (userRole != null && userRole.Count > 0)
            {
                _userRoleUnit.Delete(userRole[0].ID);
            }
            
            return Task.FromResult<object>(null);
        }

        #endregion

    }

}
