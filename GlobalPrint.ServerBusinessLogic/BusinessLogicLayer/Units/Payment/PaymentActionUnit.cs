using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Offers;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Offers;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Payment;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Payment;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Payment
{
    /// <summary>
    /// Unit about payment actions and transactions. 
    /// </summary>
    public class PaymentActionUnit : BaseUnit
    {
        [DebuggerStepThrough]
        public PaymentActionUnit()
            : base()
        {
        }

        #region Fill up user's account balance

        /// <summary> Initialize action of filling up user's account balance.
        /// </summary>
        /// <param name="userID">Identifier of the user.</param>
        /// <param name="amountOfMoney">Amount of money that user will receive.</param>
        /// <param name="externalIdentifier">External payment system's identifier for the transaction.</param>
        /// <returns>Payment action entity.</returns>
        public PaymentAction InitializeFillUpBalance(int userID, decimal amountOfMoney, string externalIdentifier)
        {
            using (IDataContext context = this.Context())
            {
                IUserRepository userRepo = this.Repository<IUserRepository>(context);
                IPaymentTransactionRepository transactionRepo = this.Repository<IPaymentTransactionRepository>(context);
                IPaymentActionRepository actionRepo = this.Repository<IPaymentActionRepository>(context);

                PaymentTransaction transaction = new PaymentTransaction()
                {
                    ID = 0,
                    Comment = $"Пополнение баланса на {amountOfMoney} у.е.",
                    FinishedOn = null,
                    StartedOn = DateTime.Now,
                    PaymentTransactionStatusID = (int)PaymentTransactionStatusEnum.InProgress
                };

                context.BeginTransaction();
                try
                {
                    transactionRepo.Insert(transaction);
                    context.Save();
                    PaymentAction action = new PaymentAction()
                    {
                        ID = 0,
                        Comment = $"Пополнение баланса на {amountOfMoney} у.е.",
                        ExternalIdentifier = externalIdentifier,
                        FinishedOn = null,
                        PaymentActionStatusID = (int)PaymentActionStatusEnum.InProgress,
                        PaymentActionTypeID = (int)PaymentActionTypeEnum.BalanceRefill,
                        PaymentTransactionID = transaction.ID,
                        StartedOn = DateTime.Now,
                        UserID = userID,
                        AmountOfMoney = amountOfMoney
                    };
                    actionRepo.Insert(action);
                    context.Save();
                    context.CommitTransaction();
                    return action;
                }
                catch (Exception ex)
                {
                    context.RollbackTransaction();
                    throw;
                }
            }
        }

        /// <summary> Confirm the action of filling up user's account balance.
        /// </summary>
        /// <param name="paymentTransactionID">Identifier of payment transaction.</param>
        public void CommitFillUpBalance(int paymentTransactionID)
        {
            using (IDataContext context = this.Context())
            {
                IPaymentTransactionRepository transactionRepo = this.Repository<IPaymentTransactionRepository>(context);
                PaymentTransaction transaction = transactionRepo.GetByID(paymentTransactionID);
                Argument.NotNull(transaction, $"Payment transaction (ID={paymentTransactionID}) not found.");
                if (transaction.PaymentTransactionStatusID == (int)PaymentTransactionStatusEnum.Committed)
                {
                    //done
                    return;
                }
                context.BeginTransaction();
                try
                {
                    this._CommitTransaction(paymentTransactionID, context);
                    context.Save();
                    context.CommitTransaction();
                }
                catch (Exception ex)
                {
                    context.RollbackTransaction();
                    throw;
                }
            }
        }

        /// <summary> Abort the action of filling up user's account balance.
        /// </summary>
        /// <param name="paymentTransactionID">Identifier of payment transaction.</param>
        public void RollbackFillUpBalance(int paymentTransactionID)
        {
            using (IDataContext context = this.Context())
            {
                IPaymentTransactionRepository transactionRepo = this.Repository<IPaymentTransactionRepository>(context);
                PaymentTransaction transaction = transactionRepo.GetByID(paymentTransactionID);
                Argument.NotNull(transaction, $"Payment transaction (ID={paymentTransactionID}) not found.");
                if (transaction.PaymentTransactionStatusID == (int)PaymentTransactionStatusEnum.RolledBack)
                {
                    //done
                    return;
                }
                context.BeginTransaction();
                try
                {
                    this._RollBackTransaction(paymentTransactionID, context);
                    context.Save();
                    context.CommitTransaction();
                }
                catch (Exception ex)
                {
                    context.RollbackTransaction();
                    throw;
                }
            }
        }
        
        #endregion Fill up user's account balance

        /// <summary>
        /// Roll back whole payment transaction. If there are already executed actions, roll back them logically. 
        /// It means that not only payment_action entities will change their statuses, 
        /// but also logic of money exchange will be AUTOMATICALLY rolled back (users will loose money).
        /// Requires transaction to be opened.
        /// </summary>
        /// <param name="paymentTransactionID">Identifier of payment transaction entity.</param>
        /// <param name="context">DB context. Requires transaction to be opened.</param>
        private void _RollBackTransaction(int paymentTransactionID, IDataContext context)
        {
            Argument.Require(context.IsTransactionAlive(), $"Попытка откатить денежную транзакцию без физической транзакции БД (ID={paymentTransactionID}).");

            IUserRepository userRepo = this.Repository<IUserRepository>(context);
            IPaymentTransactionRepository transactionRepo = this.Repository<IPaymentTransactionRepository>(context);
            IPaymentActionRepository actionRepo = this.Repository<IPaymentActionRepository>(context);

            //transaction must be in the status of "In progress"
            PaymentTransaction transaction = transactionRepo.GetByID(paymentTransactionID);
            Argument.NotNull(transaction, $"Payment transaction (ID={paymentTransactionID}) not found.");
            Argument.Require(transaction.PaymentTransactionStatusID == (int)PaymentTransactionStatusEnum.InProgress,
                $"Попытка откатить закрытую ранее денежную транзакцию (ID={paymentTransactionID}).");

            transaction.PaymentTransactionStatusID = (int)PaymentTransactionStatusEnum.RolledBack;
            transaction.FinishedOn = DateTime.Now;
            transaction.Comment = "Транзакция отменена.";
            transactionRepo.Update(transaction);

            List<PaymentAction> actions = actionRepo.Get(e => e.PaymentTransactionID == paymentTransactionID)
                .ToList();

            foreach (PaymentAction action in actions)
            {
                if (action.PaymentActionStatusID == (int)PaymentActionStatusEnum.ExecutedSuccessfully)
                {
                    //roll back action if it was already performed
                    User actionUser = userRepo.GetByID(action.UserID);
                    Argument.NotNull(actionUser, $"Payment action user (ID={action.UserID}) not found.");
                    actionUser.AmountOfMoney -= action.AmountOfMoney;
                    userRepo.Update(actionUser);
                }

                action.PaymentActionStatusID = (int)PaymentActionStatusEnum.Failed;
                action.Comment = "Транзакция отменена.";
                action.FinishedOn = DateTime.Now;
                actionRepo.Update(action);
            }
        }

        /// <summary>
        /// Commit whole payment transaction. If there are not already executed actions, execute them logically. 
        /// It means that not only payment_action entities will change their statuses, 
        /// but also logic of money exchange will be AUTOMATICALLY performed (users will receive money).
        /// Requires transaction to be opened.
        /// </summary>
        /// <param name="paymentTransactionID">Identifier of payment transaction entity.</param>
        /// <param name="context">DB context. Requires transaction to be opened.</param>
        private void _CommitTransaction(int paymentTransactionID, IDataContext context)
        {
            Argument.Require(context.IsTransactionAlive(), $"Попытка подтвердить денежную транзакцию без физической транзакции БД (ID={paymentTransactionID}).");

            IUserRepository userRepo = this.Repository<IUserRepository>(context);
            IPaymentTransactionRepository transactionRepo = this.Repository<IPaymentTransactionRepository>(context);
            IPaymentActionRepository actionRepo = this.Repository<IPaymentActionRepository>(context);

            //transaction must be in the status of "In progress"
            PaymentTransaction transaction = transactionRepo.GetByID(paymentTransactionID);
            Argument.NotNull(transaction, $"Payment transaction (ID={paymentTransactionID}) not found.");
            Argument.Require(transaction.PaymentTransactionStatusID == (int)PaymentTransactionStatusEnum.InProgress,
                $"Попытка подтвердить закрытую ранее денежную транзакцию (ID={paymentTransactionID}).");

            List<PaymentAction> actions = actionRepo.Get(e => e.PaymentTransactionID == paymentTransactionID)
                .ToList();
            Argument.Require(actions.All(e => e.PaymentActionStatusID != (int)PaymentActionStatusEnum.Failed),
                $"Попытка подтвердить денежную транзакцию, часть которой звершилась неудачно (ID={paymentTransactionID}).");

            foreach (PaymentAction action in actions)
            {
                if (action.PaymentActionStatusID == (int)PaymentActionStatusEnum.InProgress)
                {
                    //perform action if it was not already performed
                    User actionUser = userRepo.GetByID(action.UserID);
                    Argument.NotNull(actionUser, $"Payment action user (ID={action.UserID}) not found.");
                    actionUser.AmountOfMoney += action.AmountOfMoney;
                    userRepo.Update(actionUser);
                }

                action.PaymentActionStatusID = (int)PaymentActionStatusEnum.ExecutedSuccessfully;
                action.Comment = "Транзакция подтверждена.";
                action.FinishedOn = DateTime.Now;
                actionRepo.Update(action);
            }
            
            transaction.PaymentTransactionStatusID = (int)PaymentTransactionStatusEnum.Committed;
            transaction.FinishedOn = DateTime.Now;
            transaction.Comment = "Транзакция подтверждена.";
            transactionRepo.Update(transaction);
        }
    }
}
