using GlobalPrint.ClientWeb.Helpers;
using GlobalPrint.Configuration.DI;
using GlobalPrint.Infrastructure.Localization;
using GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Printers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Users;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using System;
using System.Linq;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb.Controllers
{
    public class UserPrintInfoController : BaseController
    {
        /// <summary>
        /// User business logic unit.
        /// </summary>
        private readonly IUserUnit _userUnit;

        public UserPrintInfoController(IUserUnit userUnit)
        {
            _userUnit = userUnit;
        }

        /// <summary>
        /// Number of waiting orders, not processed by current user
        /// </summary>
        /// <returns>Partial view with recieved print orders number</returns>
        [Authorize, ChildActionOnly]
        public ActionResult UserRecievedPrintOrder()
        {
            int printOrdersCount = new PrinterUnit().GetWaitingIncomingOrdersCount(this.GetCurrentUserID());
            ViewData["UserRecievedPrintOrdersCount"] = printOrdersCount > 0 ? printOrdersCount.ToString() : null;
            return PartialView("_UserRecievedPrintOrder");
        }

        /// <summary>
        /// Get user balance
        /// </summary>
        /// <returns>Partial view with user balance</returns>
        [Authorize, ChildActionOnly]
        public ActionResult UserBalance()
        {
            UserUnit userUnit = IoC.Instance.Resolve<UserUnit>();
            User currentUser = userUnit.GetByID(this.GetCurrentUserID());
            if (currentUser != null)
            {
                ViewData["UserBalance"] = currentUser.AmountOfMoney.ToString("0.00");
                return PartialView("_UserBalance");
            }
            else
            {
                throw new Exception("Не найден текущий пользователь");
            }
        }

        /// <summary>
        /// Get country picker
        /// </summary>
        /// <returns>Partial view with country picker</returns>
        [AllowAnonymous, ChildActionOnly]
        public ActionResult CountryPicker()
        {
            ViewBag.CurrentCulture = LocalizationHelper.GetCurrentCulture();
            ViewBag.CultureList = LocalizationHelper.GetCultureList().ToList();
            return PartialView("_CountryPicker");
        }

        /// <summary>
        /// Get user name.
        /// </summary>
        /// <returns>Partial view with user name</returns>
        [ChildActionOnly]
        public ActionResult UserName()
        {
            var user = _userUnit.GetByID(this.GetCurrentUserID());
            ViewBag.UserName = user.UserName;
            return PartialView("_UserName");
        }

        /// <summary>
        /// Change culture|language of application.
        /// </summary>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        public ActionResult ChangeCulture(string language)
        {
            string returnUrl = Request.UrlReferrer.AbsolutePath;
            string currentCulture = LocalizationHelper.GetCurrentCultureString();
            
            string culture = LocalizationHelper.GetImplementedCulture(language);
            RouteData.Values["lang"] = culture;  // set culture

            return Redirect((returnUrl ?? "").Replace(currentCulture, culture));
        }
    }
}