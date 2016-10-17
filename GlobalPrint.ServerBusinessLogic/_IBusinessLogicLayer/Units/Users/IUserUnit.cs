using GlobalPrint.ServerBusinessLogic.Models.Business.Printers;
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
        User GetByID(int userID);
        User GetByFilter(Expression<Func<User, bool>> filter);
        IUserProfile UpdateUserProfile(IUserProfile user);
        User InsertUser(User user);
        User DeleteUser(User user);
        User UpdateUserActivity(int userID, DateTime? lastActivityDate = null);
        List<PrinterOperatorModel> GetInactiveUsers(TimeSpan threshold, TimeSpan callInterval);
    }
}
