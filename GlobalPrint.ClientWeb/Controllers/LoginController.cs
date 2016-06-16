using GlobalPrint.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb
{
    public class LoginController : BaseController
    {
        // GET: Login/Login
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View("Register");
        }

        public ActionResult CheckLogin(string login, string password)
        {
            var response = new LoginBll().CheckLogin(login, password);
            if (response != null && response.Count > 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return Json(response);
        }

        public ActionResult RegisterUser(string name, string login, string password)
        {
            var response = new LoginBll().Register(name, login, password);
            if (response != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return Json(response);
        }

    }
}
