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
    public interface IRoleUnit
    {
        Role GetByID(int roleID);
        Role GetByFilter(Expression<Func<Role, bool>> filter);
        int GetRoleID(string roleName);
        Role Save(Role role);
        void Delete(Role role);
    }
}
