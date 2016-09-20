using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Offers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Offers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Offers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb.Controllers
{
    public class OfferController : BaseController
    {
        /// <summary>
        /// Get user offer or show blank offer.
        /// </summary>
        /// <returns>Offer view.</returns>
        // GET: /Offer/Offer
        [HttpGet]
        [Authorize]
        public ActionResult Offer(OfferTypeEnum offerTypeID)
        {
            UserOfferUnit userOfferUnit = new UserOfferUnit();

            UserOfferExtended userOffer = userOfferUnit.GetLatestUserOfferByUserID(this.GetCurrentUserID(), offerTypeID);
            List<string> offerParagraphs = new List<string>();
            if (userOffer != null && userOffer.Offer != null && !string.IsNullOrWhiteSpace(userOffer.Offer.Text))
            {
                offerParagraphs = userOffer.Offer.Text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();
            }

            ViewBag.OfferTitle = userOffer.UserOfferString;
            ViewBag.OfferParagraphs = offerParagraphs;
            return View(userOffer);
        }
    }
}