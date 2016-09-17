using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Users
{
    /// <summary>
    /// Interface only for user profile without authentification fields
    /// </summary>
    public interface IUserProfile
    {
        int UserID { get; set; }
        string UserName { get; set; }
        string Email { get; set; }
        string PhoneNumber { get; set; }
        decimal AmountOfMoney { get; set; }
    }
}
