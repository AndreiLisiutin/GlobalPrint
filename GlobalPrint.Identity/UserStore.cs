using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Identity
{
    public class UserStore<TUser> : IUserStore<TUser, int> where TUser: IdentityUser
    {
        public Task CreateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult<object>(null);

            //userTable.Insert(user);

            //return Task.FromResult<object>(null);
        }

        public Task DeleteAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("Null or empty argument: user");
            }

            //using (var db = new DB())
            //{
            //    db.Users.Remove(user);
            //}
            return Task.FromResult<object>(null);
        }

        public Task<TUser> FindByIdAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("Empty argument: userId");
            }

            //TUser result = userTable.GetUserById(userId) as TUser;
            //if (result != null)
            //{
            //    return Task.FromResult<TUser>(result);
            //}

            //return Task.FromResult<TUser>(null);
            return Task.FromResult<TUser>(null);
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("Null or empty argument: userName");
            }
            return Task.FromResult<TUser>(null);

            //List<TUser> result = userTable.GetUserByName(userName) as List<TUser>;

            //// Should I throw if > 1 user?
            //if (result != null && result.Count == 1)
            //{
            //    return Task.FromResult<TUser>(result[0]);
            //}

            //return Task.FromResult<TUser>(null);
        }

        public Task UpdateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult<object>(null);

            //userTable.Update(user);

            //return Task.FromResult<object>(null);
        }

        public void Dispose()
        {

        }
    }

}
