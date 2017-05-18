using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.ServerBusinessLogic.Models.Business.TransfersRegisters;
using GlobalPrint.ServerBusinessLogic.Models.Domain.TransfersRegisters;
using System.Collections.Generic;

namespace GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.TransfersRegisters
{
    public interface ITransfersRegisterUnit
    {
        /// <summary>
        /// Valiadate cash out action request.
        /// </summary>
        /// <param name="request">Request model with data.</param>
        /// <returns>Validation object.</returns>
        Validation ValidateCashRequest(CashRequest request);

        /// <summary>
        /// Request cash from account to out.
        /// </summary>
        /// <param name="request">Request model with data.</param>
        /// <returns>Saved request.</returns>
        CashRequest RequestCash(CashRequest request);

        /// <summary>
        /// Create new transfers register. Selects all the cash requests which are not processed yet and signs them with register. 
        /// </summary>
        /// <param name="userID">Identifier of the admin user.</param>
        /// <returns>Saved register model.</returns>
        TransfersRegister CreateTransfersRegister(int userID);

        /// <summary>
        /// Predict how many cash requests will be in next transfers register.
        /// </summary>
        /// <returns></returns>
        TransfersRegister GetNextTransferRegisterPrediction();

        /// <summary>
        /// Select transfers registers by user id.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        List<TransfersRegister> GetTransfersRegisters(int userID);

        /// <summary>
        /// Select cash requests by user id.
        /// </summary>
        /// <param name="userID">User id.</param>
        /// <returns></returns>
        List<CashRequestExtended> GetCashRequests(int userID);
    }
}
