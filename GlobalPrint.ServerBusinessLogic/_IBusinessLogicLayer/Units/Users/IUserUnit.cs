using GlobalPrint.Infrastructure.CommonUtils.Pagination;
using GlobalPrint.ServerBusinessLogic.Models.Business.Printers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Users
{
    /// <summary>
    /// Интерфейс модуля бизнес логики для пользователя.
    /// </summary>
    public interface IUserUnit
    {
        User GetByID(int userID);
        User GetByFilter(Expression<Func<User, bool>> filter);
        List<User> GetByFilter(string filter, Paging paging = null);
        int CountByFilter(string filter);
        IUserProfile UpdateUserProfile(IUserProfile user);
        User InsertUser(User user);
        User DeleteUser(User user);
        List<PrinterOperatorModel> GetInactiveUsers(TimeSpan threshold, TimeSpan callInterval);
        User UpdateUserActivity(int userID, DateTime? lastActivityDate = null);
    }

    //public interface IUserProfileUnit : IUserUnit
    //{
    //    IUserProfile UpdateUserProfile(IUserProfile user);
    //    User UpdateUserActivity(int userID, DateTime? lastActivityDate = null);
    //    List<PrinterOperatorModel> GetInactiveUsers(TimeSpan threshold, TimeSpan callInterval);
    //}

    //public interface IUserAccount : IUserUnit
    //{
    //    User InsertUser(User user);
    //    User UpdateUser(User user);
    //    User DeleteUser(User user);
    //}
}
