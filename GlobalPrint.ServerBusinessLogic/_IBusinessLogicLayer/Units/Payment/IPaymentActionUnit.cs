using GlobalPrint.ServerBusinessLogic.Models.Business.Payments;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Orders;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Payment;
using System;
using System.Collections.Generic;

namespace GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Payment
{
    /// <summary>
    /// Интерфейс модуля для работы с оплатами, денежными операциями и транзакциями.
    /// </summary>
    public interface IPaymentActionUnit
    {
        /// <summary>
        /// Получить список успешных денежных операций пользователя.
        /// </summary>
        /// <param name="userID">Идентификатор пользователя.</param>
        /// <param name="dateFrom">Фильтр по дате операции - дата с.</param>
        /// <param name="dateTo">Фильтр по дате операции - дата по.</param>
        /// <returns>Список успешных денежных операций пользователя.</returns>
        List<PaymentActionFullInfo> GetByUserID(int userID, DateTime? dateFrom = null, DateTime? dateTo = null);

        #region Fill up user's account balance

        /// <summary> 
        /// Initialize action of filling up user's account balance.
        /// </summary>
        /// <param name="userID">Identifier of the user.</param>
        /// <param name="amountOfMoney">Amount of money that user will receive.</param>
        /// <param name="externalIdentifier">External payment system's identifier for the transaction.</param>
        /// <returns>Payment action entity.</returns>
        PaymentAction InitializeFillUpBalance(int userID, decimal amountOfMoney, string externalIdentifier);

        /// <summary> 
        /// Confirm the action of filling up user's account balance.
        /// </summary>
        /// <param name="paymentTransactionID">Identifier of payment transaction.</param>
        void CommitFillUpBalance(int paymentTransactionID);

        /// <summary> 
        /// Abort the action of filling up user's account balance.
        /// </summary>
        /// <param name="paymentTransactionID">Identifier of payment transaction.</param>
        void RollbackFillUpBalance(int paymentTransactionID);

        #endregion Fill up user's account balance

        #region Print order payment

        /// <summary>
        /// Initialize action of printing the order.
        /// </summary>
        /// <param name="order">Fullfilled print order model to save.</param>
        /// <returns>Created print order.</returns>
        PrintOrder InitializePrintOrder(PrintOrder order);

        /// <summary> 
        /// Confirm action of printing the order.
        /// </summary>
        /// <param name="paymentTransactionID">Identifier of payment transaction.</param>
        void CommitPrintOrder(int printOrderID);

        /// <summary> 
        /// Abort action of printing the order.
        /// </summary>
        /// <param name="paymentTransactionID">Identifier of payment transaction.</param>
        void RollbackPrintOrder(int printOrderID);

        #endregion Print order payment

        /// <summary>
        /// Send money from one user to another operation. Method creates and executes transaction and its actions.
        /// </summary>
        /// <param name="package">Data for operation.</param>
        /// <returns>Transaction.</returns>
        PaymentTransaction InitializeAndCommitSendMoney(SendModeyPackage package);
    }
}
