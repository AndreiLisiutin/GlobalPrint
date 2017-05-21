using GlobalPrint.ClientWeb.Models.Lookup;
using GlobalPrint.Configuration.DI;
using GlobalPrint.Infrastructure.CommonUtils.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb.Controllers.Utils
{
    public class LookupController : BaseController
    {
        ILookupManagerFactory _lookupManagerFactory;
        public LookupController(ILookupManagerFactory lookupManagerFactory)
        {
            this._lookupManagerFactory = lookupManagerFactory;
        }
        public LookupController()
            : this (IoC.Instance.Resolve<ILookupManagerFactory>())
        {
        }

        /// <summary>
        /// Creates modal window with list of objects for selection one of them.
        /// </summary>
        /// <param name="lookupType">Type of lookup, enumeration.</param>
        /// <param name="searchText">Search text parameter.</param>
        /// <param name="paging">Paging parameter.</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult Lookup(LookupTypeEnum lookupType, string searchText, Paging paging, string sortByIdentifier, SortByEnum? sortByDirection)
        {
            paging = paging ?? new Paging();
            
            ILookupManager lookupManager = this._lookupManagerFactory.CreateLookupManager(lookupType);

            List<LookupResultValue> columns = lookupManager.GetColumns(sortByIdentifier, sortByDirection);

            PagedList<List<LookupResultValue>> entities = lookupManager.GetEntitiesList(searchText, paging, sortByIdentifier, sortByDirection);

            ViewBag.SearchText = searchText;
            ViewBag.LookupType = lookupType;
            
            return PartialView("Lookup", new LookupViewModel(columns, entities));
        }

        /// <summary>
        /// Get string value of lookup by its id.
        /// </summary>
        /// <param name="lookupType">Type of lookup.</param>
        /// <param name="id">Identifier of the record.</param>
        /// <returns>Text value property of the lookup.</returns>
        public ActionResult GetValue(LookupTypeEnum lookupType, long id)
        {
            ILookupManager lookupManager = this._lookupManagerFactory.CreateLookupManager(lookupType);
            List<LookupResultValue> entity = lookupManager.GetEntitityByID(id);
            string value = entity.First(e => e.IsText).Value;
            return Json(value, JsonRequestBehavior.AllowGet);
        }
    }
}