using GlobalPrint.Infrastructure.EmailUtility;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Offers;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.Models.Business.Users;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Offers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Offers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Users;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Printers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Printers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Printers;
using GlobalPrint.ServerBusinessLogic.Models.Business.Printers;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Users
{
    public class UserUnit : BaseUnit, IUserUnit
    {
        private Lazy<IEmailUtility> _emailUtility { get; set; }

        [DebuggerStepThrough]
        public UserUnit(Lazy<IEmailUtility> emailUtility)
            : base()
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
                    LatestUserOffer = latestUserOfferExtended
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

                User originalUser = userRepo.GetByID(user.ID);
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
                    throw new Exception("Не найден пользователь [ID=" + user.ID + "]");
                }
            }
        }

        /// <summary>
        /// Method only for User store
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public User UpdateUser(User user)
        {
            using (IDataContext context = this.Context())
            {
                IUserRepository userRepo = this.Repository<IUserRepository>(context);
                User originalUser = userRepo.GetByID(user.ID);

                if (originalUser != null)
                {
                    userRepo.Update(user);
                    context.Save();
                    return user;
                }
                else
                {
                    throw new Exception("Не найден пользователь [ID=" + user.ID + "]");
                }
            }
        }

        /// <summary>
        /// Insert new user and sing his offer in transaction.
        /// </summary>
        /// <param name="user">User to insert.</param>
        /// <returns>Inserted user with new ID.</returns>
        public User InsertUserWithOffer(User user)
        {
            using (IDataContext context = this.Context())
            {
                context.BeginTransaction();
                try
                {
                    IUserRepository userRepo = this.Repository<IUserRepository>(context);
                    UserOfferUnit userOfferUnit = new UserOfferUnit();

                    // Insert new user
                    userRepo.Insert(user);
                    context.Save();

                    // Create user offer
                    userOfferUnit.CreateUserOfferInTransaction(user.ID, OfferTypeEnum.UserOffer, context);

                    context.Save();
                    context.CommitTransaction();
                }
                catch (Exception)
                {
                    context.RollbackTransaction();
                    throw;
                }
                return user;
            }
        }

        /// <summary>
        /// Just insert new user.
        /// </summary>
        /// <param name="user">User to insert.</param>
        /// <returns>Inserted user with new ID.</returns>
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

        /// <summary>
        /// Update last user activity.
        /// </summary>
        /// <param name="userID">User identifier.</param>
        /// <returns>User instance.</returns>
        public User UpdateUserActivity(int userID)
        {
            using (IDataContext context = this.Context())
            {
                IUserRepository userRepo = this.Repository<IUserRepository>(context);
                User originalUser = userRepo.GetByID(userID);

                if (originalUser != null)
                {
                    originalUser.LastActivityDate = DateTime.Now;
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

        /// <summary>
        /// Return all the inactive users during "threshold" period.
        /// </summary>
        /// <param name="threshold">Period from to search inactive users.</param>
        /// <param name="callInterval">Period to to search inactive users.</param>
        /// <returns>List of inactive users.</returns>
        public List<User> GetInactiveUsers(TimeSpan threshold, TimeSpan callInterval)
        {
            // Now - threshold (30 min)
            DateTime intervalTo = DateTime.Now.Subtract(threshold);
            DateTime intervalFrom = intervalTo.Subtract(callInterval);

            using (IDataContext context = this.Context())
            {
                IUserRepository userRepo = this.Repository<IUserRepository>(context);
                IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);
                PrinterUnit printerUnit = new PrinterUnit(); 

                // LastActivityDate > (Now - threshold)
                var list = userRepo.Get(e => e.EmailConfirmed == true // user activated his account
                        && e.LastActivityDate > intervalFrom // user was active between (Now - threshold) and (Now - threshold - callInterval) ~ 30-35 min before
                        && e.LastActivityDate < intervalTo)
                    .Join(printerRepo.GetAll(), u => u.ID, p => p.OperatorUserID, (u, p) => new { User = u, Printer = p })
                    .Where(p => p.Printer != null && !p.Printer.IsDisabled)
                    .ToList();

                // Build total list to notify users
                var totalList = new List<User>();
                foreach(var printer in list)
                {
                    PrinterFullInfoModel currentPrinter = printerUnit.GetFullByID(printer.Printer.ID, context);
                    if (currentPrinter.IsAvailableNow && totalList.FindAll(x => x.ID == printer.User.ID).Count == 0)
                    {
                        totalList.Add(printer.User);
                    }
                }

                return totalList;
            }
        }
    }
}
