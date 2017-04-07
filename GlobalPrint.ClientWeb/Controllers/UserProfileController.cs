using GlobalPrint.ClientWeb.Filters;
using GlobalPrint.Configuration.DI;
using GlobalPrint.Infrastructure.BankUtility;
using GlobalPrint.Infrastructure.BankUtility.BicInfo;
using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.Infrastructure.LogUtility;
using GlobalPrint.Infrastructure.LogUtility.Robokassa;
using GlobalPrint.Infrastructure.Notifications;
using GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Payment;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.TransfersRegisters;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Users;
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
    public class UserProfileController : BaseController
    {
        /// <summary>
        /// Unit with user business logic.
        /// </summary>
        private IUserUnit _userUnit;
        /// <summary>
        /// Unit with payment actions business logic.
        /// </summary>
        private PaymentActionUnit _paymentActionUnit;
        /// <summary>
        /// Unit with money transfer registers business logic.
        /// </summary>
        private TransfersRegisterUnit _transfersRegisterUnit;

        /// <summary>
        /// Utility to get data by bank BIC.
        /// </summary>
        private IBankUtility _bankUtility;
        /// <summary>
        /// Errors log utility.
        /// </summary>
        private Lazy<ILogger> _logUtility;

        public UserProfileController()
            : this(IoC.Instance.Resolve<IUserUnit>(), new PaymentActionUnit(),
                  IoC.Instance.Resolve<IBankUtility>(), IoC.Instance.Resolve<ILoggerFactory>(), IoC.Instance.Resolve<TransfersRegisterUnit>())
        {
        }
        public UserProfileController(IUserUnit userUnit, PaymentActionUnit paymentActionUnit,
            IBankUtility bankUtility, ILoggerFactory loggerFactory, TransfersRegisterUnit transfersRegisterUnit)
        {
            _userUnit = userUnit;
            _paymentActionUnit = paymentActionUnit;
            _transfersRegisterUnit = transfersRegisterUnit;

            _bankUtility = bankUtility;
            _logUtility = new Lazy<ILogger>(() => loggerFactory.GetLogger<UserProfileController>());
        }

        /// <summary>
        /// Get user profile view.
        /// </summary>
        /// <returns>Profile info of current user.</returns>
        [HttpGet, Authorize]
        public ActionResult UserProfile()
        {
            UserUnit userUnit = IoC.Instance.Resolve<UserUnit>();
            var user = userUnit.GetByID(this.GetCurrentUserID());
            return View(user);
        }

        /// <summary>
        /// Редактирование профиля пользователя.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize]
        public ActionResult UserProfileEdit()
        {
            UserUnit userUnit = IoC.Instance.Resolve<UserUnit>();
            var user = userUnit.GetByID(this.GetCurrentUserID());
            return View(user);
        }

        /// <summary>
        /// Save user profile info.
        /// </summary>
        /// <param name="model">Profile info of current user.</param>
        /// <returns>Redirects to updated profile view.</returns>
        [HttpPost, Authorize, MultipleButton(Name = "action", Argument = "Save")]
        public ActionResult Save(User model)
        {
            if (!ModelState.IsValid)
            {
                return View("UserProfile", model);
            }
            UserUnit userUnit = IoC.Instance.Resolve<UserUnit>();

            try
            {
                userUnit.UpdateUserProfile(model);
                return RedirectToAction("UserProfile");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("UserProfile", model);
            }
        }

        /// <summary>
        /// Fill the balance of current user.
        /// </summary>
        /// <param name="amountOfMoney">Money amount to fill balance.</param>
        /// <returns>Redirects to Robokassa.</returns>
        [HttpPost, Authorize]
        public ActionResult ExecuteFillUpBalance(string amountOfMoney)
        {
            try
            {
                UserUnit userUnit = IoC.Instance.Resolve<UserUnit>();
                int userID = this.GetCurrentUserID();
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
                PaymentAction action = new PaymentActionUnit().InitializeFillUpBalance(userID, decimalUpSumm, null);
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
		/// Пополнить баланс.
		/// </summary>
		/// <returns></returns>
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
            moneyPackage.SenderUserId = this.GetCurrentUserID();
            return this._USER_PROFILE_SEND_MONEY(moneyPackage);
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
            Argument.Require(package.SenderUserId == this.GetCurrentUserID(), "Нельзя посылать деньги от лица других пользователей.");
            PaymentTransaction transaction = this._paymentActionUnit.InitializeAndCommitSendMoney(package);
            return RedirectToAction("UserProfile");
        }

        /// <summary>
        /// Get list of user's payments.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize]
        public ActionResult MyPayments()
        {
            PaymentActionUnit paymentUnit = new PaymentActionUnit();
            int userID = this.GetCurrentUserID();
            List<PaymentActionFullInfo> actions = paymentUnit.GetByUserID(userID);
            return View(actions);
        }

        /// <summary>
        /// Get bank info by bic code.
        /// </summary>
        /// <param name="bic">Bic code of bank.</param>
        /// <returns>Bank info in JSON.</returns>
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
        /// Update шdentifier of user device for notifications system.
        /// </summary>
        /// <param name="deviceID">Identifier of user device for notifications system.</param>
        /// <returns>Nothing in fact.</returns>
        [HttpGet, Authorize]
        public ActionResult UpdateDeviceID(string deviceID)
        {
            if (User.Identity.IsAuthenticated)
            {
                Argument.NotNullOrWhiteSpace(deviceID, "При изменении идентификатора устройства произошла ошибка. Идентификатор устройства не может быть пустым.");
                var user = _userUnit.UpdateDeviceID(this.GetCurrentUserID(), deviceID);
                return Json(user, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Get device identifier of current user.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize]
        public ActionResult GetDeviceID()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userID = GetCurrentUserID();
                var user = _userUnit.GetByID(GetCurrentUserID());
                return Json(new { deviceID = user.DeviceID }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { deviceID = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Get current user ID.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize]
        public ActionResult GetUserID()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userID = this.GetCurrentUserID();
                return Json(new { userID = userID }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { userID = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Add current user device into devices group.
        /// </summary>
        /// <param name="deviceID">Current device identifier.</param>
        /// <returns></returns>
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
