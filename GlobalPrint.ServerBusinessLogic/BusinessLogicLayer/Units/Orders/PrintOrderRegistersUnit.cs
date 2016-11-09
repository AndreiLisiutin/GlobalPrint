using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.Infrastructure.FileUtility;
using GlobalPrint.Infrastructure.FileUtility.FileExporters;
using GlobalPrint.Infrastructure.FileUtility.FileExporters.Exportable;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Orders;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Payment;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Printers;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.DI;
using GlobalPrint.ServerBusinessLogic.Models.Business;
using GlobalPrint.ServerBusinessLogic.Models.Business.Orders;
using GlobalPrint.ServerBusinessLogic.Models.Business.Payments;
using GlobalPrint.ServerBusinessLogic.Models.Business.TransfersRegisters;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Orders;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Payment;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Orders
{
    public class PrintOrderRegistersUnit : BaseUnit
    {
        private IFileUtility _fileUtility;

        [DebuggerStepThrough]
        public PrintOrderRegistersUnit()
        {
            this._fileUtility = IoC.Instance.Resolve<IFileUtility>();
        }
        
        /// <summary>
        /// Perform calculating the register of orders for the certain period and for certain user as printer's owner.
        /// </summary>
        /// <param name="filter">Filter for the register.</param>
        /// <returns>Register file info.</returns>
        public DocumentBusinessInfo OrderRegisterExport(OrderRegisterFilter filter)
        {
            Argument.NotNull(filter, "Фильтр для реестра заказов пустой.");
            Argument.Positive(filter.OwnerUserID, "Не задан пользователь в фильтре для реестра заказов.");
            Argument.Positive((int)filter.FileExporter, "Не задан формат выгрузки в фильтре для реестра заказов.");

            IFileExporter exporter = this._fileUtility.GetFileExporter(filter.FileExporter);
            List<ExportableEntity> exportable = this._PrepareFinanceRegister(filter.OwnerUserID, filter.DateFrom, filter.DateTo);
            byte[] serializedReport = exporter.ExportToMemory(exportable);
            string extension = exporter.GetFileExtension();
            DocumentBusinessInfo documentInfo = new DocumentBusinessInfo()
            {
                SerializedFile = serializedReport,
                Extension = extension,
                Name = $"Реестр заказов за период с {filter.DateFrom?.ToString("dd.MM.yyyy") ?? "--"} по {filter.DateTo?.ToString("dd.MM.yyyy") ?? "--"}.{extension}",
                LoadedOn = DateTime.Now,
                UserID = filter.OwnerUserID
            };
            return documentInfo;
        }

        /// <summary> Get orders by certain criteria for export.
        /// </summary>
        /// <param name="ownerUserID"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        private List<ExportableEntity> _PrepareFinanceRegister(int ownerUserID, DateTime? dateFrom, DateTime? dateTo)
        {
            dateTo = dateTo.HasValue ? dateTo.Value.AddDays(1) : (DateTime?)null;
            using (IDataContext context = this.Context())
            {
                IUserRepository userRepo = this.Repository<IUserRepository>(context);
                IPrintOrderRepository orderRepo = this.Repository<IPrintOrderRepository>(context);
                IPrinterRepository printerRepo = this.Repository<IPrinterRepository>(context);

                var payments = (
                    from printer in printerRepo.GetAll()
                    join order in orderRepo.GetAll() on printer.ID equals order.PrinterID
                    join userClient in userRepo.GetAll() on order.UserID equals userClient.ID
                    join userOperator in userRepo.GetAll() on printer.OperatorUserID equals userOperator.ID
                    where printer.OwnerUserID == ownerUserID
                        && order.PrintOrderStatusID == (int)PrintOrderStatusEnum.Printed
                        && (!dateFrom.HasValue || order.PrintedOn >= dateFrom)
                        && (!dateTo.HasValue || order.PrintedOn < dateTo)
                    orderby order.PrintedOn descending
                    select new { order = order, userClient = userClient, userOperator = userOperator }
                 )
                 .ToList();

                List<ExportableEntity> entities = payments.Select(e =>
                {
                    var properties = new List<ExportableProperty>()
                    {
                        new ExportableProperty(e.order.OrderedOn.ToString("dd.MM.yyyy HH:mm:ss"), "Дата поступления заказа", 40),
                        new ExportableProperty(e.order.PrintedOn?.ToString("dd.MM.yyyy HH:mm:ss") ?? "--", "Дата выполнения заказа", 40),
                        new ExportableProperty(e.order.FullPrice.ToString("#.00", CultureInfo.InvariantCulture), "Стоимость заказа", 20),
                        new ExportableProperty(e.userClient.Email ?? "--", "Email клиента", 60),
                        new ExportableProperty(e.userOperator.BankBic ?? "--", "БИК банка оператора принтера", 30),
                        new ExportableProperty(e.userOperator.Email ?? "--", "Email оператора принтера", 60)
                    };
                    ExportableEntity entity = new ExportableEntity(properties);
                    return entity;
                })
                .ToList();


                return entities;
            }
        }

        /// <summary>
        /// Perform calculating the register of transfers and export it.
        /// </summary>
        /// <param name="filter">Filter for the register.</param>
        /// <returns>Register file info.</returns>
        public DocumentBusinessInfo TransfersRegisterExport(TransfersRegisterFilter filter)
        {
            Argument.NotNull(filter, "Фильтр для реестра перечислений пустой.");
            Argument.Positive(filter.OwnerUserID, "Не задан пользователь в фильтре для реестра перечислений.");
            Argument.Positive(filter.TransfersRegisterID, "Не задан ключ реестра в фильтре для реестра перечислений.");
            Argument.Positive((int)filter.FileExporter, "Не задан формат выгрузки в фильтре для реестра перечислений.");

            IFileExporter exporter = this._fileUtility.GetFileExporter(filter.FileExporter);
            List<ExportableEntity> exportable = this._PrepareTransfersRegister(filter.TransfersRegisterID, filter.OwnerUserID);
            byte[] serializedReport = exporter.ExportToMemory(exportable);
            string extension = exporter.GetFileExtension();
            DocumentBusinessInfo documentInfo = new DocumentBusinessInfo()
            {
                SerializedFile = serializedReport,
                Extension = extension,
                Name = $"Реестр перечислений № {filter.TransfersRegisterID}.{extension}",
                LoadedOn = DateTime.Now,
                UserID = filter.OwnerUserID
            };
            return documentInfo;
        }

        /// <summary>
        /// Perform calculating the register of transfers.
        /// </summary>
        /// <param name="transfersRegisterID">Identifier of the transfers register.</param>
        /// <param name="userID">User-owner.</param>
        /// <returns>Data of the register.</returns>
        private List<ExportableEntity> _PrepareTransfersRegister(int transfersRegisterID, int userID)
        {
            Argument.Positive(transfersRegisterID, "Идентификатор реестра перечислений для формирования отчета не задан.");
            Argument.Positive(userID, "Пользователь, формирующий отчет по реестру перечислений, не задан.");

            using (IDataContext context = this.Context())
            {
                IUserRepository userRepo = this.Repository<IUserRepository>(context);
                ITransfersRegisterRepository registerRepo = this.Repository<ITransfersRegisterRepository>(context);
                ICashRequestRepository cashRepo = this.Repository<ICashRequestRepository>(context);



                var cashRequests = (
                    from register in registerRepo.GetAll()
                    join cashRequest in cashRepo.GetAll() on register.ID equals cashRequest.TransfersRegisterID
                    join user in userRepo.GetAll() on cashRequest.UserID equals user.ID
                    where register.ID == transfersRegisterID && register.UserID == userID
                    orderby cashRequest.CreatedOn descending
                    select new { register = register, cashRequest = cashRequest, user = user }
                 )
                 .ToList();

                List<ExportableEntity> entities = cashRequests.Select(e =>
                {
                    var properties = new List<ExportableProperty>()
                    {
                        new ExportableProperty(e.register.ID.ToString(), "Номер реестра перечислений", 10),
                        new ExportableProperty(e.user.UserName ?? "--", "Логин пользователя", 40),
                        new ExportableProperty(e.user.Email ?? "--", "Email пользователя", 40),
                        new ExportableProperty(e.user.BankBic ?? "--", "БИК банка пользователя", 40),
                        new ExportableProperty(e.user.BankCorrespondentAccount ?? "--", "Корреспондентский счет банка", 40),
                        new ExportableProperty(e.user.BankName ?? "--", "Наименование банка", 40),
                        new ExportableProperty(e.user.Inn ?? "--", "ИНН пользователя", 40),
                        new ExportableProperty(e.user.Kpp ?? "--", "КПП пользователя", 40),
                        new ExportableProperty(e.user.LegalAddress ?? "--", "Юридический адрес организации пользователя", 40),
                        new ExportableProperty(e.user.LegalFullName ?? "--", "Полное наименование организации пользователя", 40),
                        new ExportableProperty(e.user.LegalShortName ?? "--", "Краткое наименование организации пользователя", 40),
                        new ExportableProperty(e.user.Ogrn ?? "--", "ОГРН организации пользователя", 40),
                        new ExportableProperty(e.user.PaymentAccount ?? "--", "Расчетный счет пользователя", 40),
                        new ExportableProperty(e.user.PostAddress ?? "--", "Почтовый адрес пользователя", 40),
                        new ExportableProperty(e.user.PhoneNumber ?? "--", "Номер телефона пользователя", 40),
                        new ExportableProperty(e.cashRequest.AmountOfMoney.ToString("#.00", CultureInfo.InvariantCulture), "Сумма к перечислению заказа", 20),
                        new ExportableProperty(e.cashRequest.ID.ToString(), "Идентификатор запроса на вывод денег", 20)
                    };
                    ExportableEntity entity = new ExportableEntity(properties);
                    return entity;
                })
                .ToList();


                return entities;
            }
        }
    }
}
