using GlobalPrint.ClientWeb.Models.Lookup;
using GlobalPrint.ClientWeb.Models.MenuButton;
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
            //< label class="labelBox input-group">
            //	<input type = "text" class="input-def" style="width:100%">
            //	<div class="dLabel"><img src = "~/Content/img/search.png" /></ div >
            //</ label >

            var inputGroup = new TagBuilder("label");
            inputGroup.AddCssClass("labelBox");
            inputGroup.AddCssClass("input-group");
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
            lookupValueName.AddCssClass("input-def");
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

        /// <summary>
        /// Меню с кнопками.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="menuItems">Список пунктов меню.</param>
        /// <param name="htmlAttributes">Список дополнительных атрибутов внешнего div'а.</param>
        /// <returns>Кнопка меню.</returns>
        public static MvcHtmlString ButtonMenu(this HtmlHelper html, List<MenuItem> menuItems, object htmlAttributes = null)
        {
            //<a href="" class="toggle-menu-button"><span></span></a>
            //<div class="toggle-menu">
            //    <ul>
            //          <li><a href="/Home/Buy">Buy</a></li>
            //    </ul>
            //</div>

            var a = new TagBuilder("a");
            a.AddCssClass("toggle-menu-button");
            a.MergeAttribute("href", "");
            var span = new TagBuilder("span");
            span.AddCssClass("toggle-menu-button-span");
            a.InnerHtml += span;

            Action<TagBuilder, MenuItem> MergeHtmlAttributes = delegate (TagBuilder localTag, MenuItem localMenuItem)
            {
                // Добавим классы и html атрибуты
                if (localMenuItem.HtmlAttributes != null)
                {
                    foreach (var property in localMenuItem.HtmlAttributes.GetType().GetProperties())
                    {
                        localTag.MergeAttribute(property.Name, (property.GetValue(htmlAttributes) ?? "").ToString());
                    }
                }
                if (localMenuItem.Classes?.Any() ?? false)
                {
                    localMenuItem.Classes.ForEach(c => localTag.AddCssClass(c));
                }
            };

            var ul = new TagBuilder("ul");
            foreach (var menuItem in menuItems)
            {
                TagBuilder linkComponent = null;
                if (menuItem.IsPostOperation)
                {
                    // <form method="POST" action="/Home/Buy">
                    //     <a href="#" onclick="$(this).closest('form').submit()">Submit Link</a>
                    // </form>
                    linkComponent = new TagBuilder("form");
                    linkComponent.MergeAttribute("method", "POST");
                    linkComponent.MergeAttribute("action", menuItem.Reference);
                    
                    var submit = new TagBuilder("a");
                    submit.MergeAttribute("href", "#");
                    submit.MergeAttribute("onclick", "$(this).closest('form').submit(); return false;");
                    submit.InnerHtml = menuItem.Name;
                    linkComponent.InnerHtml = submit.ToString();
                }
                else
                {
                    // <a href="/Home/Buy">Buy</a>
                    linkComponent = new TagBuilder("a");
                    linkComponent.MergeAttribute("href", menuItem.Reference);
                    linkComponent.InnerHtml = menuItem.Name;
                }
                MergeHtmlAttributes(linkComponent, menuItem);

                var li = new TagBuilder("li");
                li.InnerHtml = linkComponent.ToString();
                ul.InnerHtml += li.ToString();
            }

            var menuDiv = new TagBuilder("div");
            menuDiv.AddCssClass("toggle-menu");
            menuDiv.InnerHtml = ul.ToString();

            var outerDiv = new TagBuilder("div");
            outerDiv.InnerHtml += a;
            outerDiv.InnerHtml += menuDiv;
            if (htmlAttributes != null)
            {
                foreach (var property in htmlAttributes.GetType().GetProperties())
                {
                    outerDiv.MergeAttribute(property.Name, (property.GetValue(htmlAttributes) ?? "").ToString());
                }
            }

            return new MvcHtmlString(outerDiv.ToString());
        }
    }
}