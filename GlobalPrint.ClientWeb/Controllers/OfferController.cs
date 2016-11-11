using GlobalPrint.Infrastructure.CommonUtils;
using System;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb.Controllers
{
    public class OfferController : BaseController
    {
        /// <summary>
        /// Show privacy policy on registration.
        /// </summary>
        /// <returns>Blank privacy policy.</returns>
        [HttpGet]
        public ActionResult PrivacyPolicy()
        {
            return View("PrivacyPolicy");
        }
        
        /// <summary>
        /// Get user offer or show blank offer.
        /// </summary>
        /// <returns>Offer view.</returns>
        [HttpGet]
        public ActionResult Offer()
        {
            return View("Offer");
        }
    }
}