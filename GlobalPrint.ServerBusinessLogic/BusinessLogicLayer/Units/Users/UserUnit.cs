using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Users
{
    public class UserUnit : BaseUnit
    {
        public User GetUserByID(int UserID)
        {
            using (IDataContext context = this.Context())
            {
                return this.Repository<IUserRepository>(context)
                    .GetByID(UserID);
            }
        }
        public User GetUserByFilter(Expression<Func<User, bool>> filter)
        {
            using (IDataContext context = this.Context())
            {
                return this.Repository<IUserRepository>(context)
                    .Get(filter)
                    .SingleOrDefault();
            }
        }

        public User SaveUser(User user)
        {
            using (IDataContext context = this.Context())
            {
                IUserRepository userRepo = this.Repository<IUserRepository>(context);
                User originalUser = userRepo
                    .Get(x => x.UserID == user.UserID)
                    .SingleOrDefault();

                if (originalUser != null)
                {
                    userRepo.Update(user);
                    //db.Entry(originalUser).CurrentValues.SetValues(user);
                    //db.SaveChanges();
                    return user;
                }
                else
                {
                    throw new Exception("Не найден пользователь [ID=" + user.UserID + "]");
                }
            }
        }

        public User InsertUser(User user)
        {
            using (IDataContext context = this.Context())
            {
                IUserRepository userRepo = this.Repository<IUserRepository>(context);
                userRepo.Insert(user);
                return user;
            }
        }
        public User DeleteUser(User user)
        {
            using (IDataContext context = this.Context())
            {
                IUserRepository userRepo = this.Repository<IUserRepository>(context);
                userRepo.Delete(user);
                return user;
            }
        }


        public User FillUpBalance(int userID, decimal upSumm)
        {
            using (IDataContext context = this.Context())
            {
                IUserRepository userRepo = this.Repository<IUserRepository>(context);
                User originalUser = userRepo
                    .Get(x => x.UserID == userID)
                    .SingleOrDefault();

                if (originalUser != null)
                {
                    originalUser.AmountOfMoney += upSumm;
                    userRepo.Update(originalUser);
                    //db.Entry(originalUser).CurrentValues.SetValues(originalUser);
                    //db.SaveChanges();
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
