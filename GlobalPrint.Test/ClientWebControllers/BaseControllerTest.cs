using GlobalPrint.ClientWeb;
using Moq;
using System.Security.Principal;
using System.Web.Mvc;

namespace GlobalPrint.Test.ClientWebControllers
{
    public class BaseControllerTest
    {
        /// <summary>
        /// User name for identity config for some fun.
        /// </summary>
        protected string UserName = "Bob Tester";

        /// <summary>
        /// After controller being created, we need to fake it user identity properties.
        /// </summary>
        /// <param name="controller">Created controller, inherited from <see cref="BaseController"/>.</param>
        protected void UpdateControllerContext(BaseController controller)
        {
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Mock<IPrincipal>();
            principal.Setup(p => p.IsInRole("Administrator")).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns(UserName);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;
        }

        /// <summary>
        /// Update identity properties of controller and convert it into T type.
        /// </summary>
        /// <typeparam name="T">Type to convert controller into.</typeparam>
        /// <param name="controller">Created controller, inherited from <see cref="BaseController"/>.</param>
        /// <returns></returns>
        protected T GetController<T>(BaseController controller) where T: BaseController
        {
            this.UpdateControllerContext(controller);
            return controller as T;
        }
    }
}
