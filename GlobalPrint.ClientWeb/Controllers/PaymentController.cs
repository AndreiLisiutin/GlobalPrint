using GlobalPrint.Configuration.DI;
using GlobalPrint.Infrastructure.LogUtility.Robokassa;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb.Controllers
{
    public class PaymentController : BaseController
    {
        public ActionResult Index()
        {
            int priceRub = 1000;
            int orderId = 1;

            // note: use GetRedirectUrl overloading to specify customer email

            string redirectUrl = Robokassa.GetRedirectUrl(priceRub, orderId);

            return Redirect(redirectUrl);
        }

        // So called "Result Url" in terms of Robokassa documentation.
        // This url is called by Robokassa robot.
        [HttpPost]

        public ActionResult Confirm(RobokassaConfirmationRequest confirmationRequest)
        {
            try
            {
                if (confirmationRequest.IsQueryValid(RobokassaQueryType.ResultURL))
                {
                    processOrder(confirmationRequest);

                    return Content("OK Confirm"); // content for robot
                }
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
            return Content("ERR");
        }

        // So called "Success Url" in terms of Robokassa documentation.
        // Customer is redirected to this url after successful payment. 

        [HttpPost]
        public ActionResult Success(RobokassaConfirmationRequest confirmationRequest)
        {
            try
            {

                if (confirmationRequest.IsQueryValid(RobokassaQueryType.SuccessURL))
                {
                    processOrder(confirmationRequest);
                    return Content("OK Success"); // content for robot
                    //return View(); // content for user
                }
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }

            return View("Fail");
        }

        // So called "Fail Url" in terms of Robokassa documentation.
        // Customer is redirected to this url after unsuccessful payment.

        [HttpPost]
        public ActionResult Fail()
        {
            return Content("Fail");
        }

        private void processOrder(RobokassaConfirmationRequest confirmationRequest)
        {
            UserUnit userUnit = IoC.Instance.Resolve<UserUnit>();
            userUnit.FillUpBalance(confirmationRequest.InvId, Decimal.Parse(confirmationRequest.OutSum));
            // TODO:
            // 1. verify your order Id and price here
            // 2. mark your order as paid
        }
    }
}