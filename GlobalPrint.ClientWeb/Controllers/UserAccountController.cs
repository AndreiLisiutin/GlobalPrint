using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Users;

namespace GlobalPrint.ClientWeb
{
    public class UserAccountController : BaseController
    {
        // GET: UserAccount/UserAccount
        [HttpGet]
        public ActionResult UserAccount()
        {
            int userID = Request.RequestContext.HttpContext.User.Identity.GetUserId<int>();
            var user = new UserUnit().GetUserByID(userID);
            return View(user);
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Save")]
        public ActionResult Save(User model)
        {
            if (!ModelState.IsValid)
            {
                return View("UserAccount", model);
            }

            try
            {
                new UserUnit().UpdateUserAccount(model);
                return RedirectToAction("UserAccount", new { UserID = model.UserID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("UserAccount", model);
            }
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "FillUpBalance")]
        public ActionResult FillUpBalance(string upSumm)
        {
            try
            {
                int userID = Request.RequestContext.HttpContext.User.Identity.GetUserId<int>();
                decimal decimalUpSumm;
                try
                {
                    decimalUpSumm = Convert.ToDecimal(upSumm);
                }
                catch(Exception ex)
                {
                    throw new Exception("Некорректно введена ссума пополнения", ex);
                }

                new UserUnit().FillUpBalance(userID, decimalUpSumm);
                return RedirectToAction("UserAccount", new { UserID = userID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("UserAccount");
            }
        }
    }
}
