using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Users;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;

namespace GlobalPrint.ClientWeb.Models.Auth
{
    public class RoleStore :
        IRoleStore<IdentityRole, int>
    {
        private RoleUnit _roleUnit { get; set; }

        public RoleStore(RoleUnit roleUnit)
        {
            this._roleUnit = roleUnit;
        }

        #region IRoleStore

        public Task CreateAsync(IdentityRole role)
        {
            if (role == null || role.Role == null)
            {
                throw new ArgumentNullException("role");
            }

            this._roleUnit.Save(role.Role);
            return Task.FromResult<object>(role);
        }
        
        public Task DeleteAsync(IdentityRole role)
        {
            if (role == null || role.Role == null)
            {
                throw new ArgumentNullException("role");
            }

            var roleToRemove = this._roleUnit.GetByID(role.Id);
            if (roleToRemove != null)
            {
                this._roleUnit.Delete(roleToRemove);
            }

            return Task.FromResult<object>(null);
        }

        public void Dispose()
        {
        }

        public Task<IdentityRole> FindByIdAsync(int roleId)
        {
            if (roleId <= 0)
            {
                throw new ArgumentException("roleId");
            }

            Role result = this._roleUnit.GetByID(roleId);
            if (result != null)
            {
                IdentityRole identityRole = new IdentityRole(result);
                return Task.FromResult<IdentityRole>(identityRole);
            }
            return Task.FromResult<IdentityRole>(null);
        }

        public Task<IdentityRole> FindByNameAsync(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("roleName");
            }

            Role result = this._roleUnit.GetByFilter(x => x.Name == roleName);
            if (result != null)
            {
                IdentityRole identityRole = new IdentityRole(result);
                return Task.FromResult<IdentityRole>(identityRole);
            }

            return Task.FromResult<IdentityRole>(null);
        }

        public Task UpdateAsync(IdentityRole role)
        {
            if (role == null || role.Role == null || role.Role.ID <= 0)
            {
                throw new ArgumentNullException("role");
            }

            this._roleUnit.Save(role.Role);

            return Task.FromResult<object>(role);
        }

        #endregion

    }
}