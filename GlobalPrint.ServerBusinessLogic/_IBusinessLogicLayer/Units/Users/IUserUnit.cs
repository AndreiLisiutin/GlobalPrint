using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Users
{
    public interface IUserUnit
    {
        User GetUserByID(int userID);
        User GetUserByFilter(Expression<Func<User, bool>> filter);
        IUserProfile UpdateUserProfile(IUserProfile user);
        User InsertUserWithOffer(User user);
        User InsertUser(User user);
        User DeleteUser(User user);
        User FillUpBalance(int userID, decimal upSumm);
        User UpdateUserActivity(int userID);
        List<User> GetInactiveUsers(TimeSpan threshold, TimeSpan callInterval);
    }
}
