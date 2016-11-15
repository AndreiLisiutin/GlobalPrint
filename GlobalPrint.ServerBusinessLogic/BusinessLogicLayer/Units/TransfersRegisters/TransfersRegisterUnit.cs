using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.Infrastructure.EmailUtility;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Printers;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.Models.Business.TransfersRegisters;
using GlobalPrint.ServerBusinessLogic.Models.Domain.TransfersRegisters;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.TransfersRegisters
{
    public class TransfersRegisterUnit : BaseUnit
    {
        private Lazy<IEmailUtility> _emailUtility { get; set; }

        public TransfersRegisterUnit(Lazy<IEmailUtility> emailUtility)
        {
            this._emailUtility = emailUtility;
        }

        /// <summary>
        /// Valiadate cash out action request.
        /// </summary>
        /// <param name="request">Request model with data.</param>
        /// <returns>Validation object.</returns>
        public Validation ValidateCashRequest(CashRequest request)
        {
            bool isEdit = request.ID > 0;
            using (var context = this.Context())
            {
                var cashRepo = this.Repository<ICashRequestRepository>(context);
                var userRepo = this.Repository<IUserRepository>(context);

                Validation validation = new Validation();
                validation.NotNull(request, "Заявка на вывод денег не может быть пустой.");
                validation.Positive(request.CashRequestStatusID, "Статус заявки на вывод денег не может быть пустым.");
                validation.Positive(request.AmountOfMoney, "Денежная сумма в заявке на вывод денег должна быть положительной.");
                validation.Positive(request.UserID, "Пользователь, запрашивающий вывод денег, должен быть указан.");

                User user = userRepo.GetByID(request.UserID);
                validation.NotNull(user, "Пользователь, указанный в заявке на вывод денег, не найден.");
                validation.Require(user.AmountOfMoney >= request.AmountOfMoney, "У пользователя, указанного в заявке на вывод денег, не хватает средств на счету.");
                validation.Require(request.CreatedOn != DateTime.MinValue, "Дата вывода денег некорректна.");

                if (!isEdit)
                {
                    validation.Require(!request.TransfersRegisterID.HasValue, "При создании запрос на вывод денег не может быть привязан к реестру.");
                    validation.Require(request.CashRequestStatusID == (int)CashRequestStatusEnum.InProgress, "При создании запрос на вывод денег не может быть закрыт.");
                }
                return validation;
            }

        }

        /// <summary>
        /// Request cash from account to out.
        /// </summary>
        /// <param name="request">Request model with data.</param>
        /// <returns>Saved request.</returns>
        public CashRequest RequestCash(CashRequest request)
        {
            this.ValidateCashRequest(request).ThrowExceptionIfNotValid();

            using (var context = this.Context())
            {
                var userRepo = this.Repository<IUserRepository>(context);
                var repository = this.Repository<ICashRequestRepository>(context);
                repository.Insert(request);
                context.Save();

                User user = userRepo.GetByID(request.UserID);
                EmailMessage message = this._CreateCashRequestMessage(request, user);
                this._emailUtility.Value.Send(message);
                return request;
            }
        }

        /// <summary>
        /// Create new transfers register. Selects all the cash requests which are not processed yet and signs them with register. 
        /// </summary>
        /// <param name="userID">Identifier of the admin user.</param>
        /// <returns>Saved register model.</returns>
        public TransfersRegister CreateTransfersRegister(int userID)
        {
            using (var context = this.Context())
            {
                var cashRepo = this.Repository<ICashRequestRepository>(context);
                var userRepo = this.Repository<IUserRepository>(context);
                var registerRepo = this.Repository<ITransfersRegisterRepository>(context);
                List<EmailMessage> messagesToSend = new List<EmailMessage>();

                List<CashRequest> requests = cashRepo
                    .Get(e => e.TransfersRegisterID == null && e.CashRequestStatusID == (int)CashRequestStatusEnum.InProgress)
                    .ToList();
                List<int> requestIDs = requests.Select(e => e.UserID).Distinct().ToList();
                List<User> users = userRepo.Get(e => requestIDs.Contains(e.ID)).ToList();

                TransfersRegister register = new TransfersRegister()
                {
                    CreatedOn = DateTime.Now,
                    UserID = userID
                };

                context.BeginTransaction();
                try
                {

                    registerRepo.Insert(register);
                    context.Save();

                    foreach (CashRequest cashRequest in requests)
                    {
                        User user = users.First(e => e.ID == cashRequest.UserID);

                        if (cashRequest.AmountOfMoney > user.AmountOfMoney)
                        {
                            //money is not enough
                            cashRequest.CashRequestStatusID = (int)CashRequestStatusEnum.RolledBack;
                            cashRequest.CashRequestStatusComment =
                                $"Запрос на вывод денег № {cashRequest.ID} от {cashRequest.CreatedOn.ToString("dd.MM.yyyy")} отклонен. " +
                                $"Заявленная сумма {cashRequest.AmountOfMoney.ToString("#.00")} рублей превышает доступный остаток на счете {user.AmountOfMoney.ToString("#.00")} рублей.";
                            cashRepo.Update(cashRequest);
                        }
                        else if (string.IsNullOrWhiteSpace(user.BankName) || string.IsNullOrWhiteSpace(user.BankBic)
                            || string.IsNullOrWhiteSpace(user.BankCorrespondentAccount) || string.IsNullOrWhiteSpace(user.PaymentAccount))
                        {
                            List<string> missingData = new List<string>();
                            if (string.IsNullOrWhiteSpace(user.BankName)) missingData.Add("наименование банка");
                            if (string.IsNullOrWhiteSpace(user.BankBic)) missingData.Add("БИК банка");
                            if (string.IsNullOrWhiteSpace(user.BankCorrespondentAccount)) missingData.Add("корреспондентский счет банка");
                            if (string.IsNullOrWhiteSpace(user.PaymentAccount)) missingData.Add("Расчетный счет");

                            //bank account data is not enough
                            cashRequest.CashRequestStatusID = (int)CashRequestStatusEnum.RolledBack;
                            cashRequest.CashRequestStatusComment =
                                $"Запрос на вывод денег № {cashRequest.ID} от {cashRequest.CreatedOn.ToString("dd.MM.yyyy")} отклонен. " +
                                $"Недостаточно информации о банковском счете ({string.Join(", ", missingData)}) для вывода денег. Заполните данные в личном кабинете.";
                            cashRepo.Update(cashRequest);
                        }
                        else
                        {
                            user.AmountOfMoney -= cashRequest.AmountOfMoney;
                            userRepo.Update(user);
                            cashRequest.CashRequestStatusID = (int)CashRequestStatusEnum.Committed;
                            cashRequest.TransfersRegisterID = register.ID;
                            cashRequest.CashRequestStatusComment =
                                $"Запрос на вывод денег № {cashRequest.ID} от {cashRequest.CreatedOn.ToString("dd.MM.yyyy")} успешно обработан и включен " +
                                $"в реестр перечислений № {register.ID} от {register.CreatedOn.ToString("dd.MM.yyyy")}. " +
                                $"Заявленная сумма {cashRequest.AmountOfMoney.ToString("#.00")} рублей будет перечислена на указанные в учетной записи реквизиты.";
                            cashRepo.Update(cashRequest);
                        }

                        messagesToSend.Add(this._CreateMTransfersRegisterMessage(cashRequest, user));
                    }
                    context.Save();
                    context.CommitTransaction();
                }
                catch (Exception ex)
                {
                    context.RollbackTransaction();
                    throw;
                }

                foreach (var message in messagesToSend)
                {
                    this._emailUtility.Value.Send(message);
                }
                return register;
            }
        }

        /// <summary>
        /// Creates new message for user about processing his request for cash.
        /// </summary>
        /// <param name="cashRequest">Cash request data.</param>
        /// <param name="user">User account data.</param>
        /// <returns>Email message model.</returns>
        private EmailMessage _CreateMTransfersRegisterMessage(CashRequest cashRequest, User user)
        {
            MailAddress userMail = new MailAddress(user.Email, user.UserName);
            string subject = "Global Print - Запрос на вывод денег обработан";
            string userMessageBody = cashRequest.CashRequestStatusComment;
            return new EmailMessage(user.Email, user.UserName, subject, userMessageBody);
        }

        /// <summary>
        /// Creates new message for user about processing his request for cash.
        /// </summary>
        /// <param name="cashRequest">Cash request data.</param>
        /// <param name="user">User account data.</param>
        /// <returns>Email message model.</returns>
        private EmailMessage _CreateCashRequestMessage(CashRequest cashRequest, User user)
        {
            MailAddress userMail = new MailAddress(user.Email, user.UserName);
            string subject = "Global Print - Запрос на вывод денег";
            string userMessageBody = $"Вы запросили вывод {cashRequest.AmountOfMoney.ToString("#.00")} рублей со своего счета. Данная сумма будет перечислена в конце недели. Вы получите оповещение.";
            return new EmailMessage(user.Email, user.UserName, subject, userMessageBody);
        }

        /// <summary>
        /// Select transfers registers by user id.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<TransfersRegisterExtended> GetTransfersRegisters(int userID)
        {
            using (var context = this.Context())
            {
                var cashRepo = this.Repository<ICashRequestRepository>(context);
                var userRepo = this.Repository<IUserRepository>(context);
                var registerRepo = this.Repository<ITransfersRegisterRepository>(context);

                var list = (
                    from register in registerRepo.Get(e => e.UserID == userID)
                    join cash in cashRepo.GetAll() on register.ID equals cash.TransfersRegisterID
                    group cash by register into registerGropp
                    orderby registerGropp.Key.CreatedOn descending
                    select new { Register = registerGropp.Key, Count = registerGropp.Count(), AmountOfMoneySumm = registerGropp.Sum(e => e.AmountOfMoney) }
                    )
                    .ToList()
                    .Select(e => new TransfersRegisterExtended() { TransfersRegister = e.Register, AmountOfMoneySumm = e.AmountOfMoneySumm, RequestsCount = e.Count })
                    .ToList();
                return list;
            }
        }

        /// <summary>
        /// Select cash requests by user id.
        /// </summary>
        /// <param name="userID">User id.</param>
        /// <returns></returns>
        public List<CashRequestExtended> GetCashRequests(int userID)
        {
            using (var context = this.Context())
            {
                var cashRepo = this.Repository<ICashRequestRepository>(context);
                var cashStatusRepo = this.Repository<ICashRequestStatusRepository>(context);
                var userRepo = this.Repository<IUserRepository>(context);
                var registerRepo = this.Repository<ITransfersRegisterRepository>(context);

                List<CashRequestExtended> requests = (
                    from cashRequest in cashRepo.GetAll()
                    join status in cashStatusRepo.GetAll() on cashRequest.CashRequestStatusID equals status.ID
                    where cashRequest.UserID == userID
                    orderby cashRequest.CreatedOn descending
                    select new CashRequestExtended() { CashRequest = cashRequest, CashRequestStatus = status }
                ).ToList();

                return requests;
            }
        }
    }
}
