using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Server
{
    public class UserBll
    {
        public User GetUserByID(int userID)
        {
            using (DB db = new DB())
            {
                return db.Users.First(e => e.UserID == userID);
            }
        }
    }
}
