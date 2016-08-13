using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users
{
    public interface IUserRoleRepository : IRepository<UserRole>
    {
    }
}
