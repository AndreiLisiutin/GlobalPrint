using GlobalPrint.ClientWeb.Filters;
using GlobalPrint.Configuration.DI;
using GlobalPrint.Infrastructure.LogUtility.Robokassa;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Payment;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Payment;
using System.Web.Mvc;
using System;
using GlobalPrint.ServerBusinessLogic.Models.Business.Users;
using GlobalPrint.ServerBusinessLogic.Models.Business.Payments;
using System.Collections.Generic;
using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using GlobalPrint.ClientWeb.Helpers;
using GlobalPrint.Infrastructure.CommonUtils.Pagination;

namespace GlobalPrint.ClientWeb
{
    public class UserProfileController : BaseController
    {
        private UserUnit _userUnit;
        public UserProfileController()
        {
            this._userUnit = IoC.Instance.Resolve<UserUnit>();
        }

        /// <summary>
        /// Get user profile view.
        /// </summary>
        /// <returns>Profile info of current user.</returns>
        // GET: UserProfile/UserProfile
        [HttpGet]
        [Authorize]
        public ActionResult UserProfile()
        {
            UserUnit userUnit = IoC.Instance.Resolve<UserUnit>();

            var user = userUnit.GetExtendedUserByID(this.GetCurrentUserID());
            return View(user);
        }

        /// <summary>
        /// Save user profile info.
        /// </summary>
        /// <param name="model">Profile info of current user.</param>
        /// <returns>Redirects to updated profile view.</returns>
        [HttpPost]
        [Authorize]
        [MultipleButton(Name = "action", Argument = "Save")]
        public ActionResult Save(UserExtended model)
        {
            if (!ModelState.IsValid)
            {
                return View("UserProfile", model);
            }
            UserUnit userUnit = IoC.Instance.Resolve<UserUnit>();

            try
            {
                userUnit.UpdateUserProfile(model?.User);
                return RedirectToAction("UserProfile");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("UserProfile", model);
            }
        }

        [HttpPost]
        [Authorize]
        [MultipleButton(Name = "action", Argument = "FillUpBalance")]
        public ActionResult FillUpBalance(string upSumm)
        {
            try
            {
                UserUnit userUnit = IoC.Instance.Resolve<UserUnit>();
                int userID = this.GetCurrentUserID();
                decimal decimalUpSumm;
                try
                {
                    decimalUpSumm = Convert.ToDecimal(upSumm);
                }
                catch (Exception ex)
                {
                    throw new Exception("Некорректно введена сумма пополнения", ex);
                }
                //create payment action in DB for filling up balance and redirect to robokassa
                PaymentAction action = new PaymentActionUnit().InitializeFillUpBalance(userID, decimalUpSumm, null);
                string redirectUrl = Robokassa.GetRedirectUrl(decimalUpSumm, action.PaymentTransactionID);
                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("UserProfile");
            }
        }

        /// <summary>
        /// Get list of user's payments.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult MyPayments()
        {
            PaymentActionUnit paymentUnit = new PaymentActionUnit();
            int userID = this.GetCurrentUserID();
            List<PaymentActionFullInfo> actions = paymentUnit.GetByUserID(userID);
            return View(actions);
        }

        /// <summary>
        /// Get list of users with ability to search. 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult UsersListPartial(string emailPattern, Paging paging)
        {
            paging = paging ?? new Paging();

            List<User> users = this._userUnit.GetByFilter(emailPattern, paging);
            int count = this._userUnit.CountByFilter(emailPattern);
            PagedList<User> pagedList = new PagedList<User>(users, count, paging.ItemsPerPage, paging.CurrentPage);
            ViewBag.CurrentFilter = emailPattern;
            return PartialView("UsersListPartial", pagedList);
        }
    }
}
