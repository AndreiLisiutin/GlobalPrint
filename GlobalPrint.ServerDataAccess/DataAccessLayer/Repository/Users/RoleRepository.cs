using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Users;
using GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext;
using GlobalPrint.ServerDataAccess.DataAccessLayer.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Users
{
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        public RoleRepository(DbConnectionContext context) : base(context)
        {
        }
    }
}
