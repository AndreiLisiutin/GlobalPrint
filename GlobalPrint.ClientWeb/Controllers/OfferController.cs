using GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Offers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Offers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Offers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Offers;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb.Controllers
{
    public class OfferController : BaseController
    {
        [Inject]
        private IUserOfferUnit _userOfferUnit { get; set; }

        public OfferController(IUserOfferUnit userOfferUnit)
        {
            _userOfferUnit = userOfferUnit;
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
            List<string> offerParagraphs = new List<string>();
            string offerTitle = null;
            if (userOffer != null && userOffer.Offer != null && !string.IsNullOrWhiteSpace(userOffer.Offer.Text))
            {
                offerTitle = userOffer.UserOfferString;
                offerParagraphs = userOffer.Offer.Text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();
            }

            ViewBag.OfferTitle = offerTitle;
            ViewBag.OfferParagraphs = offerParagraphs;
            return View(userOffer);
        }
    }
}