﻿using GlobalPrint.ClientWeb.Filters;
using GlobalPrint.Configuration.DI;
using GlobalPrint.Infrastructure.LogUtility.Robokassa;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Payment;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Payment;
using System.Web.Mvc;
using System;
using GlobalPrint.ServerBusinessLogic.Models.Business.Payments;
using System.Collections.Generic;
using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using GlobalPrint.ClientWeb.Helpers;
using GlobalPrint.Infrastructure.CommonUtils.Pagination;
using System.Linq;
using GlobalPrint.ClientWeb.Models.Lookup;
using GlobalPrint.Infrastructure.BankUtility;
using GlobalPrint.Infrastructure.BankUtility.BankInfo;
using GlobalPrint.Infrastructure.BankUtility.BicInfo;
using GlobalPrint.Infrastructure.LogUtility;
using GlobalPrint.ServerBusinessLogic.Models.Domain.TransfersRegisters;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.TransfersRegisters;

namespace GlobalPrint.ClientWeb
{
    public class UserProfileController : BaseController
    {
        private UserUnit _userUnit;
        private PaymentActionUnit _paymentActionUnit;
        private TransfersRegisterUnit _transfersRegisterUnit;
        private IBankUtility _bankUtility;
        private Lazy<ILogger> _logUtility;
        public UserProfileController()
            : this(IoC.Instance.Resolve<UserUnit>(), new PaymentActionUnit(),
                  IoC.Instance.Resolve<IBankUtility>(), IoC.Instance.Resolve<ILoggerFactory>(), new TransfersRegisterUnit())
        {
        }
        public UserProfileController(UserUnit userUnit, PaymentActionUnit paymentActionUnit,
            IBankUtility bankUtility, ILoggerFactory loggerFactory, TransfersRegisterUnit transfersRegisterUnit)
        {
            this._userUnit = userUnit;
            this._paymentActionUnit = paymentActionUnit;
            this._bankUtility = bankUtility;
            this._logUtility = new Lazy<ILogger>(() => loggerFactory.GetLogger<UserProfileController>());
            this._transfersRegisterUnit = transfersRegisterUnit;
        }

        /// <summary>
        /// Get user profile view.
        /// </summary>
        /// <returns>Profile info of current user.</returns>
        // GET: UserProfile/UserProfile
        [HttpGet]
        [Authorize]
        public ActionResult UserProfile()
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
        [HttpPost]
        [Authorize]
        [MultipleButton(Name = "action", Argument = "Save")]
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

        [HttpPost]
        [Authorize]
        [MultipleButton(Name = "action", Argument = "FillUpBalance")]
        public ActionResult FillUpBalance(string upSumm)
        {
            try
            {
                UserUnit userUnit = IoC.Instance.Resolve<UserUnit>();
                int userID = this.GetCurrentUserID();
                decimal decimalUpSumm;
                try
                {
                    decimalUpSumm = Convert.ToDecimal(upSumm);
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


        [HttpGet]
        [Authorize]
        public ActionResult SendMoney()
        {
            SendModeyPackage moneyPackage = new SendModeyPackage();
            moneyPackage.SenderUserId = this.GetCurrentUserID();
            return this._USER_PROFILE_SEND_MONEY(moneyPackage);
        }

        [HttpPost]
        [Authorize]
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
        [HttpGet]
        [Authorize]
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
                this._logUtility.Value.Error(e, e.Message);
                bankInfo = new BicInfo();
            }

            return Json(bankInfo, JsonRequestBehavior.AllowGet);
        }


        [HttpGet, Authorize]
        public ActionResult RequestCash()
        {
            CashRequest cashRequest = new CashRequest()
            {
                UserID = this.GetCurrentUserID(),
                CreatedOn = DateTime.Now,
                CashRequestStatusID = (int)CashRequestStatusEnum.InProgress
            };
            return _USER_PROFILE_REQUEST_CASH(cashRequest);
        }

        [HttpPost]
        [Authorize]
        public ActionResult ExecuteRequestCash(CashRequest request)
        {
            Argument.NotNull(request, "Модель для запроса вывода денег пустая.");
            Argument.Require(request.UserID == this.GetCurrentUserID(), "Нельзя выводить деньги от лица других пользователей.");
            request.CreatedOn = DateTime.Now;
            request.CashRequestStatusID = (int)CashRequestStatusEnum.InProgress;
            Validation validation = this._transfersRegisterUnit.ValidateCashRequest(request);
            if (!validation.IsValid)
            {
                validation.Errors.ForEach(e => ModelState.AddModelError("", e));
                return this._USER_PROFILE_REQUEST_CASH(request);
            }
            this._transfersRegisterUnit.RequestCash(request);
            return RedirectToAction("UserProfile");
        }

        [HttpGet, Authorize]
        public ActionResult TransfersRegisters()
        {
            int userID = this.GetCurrentUserID();
            List<TransfersRegister> registers = this._transfersRegisterUnit.GetTransfersRegisters(userID);
            return _USER_PROFILE_TRANSFER_REGISTERS(registers);
        }

        [HttpPost, Authorize]
        public ActionResult NewTransfersRegister()
        {
            int userID = this.GetCurrentUserID();
            TransfersRegister register = this._transfersRegisterUnit.CreateTransfersRegister(userID);
            return RedirectToAction("TransfersRegisters");
        }

        private ViewResult _USER_PROFILE_SEND_MONEY(SendModeyPackage package)
        {
            int userID = this.GetCurrentUserID();
            User user = this._userUnit.GetByID(userID);
            ViewBag.SenderUser = user;
            return this.View("SendMoney", package);
        }
        private ViewResult _USER_PROFILE_REQUEST_CASH(CashRequest request)
        {
            User user = this._userUnit.GetByID(request.UserID);
            ViewBag.User = user;
            return this.View("RequestCash", request);
        }
        private ViewResult _USER_PROFILE_TRANSFER_REGISTERS(List<TransfersRegister> registers)
        {
            return this.View("TransfersRegisters", registers);
        }
    }
}
