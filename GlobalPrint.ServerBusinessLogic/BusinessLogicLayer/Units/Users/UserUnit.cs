using GlobalPrint.Infrastructure.EmailUtility;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Offers;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Offers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Offers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Users
{
    public class UserUnit : BaseUnit
    {
        private Lazy<IEmailUtility> _emailUtility { get; set; }

        [DebuggerStepThrough]
        public UserUnit(Lazy<IEmailUtility> emailUtility)
            :base()
        {
            _emailUtility = emailUtility;
        }

        public User GetUserByID(int userID)
        {
            using (IDataContext context = this.Context())
            {
                return this.Repository<IUserRepository>(context)
                    .GetByID(userID);
            }
        }

        /// <summary>
        /// Get user with latest signed user offer.
        /// </summary>
        /// <param name="userID">User identifier.</param>
        /// <returns>User with latest signed user offer.</returns>
        public UserExtended GetExtendedUserByID(int userID)
        {
            using (IDataContext context = this.Context())
            {
                IUserRepository userRepository = this.Repository<IUserRepository>(context);
                UserOfferUnit userOfferUnit = new UserOfferUnit();

                User user = userRepository.GetByID(userID);
                var latestUserOfferExtended = userOfferUnit.GetLatestUserOfferByUserID(userID, OfferTypeEnum.UserOffer, context);
                
                return new UserExtended()
                {
                    User = user,
                    LatestUserOffer = latestUserOfferExtended?.LatestUserOffer
                };
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
        /// Update user profile information
        /// </summary>
        /// <param name="user">User profile info</param>
        /// <returns>Updated user profile info</returns>
        public IUserProfile UpdateUserProfile(IUserProfile user)
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

                    // send email to user about up balance
                    MailAddress userMail = new MailAddress(originalUser.Email, originalUser.UserName);
                    string userMessageBody = string.Format(
                        "Ваш баланс пополнен на {0} руб. и теперь составляет {1} руб.",
                        upSumm.ToString("0.00"),
                        originalUser.AmountOfMoney.ToString("0.00")
                    );
                    _emailUtility.Value.Send(userMail, "Global Print - Пополнение баланса", userMessageBody);

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
