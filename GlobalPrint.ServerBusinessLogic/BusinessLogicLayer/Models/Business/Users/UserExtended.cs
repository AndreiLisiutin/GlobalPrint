using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Offers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Users
{
    public class UserExtended
    {
        public User User { get; set; }
        public UserOffer LatestUserOffer { get; set; }
        public string UserOfferString
        {
            get
            {
                if (LatestUserOffer != null)
                {
                    return string.Format(
                        "Договор оферты пользователя № {0} от {1}",
                        LatestUserOffer.OfferNumber ?? "{Б/Н}",
                        LatestUserOffer.OfferDate.ToString("dd.MM.yyyy")
                    );
                }
                return string.Empty;
            }
        }
    }
}
