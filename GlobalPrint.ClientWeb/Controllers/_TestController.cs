using System;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb.Controllers
{
    public class _TestController : BaseController
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult _Test()
        {
            throw new Exception("Test exception");
            return View();
        }
        
    }
}