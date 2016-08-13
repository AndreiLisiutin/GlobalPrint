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
    public class TeamController : BaseController
    {
        // GET: Team/Team
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Team()
        {
            return View();
        }
    }
}
