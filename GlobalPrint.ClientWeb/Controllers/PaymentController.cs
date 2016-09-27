using GlobalPrint.Configuration.DI;
using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.Infrastructure.LogUtility.Robokassa;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Payment;
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
        // So called "Result Url" in terms of Robokassa documentation.
        // This url is called by Robokassa robot.
        [HttpPost]

        public ActionResult Confirm(RobokassaConfirmationRequest confirmationRequest)
        {
            Argument.NotNull(confirmationRequest, "PaymentController.Confirm model is null.");
            Argument.Positive(confirmationRequest.InvId, "PaymentController.Confirm.InvId is not positive.");
            Argument.NotNullOrWhiteSpace(confirmationRequest.SignatureValue, "PaymentController.Confirm.SignatureValue is empty.");
            try
            {
                if (!confirmationRequest.IsQueryValid(RobokassaQueryType.ResultURL))
                {
                    throw new InvalidOperationException("Некорректный ответ робокассы. Полученные от робокассы парамеры не прошли проверку подлинности.");
                }
                PaymentActionUnit paymentActionUnit = new PaymentActionUnit();
                int paymentTransactionID = confirmationRequest.InvId;
                paymentActionUnit.CommitFillUpBalance(paymentTransactionID);

                return RedirectToAction("UserProfile", "UserProfile");
            }
            catch (Exception ex)
            {
#warning ошибку сюда
                throw;
            }
        }

        // So called "Success Url" in terms of Robokassa documentation.
        // Customer is redirected to this url after successful payment. 

        [HttpPost]
        public ActionResult Success(RobokassaConfirmationRequest confirmationRequest)
        {
            Argument.NotNull(confirmationRequest, "PaymentController.Success model is null.");
            Argument.Positive(confirmationRequest.InvId, "PaymentController.Success.InvId is not positive.");
            Argument.NotNullOrWhiteSpace(confirmationRequest.SignatureValue, "PaymentController.Success.SignatureValue is empty.");
            try
            {
                if (!confirmationRequest.IsQueryValid(RobokassaQueryType.SuccessURL))
                {
                    throw new InvalidOperationException("Некорректный ответ робокассы. Полученные от робокассы парамеры не прошли проверку подлинности.");
                }
                PaymentActionUnit paymentActionUnit = new PaymentActionUnit();
                int paymentTransactionID = confirmationRequest.InvId;
                paymentActionUnit.CommitFillUpBalance(paymentTransactionID);
                return Content("OK Success");
            }
            catch (Exception ex)
            {
#warning ошибку сюда
                return Content(ex.ToString());
            }
        }

        // So called "Fail Url" in terms of Robokassa documentation.
        // Customer is redirected to this url after unsuccessful payment.

        [HttpPost]
        public ActionResult Fail(RobokassaConfirmationRequest confirmationRequest)
        {
            Argument.NotNull(confirmationRequest, "PaymentController.Fail model is null.");
            Argument.Positive(confirmationRequest.InvId, "PaymentController.Fail.InvId is not positive.");
            Argument.NotNullOrWhiteSpace(confirmationRequest.SignatureValue, "PaymentController.Fail.SignatureValue is empty.");
            try
            {
                if (!confirmationRequest.IsQueryValid(RobokassaQueryType.ResultURL))
                {
                    throw new InvalidOperationException("Некорректный ответ робокассы. Полученные от робокассы парамеры не прошли проверку подлинности.");
                }
                PaymentActionUnit paymentActionUnit = new PaymentActionUnit();
                int paymentTransactionID = confirmationRequest.InvId;
                paymentActionUnit.RollbackFillUpBalance(paymentTransactionID);

                return RedirectToAction("UserProfile", "UserProfile");
            }
            catch (Exception ex)
            {
#warning ошибку сюда
                throw;
            }
        }
    }
}