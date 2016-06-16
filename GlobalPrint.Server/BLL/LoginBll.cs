using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Server
{
    public class LoginBll
    {
        public List<User> CheckLogin(string login, string password)
        {
            using (var db = new DB())
            {
                return db.Users.Where(x => x.Login == login && x.Password == password).ToList();
            }
        }

        public User Register(string name, string login, string password)
        {
            using (var db = new DB())
            {
                List<User> usersWithSameLogin = this.CheckLogin(login, password);
                if (usersWithSameLogin.Count > 0)
                {
                    return null;
                }

                User newUser = new User()
                {
                    Email = login,
                    Name = name,
                    Login = login,
                    Password = password
                };
                return db.Users.Add(newUser);
            }
        }
    }
}
