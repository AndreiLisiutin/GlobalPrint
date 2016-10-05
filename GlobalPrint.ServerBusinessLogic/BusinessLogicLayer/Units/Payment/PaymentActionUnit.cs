using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Offers;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Offers;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Orders;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Payment;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Printers;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.Models.Business.Payments;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Orders;
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

        /// <summary>
        /// Return list of user's successfull payments.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="dateFrom">Date from for the payments.</param>
        /// <param name="dateTo">Date to for the payments.</param>
        /// <returns></returns>
        public List<PaymentActionFullInfo> GetByUserID(int userID, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            using (IDataContext context = this.Context())
            {
                IPaymentTransactionRepository transactionRepo = this.Repository<IPaymentTransactionRepository>(context);
                IPaymentActionRepository actionRepo = this.Repository<IPaymentActionRepository>(context);
                IPaymentActionStatusRepository actionStatusRepo = this.Repository<IPaymentActionStatusRepository>(context);
                IPaymentActionTypeRepository actionTypeRepo = this.Repository<IPaymentActionTypeRepository>(context);

                List<PaymentActionFullInfo> paymentActions = (
                    from action in actionRepo.GetAll()
                    join transaction in transactionRepo.GetAll() on action.PaymentTransactionID equals transaction.ID
                    join status in actionStatusRepo.GetAll() on action.PaymentActionStatusID equals status.ID
                    join type in actionTypeRepo.GetAll() on action.PaymentActionTypeID equals type.ID
                    where action.UserID == userID 
                        && action.PaymentActionStatusID == (int)PaymentActionStatusEnum.ExecutedSuccessfully
                        && (!dateFrom.HasValue || action.FinishedOn >= dateFrom)
                        && (!dateTo.HasValue || action.FinishedOn <= dateTo)
                    orderby action.FinishedOn descending
                    select new { action = action, transaction = transaction, status = status, type = type }
                 )
                 .ToList()
                 .Select(e => new PaymentActionFullInfo(e.action, e.transaction, e.status, e.type))
                 .ToList();

                return paymentActions;
            }
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
        
        #region Print order payment

        /// <summary>
        /// Initialize action of printing the order.
        /// </summary>
        /// <param name="order">Fullfilled print order model to save.</param>
        /// <returns>Created print order.</returns>
        public PrintOrder InitializePrintOrder(PrintOrder order)
        {
            using (IDataContext context = this.Context())
            {
                IUserRepository userRepo = this.Repository<IUserRepository>(context);
                IPaymentTransactionRepository transactionRepo = this.Repository<IPaymentTransactionRepository>(context);
                IPaymentActionRepository actionRepo = this.Repository<IPaymentActionRepository>(context);
                IPrintOrderRepository orderRepo = this.Repository<IPrintOrderRepository>(context);

                User client = userRepo.GetByID(order.UserID);

                context.BeginTransaction();
                try
                {
                    //creating payment transaction
                    PaymentTransaction transaction = new PaymentTransaction()
                    {
                        ID = 0,
                        Comment = $"Оплата заказа печати.",
                        FinishedOn = null,
                        StartedOn = DateTime.Now,
                        PaymentTransactionStatusID = (int)PaymentTransactionStatusEnum.InProgress
                    };

                    transactionRepo.Insert(transaction);
                    context.Save();

                    //associates payment transaction with the print order
                    order.PaymentTransactionID = transaction.ID;
                    orderRepo.Insert(order);
                    context.Save();

                    transaction.Comment = $"Оплата заказа печати № {order.ID} от {DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}.";
                    transactionRepo.Update(transaction);

                    //creates payment action that "freezes" user's money for that order
                    PaymentAction action = new PaymentAction()
                    {
                        ID = 0,
                        Comment = $"Оплата заказа печати № {order.ID} от {DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}. Заморозка денежных средств в размере оплаты заказа на счете клиента.",
                        ExternalIdentifier = order.ID.ToString(),
                        FinishedOn = null,
                        //note that action is already in executed status
                        PaymentActionStatusID = (int)PaymentActionStatusEnum.ExecutedSuccessfully,
                        PaymentActionTypeID = (int)PaymentActionTypeEnum.PaymentForOrder,
                        PaymentTransactionID = transaction.ID,
                        StartedOn = DateTime.Now,
                        UserID = client.ID,
                        AmountOfMoney = -order.FullPrice
                    };
                    actionRepo.Insert(action);
                    
                    //performs action
                    client.AmountOfMoney -= order.FullPrice;
                    userRepo.Update(client);

                    context.Save();
                    context.CommitTransaction();


                    context.CommitTransaction();
                    return order;
                }
                catch (Exception ex)
                {
                    context.RollbackTransaction();
                    throw;
                }
            }
        }

        /// <summary> Confirm action of printing the order.
        /// </summary>
        /// <param name="paymentTransactionID">Identifier of payment transaction.</param>
        public void CommitPrintOrder(int printOrderID)
        {
            Argument.Positive(printOrderID, $"Print order (ID={printOrderID}) not found.");
            using (IDataContext context = this.Context())
            {
                IPaymentTransactionRepository transactionRepo = this.Repository<IPaymentTransactionRepository>(context);
                IPrintOrderRepository orderRepo = this.Repository<IPrintOrderRepository>(context);
                IUserRepository userRepo = this.Repository<IUserRepository>(context);
                IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);
                IPaymentActionRepository actionRepo = this.Repository<IPaymentActionRepository>(context);

                PrintOrder order = orderRepo.GetByID(printOrderID);
                Argument.NotNull(order, $"Print order (ID={printOrderID}) not found.");
                Argument.Positive(order.PaymentTransactionID, $"Print order (ID={printOrderID}) has no payment transaction 0o.");

                PaymentTransaction transaction = transactionRepo.GetByID(order.PaymentTransactionID);
                Argument.NotNull(transaction, $"Payment transaction (ID={order.PaymentTransactionID}) not found.");
                if (transaction.PaymentTransactionStatusID == (int)PaymentTransactionStatusEnum.Committed)
                {
                    //done
                    return;
                }

                User printerOwner = printerRepo.Get(e => e.ID == order.PrinterID)
                   .Join(userRepo.GetAll(), e => e.OwnerUserID, e => e.ID, (p, u) => u)
                   .First();

                context.BeginTransaction();
                try
                {
                    //creates payment action that "freezes" user's money for that order
                    //maybe it make sense to move it to Initialization phase
                    PaymentAction action = new PaymentAction()
                    {
                        ID = 0,
                        Comment = $"Оплата заказа печати № {order.ID} от {order.OrderedOn.ToString("dd.MM.yyyy HH:mm:ss")}. Перечисление оплаты заказа владельцу принтера.",
                        ExternalIdentifier = order.ID.ToString(),
                        FinishedOn = null,
                        PaymentActionStatusID = (int)PaymentActionStatusEnum.InProgress,
                        PaymentActionTypeID = (int)PaymentActionTypeEnum.PaymentForOrder,
                        PaymentTransactionID = transaction.ID,
                        StartedOn = DateTime.Now,
                        UserID = printerOwner.ID,
                        AmountOfMoney = order.FullPrice
                    };
                    actionRepo.Insert(action);
                    context.Save();
                    
                    this._CommitTransaction(order.PaymentTransactionID, context);

                    //mark order as printed
                    order.PrintedOn = DateTime.Now;
                    order.PrintOrderStatusID = (int)PrintOrderStatusEnum.Printed;
                    orderRepo.Update(order);

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

        /// <summary> Abort action of printing the order.
        /// </summary>
        /// <param name="paymentTransactionID">Identifier of payment transaction.</param>
        public void RollbackPrintOrder(int printOrderID)
        {
            Argument.Positive(printOrderID, $"Print order (ID={printOrderID}) not found.");
            using (IDataContext context = this.Context())
            {
                IPaymentTransactionRepository transactionRepo = this.Repository<IPaymentTransactionRepository>(context);
                IPrintOrderRepository orderRepo = this.Repository<IPrintOrderRepository>(context);
                IUserRepository userRepo = this.Repository<IUserRepository>(context);
                IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);
                IPaymentActionRepository actionRepo = this.Repository<IPaymentActionRepository>(context);

                PrintOrder order = orderRepo.GetByID(printOrderID);
                Argument.NotNull(order, $"Print order (ID={printOrderID}) not found.");
                Argument.Positive(order.PaymentTransactionID, $"Print order (ID={printOrderID}) has no payment transaction 0o.");

                PaymentTransaction transaction = transactionRepo.GetByID(order.PaymentTransactionID);
                Argument.NotNull(transaction, $"Payment transaction (ID={order.PaymentTransactionID}) not found.");
                if (transaction.PaymentTransactionStatusID == (int)PaymentTransactionStatusEnum.RolledBack)
                {
                    //done
                    return;
                }
                context.BeginTransaction();
                try
                {
                    this._RollBackTransaction(order.PaymentTransactionID, context);
                    
                    //mark order as not printed
                    order.PrintOrderStatusID = (int)PrintOrderStatusEnum.Rejected;
                    orderRepo.Update(order);

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

        #endregion Print order payment


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
            transaction.Comment = transaction.Comment == null ? "Транзакция отменена." : (transaction.Comment + " Транзакция отменена.");
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
                action.Comment = action.Comment == null ? "Транзакция отменена." : (action.Comment + " Транзакция отменена.");
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
                action.Comment = action.Comment == null ? "Транзакция подтверждена." : (action.Comment + " Транзакция подтверждена.");
                action.FinishedOn = DateTime.Now;
                actionRepo.Update(action);
            }

            transaction.PaymentTransactionStatusID = (int)PaymentTransactionStatusEnum.Committed;
            transaction.FinishedOn = DateTime.Now;
            transaction.Comment = transaction.Comment == null ? "Транзакция подтверждена." : (transaction.Comment + " Транзакция подтверждена.");
            transactionRepo.Update(transaction);
        }
    }
}
