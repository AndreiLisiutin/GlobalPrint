using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Printers;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.Models.Domain.TransfersRegisters;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.TransfersRegisters
{
    public class TransfersRegisterUnit : BaseUnit
    {
        public TransfersRegisterUnit()
        {
        }

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
                validation.Require(user.AmountOfMoney > request.AmountOfMoney, "У пользователя, указанного в заявке на вывод денег, не хватает средств на счету.");
                validation.Require(request.CreatedOn != DateTime.MinValue, "Дата вывода денег некорректна.");

                if (!isEdit)
                {
                    validation.Require(!request.TransfersRegisterID.HasValue, "При создании запрос на вывод денег не может быть привязан к реестру.");
                    validation.Require(request.CashRequestStatusID == (int)CashRequestStatusEnum.InProgress, "При создании запрос на вывод денег не может быть закрыт.");
                }
                return validation;
            }

        }

        public CashRequest RequestCash(CashRequest request)
        {
            this.ValidateCashRequest(request).ThrowExceptionIfNotValid();

            using (var context = this.Context())
            {
                var repository = this.Repository<ICashRequestRepository>(context);
                repository.Insert(request);
                context.Save();
                return request;
            }
        }

        public TransfersRegister CreateTransfersRegister(int userID)
        {
            using (var context = this.Context())
            {
                var cashRepo = this.Repository<ICashRequestRepository>(context);
                var userRepo = this.Repository<IUserRepository>(context);
                var registerRepo = this.Repository<ITransfersRegisterRepository>(context);

                List<CashRequest> requests = cashRepo
                    .Get(e => e.TransfersRegisterID == null && e.CashRequestStatusID == (int)CashRequestStatusEnum.InProgress)
                    .ToList();
                List<int> requestIDs = requests.Select(e => e.UserID).Distinct().ToList();
                List<User> users = userRepo.Get(e => requestIDs.Contains(e.ID)).ToList();

                context.BeginTransaction();

                try
                {
                    TransfersRegister register = new TransfersRegister()
                    {
                        CreatedOn = DateTime.Now,
                        UserID = userID
                    };

                    registerRepo.Insert(register);
                    context.Save();

                    foreach (CashRequest cashRequest in requests)
                    {
                        User user = users.First(e => e.ID == cashRequest.UserID);

                        if (cashRequest.AmountOfMoney <= user.AmountOfMoney)
                        {
                            user.AmountOfMoney -= cashRequest.AmountOfMoney;
                            userRepo.Update(user);
                            cashRequest.CashRequestStatusID = (int)CashRequestStatusEnum.Committed;
                            cashRequest.TransfersRegisterID = register.ID;
                            cashRepo.Update(cashRequest);
                        }
                        else
                        {
                            cashRequest.CashRequestStatusID = (int)CashRequestStatusEnum.RolledBack;
                            cashRepo.Update(cashRequest);
                        }
                    }
                    context.Save();
                    context.CommitTransaction();
                    return register;
                }
                catch (Exception ex)
                {
                    context.RollbackTransaction();
                    throw;
                }
            }
        }

        public List<TransfersRegister> GetTransfersRegisters(int userID)
        {
            using (var context = this.Context())
            {
                var cashRepo = this.Repository<ICashRequestRepository>(context);
                var userRepo = this.Repository<IUserRepository>(context);
                var registerRepo = this.Repository<ITransfersRegisterRepository>(context);

                return registerRepo.Get(e => e.UserID == userID).ToList();
            }
        }
    }
}
