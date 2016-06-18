using GlobalPrint.Server;
using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Text.RegularExpressions;
using GlobalPrint.Server.Utilities;

namespace GlobalPrint.ClientWeb
{
    public class UserAccountController : BaseController
    {
        // GET: UserAccount/UserAccount
        [HttpGet]
        public ActionResult UserAccount(int UserID)
        {
            var user = new UserBll().GetUserByID(UserID);
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
                new UserBll().SaveUser(model);
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
        public ActionResult FillUpBalance(User model, string upSumm)
        {
            if (!ModelState.IsValid)
            {
                return View("UserAccount", model);
            }

            try
            {
                decimal decimalUpSumm;
                try
                {
                    decimalUpSumm = StringExtension.ConvertCurrentcyToDecimal(upSumm);
                }
                catch(Exception ex)
                {
                    throw new Exception("Некорректно введена ссума пополнения", ex);
                }

                new UserBll().FillUpBalance(model, decimalUpSumm);
                return RedirectToAction("UserAccount", new { UserID = model.UserID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("UserAccount", model);
            }
        }
    }
}
