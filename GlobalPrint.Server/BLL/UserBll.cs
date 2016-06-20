using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Server
{
    public class UserBll
    {
        public User GetUserByID(int UserID)
        {
            using (var db = new DB())
            {
                return db.Users.SingleOrDefault(x => x.UserID == UserID);
            }
        }

        public User GetUserByPhone(string phone)
        {
            string formattedPhone = SmsUtility.ExtractValidPhone(phone);
            using (var db = new DB())
            {
                return db.Users.SingleOrDefault(x => x.Phone == formattedPhone);
            }
        }

        public User SaveUser(User user)
        {
            using (var db = new DB())
            {
                User originalUser = db.Users.SingleOrDefault(x => x.UserID == user.UserID);
                if (originalUser != null)
                {
                    db.Entry(originalUser).CurrentValues.SetValues(user);
                    db.SaveChanges();
                    return user;
                }
                else
                {
                    throw new Exception("Не найден пользователь [ID=" + user.UserID + "]");
                }
            }
        }

        public User FillUpBalance(int userID, decimal upSumm)
        {
            using (var db = new DB())
            {
                User originalUser = db.Users.SingleOrDefault(x => x.UserID == userID);
                if (originalUser != null)
                {
                    originalUser.AmountOfMoney += upSumm;
                    db.Entry(originalUser).CurrentValues.SetValues(originalUser);
                    db.SaveChanges();
                    return originalUser;
                }
                else
                {
                    throw new Exception("Не найден пользователь [ID=" + userID + "]");
                }
            }
        }
    }
}
