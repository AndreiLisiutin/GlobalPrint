using GlobalPrint.Infrastructure.EmailUtility;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
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
using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.Infrastructure.CommonUtils.Pagination;

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

        public User GetByID(int userID)
        {
            using (IDataContext context = this.Context())
            {
                return this.Repository<IUserRepository>(context)
                    .GetByID(userID);
            }
        }
        
        /// <summary>
        /// Get single user by filter.
        /// </summary>
        /// <param name="filter">Filter on user.</param>
        /// <returns>Single found user.</returns>
        public User GetByFilter(Expression<Func<User, bool>> filter)
        {
            using (IDataContext context = this.Context())
            {
                return this.Repository<IUserRepository>(context)
                    .Get(filter)
                    .SingleOrDefault();
            }
        }

        /// <summary>
        /// Get users by filter and paging.
        /// </summary>
        /// <param name="filter">Filter to user.</param>
        /// <param name="paging">Pagination information.</param>
        /// <returns>Found users by filter criteria and paging.</returns>
        public List<User> GetByFilter(string filter, Paging paging = null)
        {
            int skip = paging?.Skip ?? 0;
            using (IDataContext context = this.Context())
            {
                var query = this.Repository<IUserRepository>(context)
                    .GetAll();

                if (filter != null)
                {
                    filter = filter.ToLower();
                    query = query
                        .Where(e => e.Email.ToLower().Contains(filter));
                }

                query = query.OrderBy(e => e.Email);
                if (paging != null)
                {
                    query = query
                        .Skip(skip)
                        .Take(paging.ItemsPerPage);
                }

                return query.ToList();
            }
        }

        /// <summary>
        /// Get users count by filter.
        /// </summary>
        /// <param name="filter">Filter to user.</param>
        /// <returns>Count of users found by filter criteria.</returns>
        public int CountByFilter(string filter)
        {
            using (IDataContext context = this.Context())
            {
                var query = this.Repository<IUserRepository>(context)
                   .GetAll();

                if (filter != null)
                {
                    query = query
                        .Where(e => e.Email.Contains(filter));
                }
                return query.Count();
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

                    originalUser.BankBic = user.BankBic;
                    originalUser.BankCorrespondentAccount = user.BankCorrespondentAccount;
                    originalUser.BankName = user.BankName;
                    originalUser.Inn = user.Inn;
                    originalUser.Kpp = user.Kpp;
                    originalUser.LegalAddress = user.LegalAddress;
                    originalUser.LegalFullName = user.LegalFullName;
                    originalUser.LegalShortName = user.LegalShortName;
                    originalUser.Ogrn = user.Ogrn;
                    originalUser.PaymentAccount = user.PaymentAccount;
                    originalUser.PostAddress = user.PostAddress;

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
        /// Just insert new user.
        /// </summary>
        /// <param name="user">User to insert.</param>
        /// <returns>Inserted user with new ID.</returns>
        public User InsertUser(User user)
        {
            using (IDataContext context = this.Context())
            {
                IUserRepository userRepo = this.Repository<IUserRepository>(context);
                user.CreatedOn = DateTime.Now;
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
        
        /// <summary>
        /// Update last user activity date.
        /// </summary>
        /// <param name="userID">User identifier.</param>
        /// <param name="lastActivityDate">Last user activity date. Optional parameter, default is DateTime.Now.</param>
        /// <returns>User instance.</returns>
        public User UpdateUserActivity(int userID, DateTime? lastActivityDate = null)
        {
            DateTime _lastActivityDate = lastActivityDate.GetValueOrDefault(DateTime.Now);

            using (IDataContext context = this.Context())
            {
                IUserRepository userRepo = this.Repository<IUserRepository>(context);
                User originalUser = userRepo.GetByID(userID);

                if (originalUser != null)
                {
                    originalUser.LastActivityDate = _lastActivityDate;
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
        public List<PrinterOperatorModel> GetInactiveUsers(TimeSpan threshold, TimeSpan callInterval)
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
                    .Join(printerRepo.GetAll(), u => u.ID, p => p.OperatorUserID, (u, p) => new { PrinterOperator = u, Printer = p })
                    .Where(p => p.Printer != null && !p.Printer.IsDisabled)
                    .ToList();

                // Build total list to notify users
                List<PrinterOperatorModel> totalList = new List<PrinterOperatorModel>();
                foreach (var item in list)
                {
                    PrinterFullInfoModel currentPrinter = printerUnit.GetFullByID(item.Printer.ID, context);
                    if (currentPrinter.IsAvailableNow && totalList.FindAll(x => x.PrinterOperator.ID == item.PrinterOperator.ID).Count == 0)
                    {
                        totalList.Add(new PrinterOperatorModel() { Printer = item.Printer, PrinterOperator = item.PrinterOperator });
                    }
                }
                
                return totalList;
            }
        }
    }
}
