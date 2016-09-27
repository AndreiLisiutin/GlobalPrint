using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Offers;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Offers;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Payment;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Offers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Payment;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Payment
{
    public class PaymentActionUnit : BaseUnit
    {
        [DebuggerStepThrough]
        public PaymentActionUnit()
            : base()
        {
        }

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
                        UserID = userID
                    };
                    actionRepo.Insert(action);

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
                    context.CommitTransaction();
                }
                catch (Exception ex)
                {
                    context.RollbackTransaction();
                    throw;
                }
            }
        }

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
                    context.CommitTransaction();
                }
                catch (Exception ex)
                {
                    context.RollbackTransaction();
                    throw;
                }
            }
        }



        private void _RollBackTransaction(int paymentTransactionID, IDataContext context)
        {
            Argument.Require(context.IsTransactionAlive(), $"Попытка откатить денежную транзакцию без физической транзакции БД (ID={paymentTransactionID}).");

            IUserRepository userRepo = this.Repository<IUserRepository>(context);
            IPaymentTransactionRepository transactionRepo = this.Repository<IPaymentTransactionRepository>(context);
            IPaymentActionRepository actionRepo = this.Repository<IPaymentActionRepository>(context);

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
        private void _CommitTransaction(int paymentTransactionID, IDataContext context)
        {
            Argument.Require(context.IsTransactionAlive(), $"Попытка подтвердить денежную транзакцию без физической транзакции БД (ID={paymentTransactionID}).");

            IUserRepository userRepo = this.Repository<IUserRepository>(context);
            IPaymentTransactionRepository transactionRepo = this.Repository<IPaymentTransactionRepository>(context);
            IPaymentActionRepository actionRepo = this.Repository<IPaymentActionRepository>(context);

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
