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
    }
}
