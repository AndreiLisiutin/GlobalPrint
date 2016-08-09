using GlobalPrint.Server.DAL;
using GlobalPrint.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Server.BLL
{
    public class UserActionLogBll
    {
        /// <summary>
        /// Логировать действие пользователя
        /// </summary>
        /// <param name="obj">Объект с логом</param>
        public void AddUserActionLog(UserActionLog obj)
        {
            using (var db = new DB())
            {
                new UserActionLogRepository(db).Insert(obj);
            }
        }
    }
}
