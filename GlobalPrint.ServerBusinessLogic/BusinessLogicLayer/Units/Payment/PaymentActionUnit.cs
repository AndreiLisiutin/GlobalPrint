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

        public PaymentAction CommitFillUpBalance(int paymentActionID)
        {
            using (IDataContext context = this.Context())
            {
                IUserRepository userRepo = this.Repository<IUserRepository>(context);
                IPaymentTransactionRepository transactionRepo = this.Repository<IPaymentTransactionRepository>(context);
                IPaymentActionRepository actionRepo = this.Repository<IPaymentActionRepository>(context);

                context.BeginTransaction();
                try
                {
                    PaymentAction action = actionRepo.GetByID(paymentActionID);
                    Argument.NotNull(action, $"Payment action (ID={paymentActionID}) not found.");
                    PaymentTransaction transaction = transactionRepo.GetByID(action.PaymentTransactionID);
                    Argument.NotNull(transaction, $"Payment transaction (ID={action.PaymentTransactionID};PaymentActionID={paymentActionID}) not found.");
                    User user = userRepo.GetByID(action.UserID);
                    Argument.NotNull(user, $"User account for filling up balance operation not found (ID={action.UserID};PaymentActionID={paymentActionID}) not found.");

                    user.AmountOfMoney += action.AmountOfMoney;
                    userRepo.Update(user);
                    transaction.PaymentTransactionStatusID = (int)PaymentTransactionStatusEnum.Committed;
                    transaction.FinishedOn = DateTime.Now;
                    transactionRepo.Update(transaction);
                    action.PaymentActionStatusID = (int)PaymentActionStatusEnum.ExecutedSuccessfully;
                    action.FinishedOn = DateTime.Now;
                    actionRepo.Update(action);

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

        public PaymentAction RollbackFillUpBalance(int paymentActionID)
        {
            using (IDataContext context = this.Context())
            {
                IUserRepository userRepo = this.Repository<IUserRepository>(context);
                IPaymentTransactionRepository transactionRepo = this.Repository<IPaymentTransactionRepository>(context);
                IPaymentActionRepository actionRepo = this.Repository<IPaymentActionRepository>(context);

                context.BeginTransaction();
                try
                {
                    PaymentAction action = actionRepo.GetByID(paymentActionID);
                    Argument.NotNull(action, $"Payment action (ID={paymentActionID}) not found.");
                    PaymentTransaction transaction = transactionRepo.GetByID(action.PaymentTransactionID);
                    Argument.NotNull(transaction, $"Payment transaction (ID={action.PaymentTransactionID};PaymentActionID={paymentActionID}) not found.");
                    
                    transaction.PaymentTransactionStatusID = (int)PaymentTransactionStatusEnum.RolledBack;
                    transaction.FinishedOn = DateTime.Now;
                    transaction.Comment = "Отменено.";
                    transactionRepo.Update(transaction);
                    action.PaymentActionStatusID = (int)PaymentActionStatusEnum.Failed;
                    action.Comment = "Отменено.";
                    action.FinishedOn = DateTime.Now;
                    actionRepo.Update(action);

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
    }
}
