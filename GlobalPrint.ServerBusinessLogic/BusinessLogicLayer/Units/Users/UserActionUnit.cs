using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Users
{
    public class UserActionUnit : BaseUnit
    {
        public UserActionUnit()
        {
        }
        /// <summary>
        /// Логировать действие пользователя
        /// </summary>
        /// <param name="obj">Объект с логом</param>
        public void AddUserActionLog(UserActionLog obj)
        {
            using (IDataContext context = this.Context())
            {
                IUserActionLogRepository usreActionLogRepo = this.Repository<IUserActionLogRepository>(context);
                usreActionLogRepo.Insert(obj);
                context.Save();
            }
        }

    }
}
