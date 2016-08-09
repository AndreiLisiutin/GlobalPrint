using GlobalPrint.Server.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Server.DAL
{
    public class UserActionLogRepository : BaseRepository<UserActionLog>, IUserActionLogRepository
    {
        public UserActionLogRepository(DbContext context)
            : base(context)
        {
        }

        public override void Insert(UserActionLog entity)
        {
            entity.Date = DateTime.Now;
            base.Insert(entity);
        }

        public override void Update(UserActionLog entity)
        {
            entity.Date = DateTime.Now;
            base.Update(entity);
        }
    }
}
