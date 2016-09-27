using GlobalPrint.Configuration.DI;
using GlobalPrint.Infrastructure.LogUtility.Robokassa;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Users;
using GlobalPrint.ServerBusinessLogic.Models.Business.Users;
using System;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb
{
    public class UserProfileController : BaseController
    {
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
                    throw new Exception("Некорректно введена ссума пополнения", ex);
                }
                string redirectUrl = Robokassa.GetRedirectUrl(decimalUpSumm, userID);
                return Redirect(redirectUrl);
                
                //return RedirectToAction("UserProfile", new { UserID = userID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("UserProfile");
            }
        }
    }
}
