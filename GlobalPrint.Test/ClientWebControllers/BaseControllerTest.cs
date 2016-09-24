using GlobalPrint.ClientWeb;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using static GlobalPrint.Infrastructure.DI.BaseIoC;

namespace GlobalPrint.Test.ClientWebControllers
{
    public class BaseControllerTest
    {
        protected string UserName = "sergei.lisiutin@gmail.com";
        
        protected void UpdateControllerContext(BaseController controller)
        {
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Moq.Mock<IPrincipal>();
            principal.Setup(p => p.IsInRole("Administrator")).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns(UserName);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;
        }

        protected T GetController<T>(BaseController controller) where T: BaseController
        {
            this.UpdateControllerContext(controller);
            return controller as T;
        }
    }
}
