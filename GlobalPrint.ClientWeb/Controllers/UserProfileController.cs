using GlobalPrint.ClientWeb.Filters;
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

namespace GlobalPrint.ClientWeb
{
    public class UserProfileController : BaseController
    {
        private UserUnit _userUnit;
        public UserProfileController()
        {
            this._userUnit = IoC.Instance.Resolve<UserUnit>();
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
            var user = userUnit.GetUserByID(this.GetCurrentUserID());
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
        /// Get list of users with ability to search. 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult UsersListPartial(LookupTypeEnum lookupType, string searchText, Paging paging)
        {
            paging = paging ?? new Paging();

            ILookupManager lookupManager = new LookupManagerFactory().GetLookupManager(lookupType);

            List<LookupResultValue> columns = lookupManager.GetColumns();
            PagedList<List<LookupResultValue>> entities = lookupManager.GetEntitiesList(searchText, paging);
            
            ViewBag.SearchText = searchText;
            ViewBag.LookupType = lookupType;

            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            return PartialView("UsersListPartial", new LookupViewModel(columns, entities));
        }
    }

    public class LookupManagerFactory
    {
        public ILookupManager GetLookupManager(LookupTypeEnum lookupType)
        {
            switch (lookupType)
            {
                case LookupTypeEnum.User:
                    return new UserLookupManager();
                default:
                    throw new InvalidOperationException("Unknown type of lookup manager.");
            }
        }
    }

    public interface ILookupManager
    {
        PagedList<List<LookupResultValue>> GetEntitiesList(string searchText, Paging paging);
        List<LookupResultValue> GetColumns();
    }

    public abstract class BaseLookupManager<T> : ILookupManager
    {
        public abstract List<T> GetEntities(string searchText, Paging paging);
        public abstract int GetCount(string searchText);
        public abstract List<LookupResultValue> Convert(T entity);
        public virtual T GetEmptyEntity()
        {
            return Activator.CreateInstance<T>();
        }

        public virtual PagedList<List<LookupResultValue>> GetEntitiesList(string searchText, Paging paging)
        {
            List<T> entities = this.GetEntities(searchText, paging);
            List<List<LookupResultValue>> lookupList = entities.Select(e => this.Convert(e)).ToList();
            int count = this.GetCount(searchText);

            return new PagedList<List<LookupResultValue>>(lookupList, count, paging.ItemsPerPage, paging.CurrentPage);
        }
        public virtual List<LookupResultValue> GetColumns()
        {
            T entity = this.GetEmptyEntity();
            List<LookupResultValue> lookupEntity = this.Convert(entity);
            return lookupEntity;
        }
    }

    public class UserLookupManager : BaseLookupManager<User>
    {
        private UserUnit _userUnit;
        public UserLookupManager()
            : this (IoC.Instance.Resolve<UserUnit>())
        {
        }
        public UserLookupManager(UserUnit userUnit)
        {
            this._userUnit = userUnit;
        }
        public override List<User> GetEntities(string searchText, Paging paging)
        {
            return this._userUnit.GetByFilter(searchText, paging);
        }
        public override int GetCount(string searchText)
        {
            return this._userUnit.CountByFilter(searchText);
        }
        public override List<LookupResultValue> Convert(User entity)
        {
            return new List<LookupResultValue>()
            {
                new LookupResultValue("ID", entity.ID.ToString(), 0, isIdentifier: true),
                new LookupResultValue("Email", entity.Email, 5, isText: true),
                new LookupResultValue("Логин", entity.UserName, 4),
                new LookupResultValue("Дата последней активности", entity.LastActivityDate.ToString("dd.MM.yyyy HH:mm:ss"), 2),
            };
        }
    }

    public enum LookupTypeEnum
    {
        User = 1
    }

    public class LookupViewModel
    {
        public LookupViewModel()
        {

        }
        public LookupViewModel(List<LookupResultValue> columns, PagedList<List<LookupResultValue>> values)
        {
            this.Columns = columns;
            this.Values = values;
        }
        public List<LookupResultValue> Columns { get; set; }
        public PagedList<List<LookupResultValue>> Values { get; set; }
    }
    
    public class LookupResultValue
    {
        public LookupResultValue()
            :this("Column name", "Value", 1, false, false)
        {
        }
        public LookupResultValue(string name, string value, int length, bool isIdentifier = false, bool isText = false)
        {
            this.Name = name;
            this.Value = value;
            this.Length = length;
            this.IsIdentifier = isIdentifier;
            this.IsText = isText;
        }
        public string Name { get; set; }
        public string Value { get; set; }
        public int Length { get; set; }
        public bool IsIdentifier { get; set; }
        public bool IsText { get; set; }
    }
}
