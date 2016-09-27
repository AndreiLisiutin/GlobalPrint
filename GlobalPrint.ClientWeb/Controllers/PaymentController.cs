using GlobalPrint.ClientWeb.Attributes;
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
        /// <summary>
        /// Action for Robokassa. So called "Result Url" in terms of Robokassa documentation.
        /// This url is called by Robokassa robot. We care confirming his payment if it wasn't already confirmed and redirecting user to his account.
        /// </summary>
        /// <param name="confirmationRequest">Data about payment operation from Robokassa.</param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeByUrl(new string[] { "auth.robokassa.ru" })]

        public ActionResult Result(RobokassaConfirmationRequest confirmationRequest)
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

        /// <summary>
        /// Action for Robokassa. So called "Success Url" in terms of Robokassa documentation.
        /// Customer is redirected to this url after successful payment. We care confirming his payment if it wasn't already confirmed and redirecting user to his account.
        /// </summary>
        /// <param name="confirmationRequest">Data about payment operation from Robokassa.</param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeByUrl(new string[] { "auth.robokassa.ru" })]
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

                return RedirectToAction("UserProfile", "UserProfile");
            }
            catch (Exception ex)
            {
#warning ошибку сюда
                throw;
            }
        }

        /// <summary>
        /// Action for Robokassa. Is called when payment was failed. We are just rolling back payment transaction.
        /// </summary>
        /// <param name="confirmationRequest">Data about payment operation from Robokassa.</param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeByUrl(new string[] { "auth.robokassa.ru" })]
        public ActionResult Fail(RobokassaConfirmationRequest confirmationRequest)
        {
            Argument.NotNull(confirmationRequest, "PaymentController.Fail model is null.");
            Argument.Positive(confirmationRequest.InvId, "PaymentController.Fail.InvId is not positive.");
            Argument.NotNullOrWhiteSpace(confirmationRequest.SignatureValue, "PaymentController.Fail.SignatureValue is empty.");
            try
            {
                //no need to confirm query - robokassa doesn't send us such a data
                //if (!confirmationRequest.IsQueryValid(RobokassaQueryType.ResultURL))
                //{
                //    throw new InvalidOperationException("Некорректный ответ робокассы. Полученные от робокассы парамеры не прошли проверку подлинности.");
                //}
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