using GlobalPrint.Server;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ClientWeb
{
    public class UserStore<TUser> : IUserStore<TUser, int>,
        IUserLockoutStore<TUser, int>,
        IUserPasswordStore<TUser, int>,
        IUserTwoFactorStore<TUser, int>
        where TUser : IdentityUser
    {
        private ApplicationDbContext _proxyContext { get; set; }

        public UserStore(ApplicationDbContext context)
        {
            this._proxyContext = context;
        }

        #region IUserStore

        public Task CreateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("Null or empty argument: user");
            }

            var userToAdd = user.ToDbUser();
            userToAdd.Login = userToAdd.Name;
            userToAdd.Phone = userToAdd.Phone ?? "";

            using (var _context = new DB())
            {
                var savedUser = _context.Users.Add(userToAdd);
                _context.SaveChanges();

                user.Id = savedUser.UserID;
                return Task.FromResult<object>(user);
            }                
        }

        public Task DeleteAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("Null or empty argument: user");
            }


            using (var _context = new DB())
            {
                var userToRemove = _context.Users.SingleOrDefault(x => x.UserID == user.Id);
                if (userToRemove != null)
                {
                    _context.Users.Remove(userToRemove);
                }

                return Task.FromResult<object>(null);
            }
        }

        public Task<TUser> FindByIdAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("Empty argument: userId");
            }


            using (var _context = new DB())
            {
                var result = IdentityUser.FromDbUser(_context.Users.SingleOrDefault(x => x.UserID == userId)) as TUser;
                if (result != null)
                {
                    return Task.FromResult<TUser>(result);
                }

                return Task.FromResult<TUser>(null);
            }
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("Null or empty argument: userName");
            }

            using (var _context = new DB())
            {
                var temp = _context.Users.SingleOrDefault(x => x.Login == userName || x.Email == userName);
                var result = IdentityUser.FromDbUser(temp) as TUser;
                if (result != null)
                {
                    return Task.FromResult<TUser>(result);
                }

                return Task.FromResult<TUser>(null);
            }
        }

        public Task UpdateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("Null or empty argument: user");
            }

            using (var _context = new DB())
            {
                var original = _context.Users.SingleOrDefault(x => x.UserID == user.Id);
                if (original != null)
                {
                    _context.Entry(original).CurrentValues.SetValues(user);
                    _context.SaveChanges();
                }

                return Task.FromResult<object>(user);
            }
        }

        public void Dispose()
        {

        }

        #endregion

        #region LockoutStore
        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            return Task.FromResult(0);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            return Task.Factory.StartNew<bool>(() => false);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task ResetAccessFailedCountAsync(TUser user)
        {
            return Task.FromResult(0);
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IUserPasswordStore

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentException("Empty argument: user");
            }

            using (var _context = new DB())
            {
                var result = IdentityUser.FromDbUser(_context.Users.SingleOrDefault(x => x.UserID == user.Id)) as TUser;
                if (result != null)
                {
                    return Task.FromResult<string>(result.PasswordHash);
                }
                return Task.FromResult<string>(null);
            }
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentException("Empty argument: user");
            }

            using (var _context = new DB())
            {
                var result = IdentityUser.FromDbUser(_context.Users.SingleOrDefault(x => x.UserID == user.Id)) as TUser;
                var hasPassword = !string.IsNullOrEmpty(result.PasswordHash);

                return Task.FromResult<bool>(hasPassword);
            }
        }

        #endregion

        #region IUserTwoFactorStore

        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            return Task.FromResult(false);
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}
