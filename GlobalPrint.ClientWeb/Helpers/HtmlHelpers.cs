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

            List<int> pages = GetPagesToShow(pagedCollection.CurrentPage, pagedCollection.PagesCount);
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
        private static List<int> GetPagesToShow(int currentPage, int pageCount, int showMax = 10)
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
    }
}