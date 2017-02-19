using System.Web.Mvc;

namespace GlobalPrint.ClientWeb.Controllers
{
    /// <summary>
    /// Контроллер страницы "О проекте".
    /// </summary>
    public class AboutController : BaseController
    {
        /// <summary>
        /// Получить страницу "О проекте".
        /// </summary>
        /// <returns>Страница "О проекте".</returns>
        [HttpGet, AllowAnonymous]
        public ActionResult About()
        {
            return View();
        }        
    }
}