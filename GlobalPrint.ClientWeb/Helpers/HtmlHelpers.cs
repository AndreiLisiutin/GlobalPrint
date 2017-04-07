using GlobalPrint.ClientWeb.Models.Lookup;
using GlobalPrint.Infrastructure.CommonUtils.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GlobalPrint.ClientWeb.Helpers
{

    public static class HtmlHelpers
    {
		/// <summary>
		/// Получить класс для ссылки пункта меню в зависимости от текущего маршрута.
		/// </summary>
		/// <param name="html"></param>
		/// <param name="controllers"></param>
		/// <param name="actions"></param>
		/// <param name="cssClass"></param>
		/// <returns></returns>
		public static string IsSelected(this HtmlHelper html, string controllers = "", string actions = "", string cssClass = "active")
		{
			ViewContext viewContext = html.ViewContext;
			bool isChildAction = viewContext.Controller.ControllerContext.IsChildAction;

			if (isChildAction)
				viewContext = html.ViewContext.ParentActionViewContext;

			RouteValueDictionary routeValues = viewContext.RouteData.Values;
			string currentAction = routeValues["action"].ToString();
			string currentController = routeValues["controller"].ToString();

			if (String.IsNullOrEmpty(actions))
				actions = currentAction;

			if (String.IsNullOrEmpty(controllers))
				controllers = currentController;

			string[] acceptedActions = actions.Trim().Split(',').Distinct().ToArray();
			string[] acceptedControllers = controllers.Trim().Split(',').Distinct().ToArray();

			return acceptedActions.Contains(currentAction) && acceptedControllers.Contains(currentController) ?
				cssClass : String.Empty;
		}

		/// <summary>
		/// Create paging element for grid.
		/// </summary>
		/// <param name="html"></param>
		/// <param name="pagedCollection">Collection with information about pagination state.</param>
		/// <param name="url">Action HREF builder from page number. Try it by Url.Action(...).</param>
		/// <returns></returns>
		public static MvcHtmlString Paging(this HtmlHelper html, IPagedCollection pagedCollection, Func<int, string> url)
        {
            if (pagedCollection.PagesCount == 1)
            {
                return new MvcHtmlString("");
            }

            List<int> pages = _GetPagesToShow(pagedCollection.CurrentPage, pagedCollection.PagesCount);
            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination");
            foreach (int page in pages)
            {
                var li = new TagBuilder("li");
                if (page == pagedCollection.CurrentPage)
                {
                    li.AddCssClass("active");
                }
                var a = new TagBuilder("a");
                a.MergeAttribute("href", url(page));
                a.MergeAttribute("data-page", page.ToString());
                a.InnerHtml = page.ToString();
                li.InnerHtml += a.ToString();
                ul.InnerHtml += li.ToString();
            }
            return new MvcHtmlString(ul.ToString());
        }

        /// <summary>
        /// Define pages that should be displayed to user.
        /// </summary>
        /// <param name="currentPage">Current page of the pagination.</param>
        /// <param name="pageCount">Count of all pages.</param>
        /// <param name="showMax">Maximum number of pages to show to user.</param>
        /// <returns>List of integers as page numbers.</returns>
        private static List<int> _GetPagesToShow(int currentPage, int pageCount, int showMax = 10)
        {
            List<int> pages = new List<int>();
            int maxLeftShift = (int)Math.Ceiling(0.33 * showMax);

            pages.Add(1); //first
            pages.Add(pageCount); //last
            pages.Add(currentPage); //current
            for (var newPage = currentPage - maxLeftShift; newPage <= pageCount; newPage++)
            {
                if (newPage > 0 && !pages.Contains(newPage) && pages.Count < showMax)
                {
                    pages.Add(newPage);
                }
            }
            return pages
                .Distinct()
                .OrderBy(e => e)
                .ToList();
        }

        /// <summary>
        /// Create lookup of a certain type.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="lookupType">Type of lookup.</param>
        /// <returns></returns>
        public static MvcHtmlString Lookup(this HtmlHelper html, LookupTypeEnum lookupType, string name = "lookupValueId", long? value = null, object htmlAttributes = null)
        {
			//< label class="labelBox">
			//	<input type = "text" class="input-def order-code" style="width:100%">
			//	<div class="dLabel"><img src = "~/Content/img/search.png" /></ div >
			//</ label >

			var inputGroup = new TagBuilder("label");
            inputGroup.AddCssClass("labelBox");
			inputGroup.AddCssClass("lookup-input-group");
			inputGroup.MergeAttribute("data-lookup-type", ((int)lookupType).ToString());
			if (htmlAttributes != null)
            {
                foreach (var property in htmlAttributes.GetType().GetProperties())
                {
                    inputGroup.MergeAttribute(property.Name, (property.GetValue(htmlAttributes) ?? "").ToString());
                }
            }

            var lookupValueId = new TagBuilder("input");
            lookupValueId.AddCssClass("hidden");
            lookupValueId.AddCssClass("form-control");
            lookupValueId.AddCssClass("lookup-value-id");
            lookupValueId.MergeAttribute("type", "text");
            lookupValueId.MergeAttribute("name", name);
            lookupValueId.MergeAttribute("id", "lookupValueId");
            if (value.HasValue && value > 0)
            {
                lookupValueId.MergeAttribute("value", value.ToString());
            }
            inputGroup.InnerHtml += lookupValueId.ToString();

            var lookupValueName = new TagBuilder("input");
            lookupValueName.MergeAttribute("type", "text");
            lookupValueName.MergeAttribute("name", "lookupValueName");
            lookupValueName.MergeAttribute("id", "lookupValueName");
			lookupValueName.MergeAttribute("disabled", "disabled");
            lookupValueName.MergeAttribute("style", "width:100%");
			lookupValueName.AddCssClass("lookup-value-name");
            lookupValueName.AddCssClass("form-control");
            lookupValueName.AddCssClass("input-def");
            lookupValueName.AddCssClass("order-code");
            inputGroup.InnerHtml += lookupValueName.ToString();

            var inputGroupBtn = new TagBuilder("div");
            inputGroupBtn.AddCssClass("input-group-btn");
            inputGroupBtn.AddCssClass("dLabel");
			inputGroupBtn.MergeAttribute("id", "lookupButton");
			inputGroupBtn.AddCssClass("lookup-show-button");

            var lookupButton = new TagBuilder("img");
            lookupButton.MergeAttribute("src", new Uri(new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority)), "Content/img/search.png").AbsoluteUri);
			inputGroupBtn.InnerHtml += lookupButton.ToString();

			inputGroup.InnerHtml += inputGroupBtn.ToString();
            return new MvcHtmlString(inputGroup.ToString());
        }
    }
}