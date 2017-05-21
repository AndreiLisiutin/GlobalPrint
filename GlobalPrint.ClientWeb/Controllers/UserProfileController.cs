using GlobalPrint.ClientWeb.Filters;
using GlobalPrint.Infrastructure.BankUtility;
using GlobalPrint.Infrastructure.BankUtility.BicInfo;
using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.Infrastructure.LogUtility;
using GlobalPrint.Infrastructure.LogUtility.Robokassa;
using GlobalPrint.Infrastructure.Notifications;
using GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Payment;
using GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.TransfersRegisters;
using GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Users;
using GlobalPrint.ServerBusinessLogic.Models.Business.Payments;
using GlobalPrint.ServerBusinessLogic.Models.Business.TransfersRegisters;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Payment;
using GlobalPrint.ServerBusinessLogic.Models.Domain.TransfersRegisters;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb
{
    /// <summary>
    /// Контроллер профиля пользователя.
    /// </summary>
    public class UserProfileController : BaseController
    {
        /// <summary>
        /// Модуль бизнес логики для пользователя.
        /// </summary>
        private IUserUnit _userUnit;

        /// <summary>
        /// Модуль бизнес логики для денежных операций пользователя.
        /// </summary>
        private IPaymentActionUnit _paymentActionUnit;

        /// <summary>
        /// Модуль бизнес логики для реестров перечислений.
        /// </summary>
        private ITransfersRegisterUnit _transfersRegisterUnit;

        /// <summary>
        /// Утилита для получения информации о банке по БИК.
        /// </summary>
        private IBankUtility _bankUtility;

        /// <summary>
        /// Утилита логирования ошибок.
        /// </summary>
        private Lazy<ILogger> _logUtility;
        
        public UserProfileController(
            IUserUnit userUnit, 
            IPaymentActionUnit paymentActionUnit,
            ITransfersRegisterUnit transfersRegisterUnit,
            IBankUtility bankUtility,
            ILoggerFactory loggerFactory)
        {
            Argument.NotNull(userUnit, "Не задан модуль бизнес логики для пользователя.");
            Argument.NotNull(paymentActionUnit, "Не задан модуль бизнес логики для денежных операций пользователя.");
            Argument.NotNull(transfersRegisterUnit, "Не задан модуль бизнес логики для реестров перечислений.");
            Argument.NotNull(bankUtility, "Не задана утилита для получения информации о банке по БИК.");
            Argument.NotNull(loggerFactory, "Не задана утилита логирования ошибок.");

            _userUnit = userUnit;
            _paymentActionUnit = paymentActionUnit;
            _transfersRegisterUnit = transfersRegisterUnit;

            _bankUtility = bankUtility;
            _logUtility = new Lazy<ILogger>(() => loggerFactory.GetLogger<UserProfileController>());
        }

        /// <summary>
        /// Получить страницу с профилем текущего пользователя.
        /// </summary>
        /// <returns>Страница с профилем текущего пользователя.</returns>
        [HttpGet, Authorize]
        public ActionResult UserProfile()
        {
            var userID = GetCurrentUserID();
            var user = _userUnit.GetByID(userID);
            return View(user);
        }

        /// <summary>
        /// Редактировать профиль текущего пользователя.
        /// </summary>
        /// <returns>Страница редактирования профиля пользователя.</returns>
        [HttpGet, Authorize]
        public ActionResult UserProfileEdit()
        {
            var userID = GetCurrentUserID();
            var user = _userUnit.GetByID(userID);
            return View(user);
        }

        /// <summary>
        /// Сохранить профиль пользователя.
        /// </summary>
        /// <param name="model">Профиль пользователя.</param>
        /// <returns>Страница просмотра профиля пользователя.</returns>
        [HttpPost, Authorize, MultipleButton(Name = "action", Argument = "Save")]
        public ActionResult Save(User model)
        {
            if (!ModelState.IsValid)
            {
                return View("UserProfileEdit", model);
            }

            try
            {
                _userUnit.UpdateUserProfile(model);
                return RedirectToAction("UserProfile");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("UserProfileEdit", model);
            }
        }

        /// <summary>
        /// Пополнить баланс текущего пользователя.
        /// </summary>
        /// <param name="amountOfMoney">Сумма пополнения баланса.</param>
        /// <returns>Страница пополнения баланса Робокассы.</returns>
        [HttpPost, Authorize]
        public ActionResult ExecuteFillUpBalance(string amountOfMoney)
        {
            try
            {
                int userID = GetCurrentUserID();
                decimal decimalUpSumm;
                try
                {
                    decimalUpSumm = Convert.ToDecimal(amountOfMoney);
                }
                catch (Exception ex)
                {
                    throw new Exception("Некорректно введена сумма пополнения", ex);
                }
                //create payment action in DB for filling up balance and redirect to robokassa
                PaymentAction action = _paymentActionUnit.InitializeFillUpBalance(userID, decimalUpSumm, null);
                string redirectUrl = Robokassa.GetRedirectUrl(decimalUpSumm, action.PaymentTransactionID);
                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("UserProfile");
            }
        }

        /// <summary>
        /// Открыть страницу запроса суммы для пополнения баланса.
        /// </summary>
        /// <returns>Страница запроса суммы для пополнения баланса.</returns>
        [HttpGet, Authorize]
        public ActionResult FillUpBalance()
        {
			return View();
        }

        /// <summary>
        /// Send money to another user.
        /// </summary>
        /// <returns>Redirects to SendMoney view.</returns>
        [HttpGet, Authorize]
        public ActionResult SendMoney()
        {
            SendModeyPackage moneyPackage = new SendModeyPackage();
            moneyPackage.SenderUserId = GetCurrentUserID();
            return _USER_PROFILE_SEND_MONEY(moneyPackage);
        }

        /// <summary>
        /// Perform money transfer.
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        [HttpPost, Authorize]
        public ActionResult ExecuteSendMoney(SendModeyPackage package)
        {
            Argument.NotNull(package, "Модель для пересылки денег от одного пользователя другому пустая.");
            Argument.Require(package.SenderUserId == GetCurrentUserID(), "Нельзя посылать деньги от лица других пользователей.");
            PaymentTransaction transaction = _paymentActionUnit.InitializeAndCommitSendMoney(package);
            return RedirectToAction("UserProfile");
        }

        /// <summary>
        /// Получить список денежных операций текущего пользователя.
        /// </summary>
        /// <returns>Страница со списком денежных операций текущего пользователя.</returns>
        [HttpGet, Authorize]
        public ActionResult MyPayments()
        {
            var userID = GetCurrentUserID();
            var paymentActions = _paymentActionUnit.GetByUserID(userID);
            return View(paymentActions);
        }

        /// <summary>
        /// Получить информацию о банке по его БИК.
        /// </summary>
        /// <param name="bic">БИК банка.</param>
        /// <returns>Информация о банке в формате JSON.</returns>
        [HttpGet, Authorize]
        public ActionResult GetBankInfo(string bic)
        {
            IBankInfo bankInfo = null;
            try
            {
                bankInfo = _bankUtility.GetBankInfo(bic);
            }
            catch (Exception e)
            {
                _logUtility.Value.Error(e, e.Message);
                bankInfo = new BicInfo();
            }

            return Json(bankInfo, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Получить идентификатор текущего пользователя.
        /// </summary>
        /// <returns>Идентификатор текущего пользователя, если он авторизован.</returns>
        [HttpGet, Authorize]
        public ActionResult GetUserID()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Json(new { userID = GetCurrentUserID() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { userID = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Добавить идентификатор устройства (браузера) текущего пользователя в его группу устройств.
        /// </summary>
        /// <param name="deviceID">Идентификатор устройства (браузера) текущего пользователя.</param>
        /// <returns>Код группы устройств пользователя (совпадает с ID пользователя).</returns>
        [HttpGet, Authorize]
        public ActionResult AddDeviceToGroup(string deviceID)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userID = GetCurrentUserID();
                new FirebaseNotificator().AddDeviceToGroup(deviceID, userID.ToString());
                return Json(new { groupID = userID.ToString() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { groupID = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet, Authorize]
        public ActionResult RequestCash()
        {
            CashRequest cashRequest = new CashRequest()
            {
                UserID = GetCurrentUserID(),
                CreatedOn = DateTime.Now,
                CashRequestStatusID = (int)CashRequestStatusEnum.InProgress,
                CashRequestStatusComment = "В процессе обработки."
            };
            return _USER_PROFILE_REQUEST_CASH(cashRequest);
        }

        [HttpPost, Authorize]
        public ActionResult ExecuteRequestCash(CashRequest request)
        {
            Argument.NotNull(request, "Модель для запроса вывода денег пустая.");
            Argument.Require(request.UserID == GetCurrentUserID(), "Нельзя выводить деньги от лица других пользователей.");
            request.CreatedOn = DateTime.Now;
            request.CashRequestStatusID = (int)CashRequestStatusEnum.InProgress;
            request.CashRequestStatusComment = "В процессе обработки.";
            Validation validation = _transfersRegisterUnit.ValidateCashRequest(request);
            if (!validation.IsValid)
            {
                validation.Errors.ForEach(e => ModelState.AddModelError("", e));
                return _USER_PROFILE_REQUEST_CASH(request);
            }
            _transfersRegisterUnit.RequestCash(request);
            return RedirectToAction("CashRequests");
        }

        /// <summary>
        /// Predict how many cash requests will be in next transfers register.
        /// </summary>
        /// <returns>JSON data with TransfersRegister object.</returns>
        [HttpGet, Authorize]
        public ActionResult GetNextTransferRegisterPrediction()
        {
            TransfersRegister transfersRegister = _transfersRegisterUnit.GetNextTransferRegisterPrediction();
            return Json(transfersRegister, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Authorize]
        public ActionResult TransfersRegisters()
        {
            int userID = GetCurrentUserID();
            List<TransfersRegister> registers = _transfersRegisterUnit.GetTransfersRegisters(userID);
            return _USER_PROFILE_TRANSFER_REGISTERS(registers);
        }

        [HttpPost, Authorize]
        public ActionResult NewTransfersRegister()
        {
            int userID = GetCurrentUserID();
            TransfersRegister register = _transfersRegisterUnit.CreateTransfersRegister(userID);
            return RedirectToAction("TransfersRegisters");
        }

        [HttpGet, Authorize]
        public ActionResult CashRequests()
        {
            int userID = GetCurrentUserID();
            List<CashRequestExtended> requests = _transfersRegisterUnit.GetCashRequests(userID);
            return _USER_PROFILE_CASH_REQUESTS(requests);
        }

        private ViewResult _USER_PROFILE_SEND_MONEY(SendModeyPackage package)
        {
            int userID = GetCurrentUserID();
            User user = _userUnit.GetByID(userID);
            ViewBag.SenderUser = user;
            return View("SendMoney", package);
        }
        private ViewResult _USER_PROFILE_REQUEST_CASH(CashRequest request)
        {
            User user = _userUnit.GetByID(request.UserID);
            ViewBag.User = user;
            return View("RequestCash", request);
        }
        private ViewResult _USER_PROFILE_CASH_REQUESTS(List<CashRequestExtended> requests)
        {
            return View("CashRequests", requests);
        }
        private ViewResult _USER_PROFILE_TRANSFER_REGISTERS(List<TransfersRegister> registers)
        {
            return View("TransfersRegisters", registers);
        }
    }
}
