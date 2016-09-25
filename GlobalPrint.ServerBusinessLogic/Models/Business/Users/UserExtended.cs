using GlobalPrint.ServerBusinessLogic.Models.Business.Offers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Offers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.Models.Business.Users
{
    /// <summary>
    /// Model of user extended with latest user offer.
    /// </summary>
    public class UserExtended
    {
        public User User { get; set; }
        public UserOfferExtended LatestUserOffer { get; set; }
    }
}
