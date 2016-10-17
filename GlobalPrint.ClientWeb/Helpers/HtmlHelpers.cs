using GlobalPrint.ClientWeb.Models.Lookup;
using GlobalPrint.Infrastructure.CommonUtils.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb.Helpers
{

    public static class HtmlHelpers
    {
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
            //< div class="input-group lookup-input-group" data-lookup-type="{lookupType}">
            //    <input type = "text" class="hidden form-control lookup-value-id" name="lookupValueId" id="lookupValueId">
            //    <input type = "text" class="form-control lookup-value-name" disabled="" name="lookupValueName" id="lookupValueName">
            //    <div class="input-group-btn">
            //        <button class="btn btn-default lookup-show-button" id="lookupButton" type="submit"><i class="glyphicon glyphicon-search"></i></button>
            //    </div>
            //</div>

            var inputGroup = new TagBuilder("div");
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
            lookupValueName.AddCssClass("form-control");
            lookupValueName.AddCssClass("lookup-value-name");
            lookupValueName.MergeAttribute("type", "text");
            lookupValueName.MergeAttribute("name", "lookupValueName");
            lookupValueName.MergeAttribute("id", "lookupValueName");
            lookupValueName.MergeAttribute("disabled", "disabled");
            inputGroup.InnerHtml += lookupValueName.ToString();

            var inputGroupBtn = new TagBuilder("div");
            inputGroupBtn.AddCssClass("input-group-btn");

            var lookupButton = new TagBuilder("button");
            lookupButton.AddCssClass("btn");
            lookupButton.AddCssClass("btn-default");
            lookupButton.AddCssClass("lookup-show-button");
            lookupButton.MergeAttribute("id", "lookupButton");
            lookupButton.MergeAttribute("type", "button");

            var lookupButtonCaption = new TagBuilder("i");
            lookupButtonCaption.AddCssClass("glyphicon");
            lookupButtonCaption.AddCssClass("glyphicon-search");
            lookupButton.InnerHtml += lookupButtonCaption.ToString();
            inputGroupBtn.InnerHtml += lookupButton.ToString();

            inputGroup.InnerHtml += inputGroupBtn.ToString();
            return new MvcHtmlString(inputGroup.ToString());
        }
    }
}