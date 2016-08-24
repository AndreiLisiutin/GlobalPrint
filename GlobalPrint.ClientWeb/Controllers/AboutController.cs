using System.Web.Mvc;

namespace GlobalPrint.ClientWeb.Controllers
{
    public class AboutController : BaseController
    {
        /// <summary>
        /// Get page about project
        /// </summary>
        /// <returns></returns>
        // GET: About/About
        [HttpGet]
        [AllowAnonymous]
        public ActionResult About()
        {
            return View();
        }
        
    }
}