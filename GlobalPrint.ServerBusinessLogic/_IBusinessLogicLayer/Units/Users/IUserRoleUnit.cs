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
    public interface IUserRoleUnit
    {
        UserRole GetByID(int userRoleID);
        List<UserRole> GetByFilter(Expression<Func<UserRole, bool>> filter);
        List<string> GetUserRoles(int userID);
        UserRole Save(UserRole model);
        void Delete(int userRoleID);
    }
}
