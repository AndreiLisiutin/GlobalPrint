using GlobalPrint.ClientWeb.Models.OfferController;
using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Offers;
using GlobalPrint.ServerBusinessLogic.Models.Business.Offers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Offers;
using System;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb.Controllers
{
    public class OfferController : BaseController
    {
        private IUserOfferUnit _userOfferUnit { get; set; }
        private IOfferUnit _offerUnit { get; set; }

        public OfferController(IUserOfferUnit userOfferUnit, IOfferUnit offerUnit)
        {
            _userOfferUnit = userOfferUnit;
            _offerUnit = offerUnit;
        }

        /// <summary>
        /// Show blank user offer on registration.
        /// </summary>
        /// <returns>Blank actual user offer.</returns>
        // GET: /Offer/ActualUserOffer
        [HttpGet]
        public ActionResult ActualUserOffer()
        {
            Offer userOffer = _offerUnit.GetActualOfferByType(OfferTypeEnum.UserOffer);
            Argument.NotNull(userOffer, "Не найдена актуальная оферта пользователя.");
            Argument.NotNullOrWhiteSpace(userOffer.Text, "Текст оферты пользователя пуст.");

            OfferViewModel offerModel = new OfferViewModel()
            {
                Title = userOffer.Name ?? "Договор оферты пользователя",
                Paragraphs = userOffer.Text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None)
            };

            return View("Offer", offerModel);
        }

        /// <summary>
        /// Get user offer or show blank offer.
        /// </summary>
        /// <returns>Offer view.</returns>
        // GET: /Offer/Offer
        [HttpGet]
        [Authorize]
        public ActionResult Offer(OfferTypeEnum offerTypeID)
        {
            UserOfferExtended userOffer = _userOfferUnit.GetLatestUserOfferByUserID(this.GetCurrentUserID(), offerTypeID);

            string[] offerParagraphs = null;
            string offerTitle = null;
            if (userOffer != null && userOffer.Offer != null && !string.IsNullOrWhiteSpace(userOffer.UserOfferText))
            {
                offerTitle = userOffer.UserOfferTitle;
                offerParagraphs = userOffer.UserOfferText.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            }
            
            OfferViewModel offerModel = new OfferViewModel()
            {
                Title = offerTitle,
                Paragraphs = offerParagraphs
            };
            return View(offerModel);
        }
    }
}