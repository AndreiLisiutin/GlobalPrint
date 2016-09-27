using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic.Models.Business.Offers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Offers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Offers
{
    public interface IUserOfferUnit
    {
        UserOfferExtended GetLatestUserOfferByUserID(int userID, OfferTypeEnum offerType);
        UserOfferExtended GetLatestUserOfferByUserID(int userID, OfferTypeEnum offerType, IDataContext context);
        UserOffer SaveUserOffer(UserOffer model);
        UserOffer CreateUserOffer(UserOffer model);
        UserOffer EditUserOffer(UserOffer model);
        UserOffer CreateUserOfferInTransaction(int userID, OfferTypeEnum offerTypeID, IDataContext context);
        void DeleteUserOfferByUserID(int userID);
    }
}
