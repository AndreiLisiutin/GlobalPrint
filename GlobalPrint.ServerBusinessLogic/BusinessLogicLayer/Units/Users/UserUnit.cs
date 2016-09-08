using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Users
{
    public class UserUnit : BaseUnit
    {
        [DebuggerStepThrough]
        public UserUnit()
            :base()
        {
        }
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

        /// <summary>
        /// Update user account information
        /// </summary>
        /// <param name="user">User account info</param>
        /// <returns>Updated uer account info</returns>
        public IUserAccount UpdateUserAccount(IUserAccount user)
        {
            using (IDataContext context = this.Context())
            {
                IUserRepository userRepo = this.Repository<IUserRepository>(context);

                User originalUser = userRepo.GetByID(user.UserID);
                if (originalUser != null)
                {
                    originalUser.UserName = user.UserName;
                    originalUser.Email = user.Email;
                    originalUser.PhoneNumber = user.PhoneNumber;
                    originalUser.AmountOfMoney = user.AmountOfMoney;

                    userRepo.Update(originalUser);
                    context.Save();
                    return originalUser;
                }
                else
                {
                    throw new Exception("Не найден пользователь [ID=" + user.UserID + "]");
                }
            }
        }
        
        /// <summary>
        /// Method only for 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public User UpdateUser(User user)
        {
            using (IDataContext context = this.Context())
            {
                IUserRepository userRepo = this.Repository<IUserRepository>(context);
                User originalUser = userRepo.GetByID(user.UserID);

                if (originalUser != null)
                {
                    userRepo.Update(user);
                    context.Save();
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
                context.Save();
                return user;
            }
        }

        public User DeleteUser(User user)
        {
            using (IDataContext context = this.Context())
            {
                IUserRepository userRepo = this.Repository<IUserRepository>(context);
                userRepo.Delete(user);
                context.Save();
                return user;
            }
        }


        public User FillUpBalance(int userID, decimal upSumm)
        {
            using (IDataContext context = this.Context())
            {
                IUserRepository userRepo = this.Repository<IUserRepository>(context);
                User originalUser = userRepo.GetByID(userID);

                if (originalUser != null)
                {
                    originalUser.AmountOfMoney += upSumm;
                    userRepo.Update(originalUser);
                    context.Save();
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
