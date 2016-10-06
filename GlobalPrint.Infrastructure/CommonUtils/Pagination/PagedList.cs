using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.CommonUtils.Pagination
{
    public class PagedList<T> : IPagedCollection
    {
        public PagedList()
            : this(new List<T>(), 0, 10, 1)
        {

        }
        public PagedList(List<T> entities, int count, int itemsPerPage, int currentPage)
        {
            this.Entities = entities;
            this.Count = count;
            this.ItemsPerPage = itemsPerPage;
            this.CurrentPage = currentPage;
        }

        public List<T> Entities { get; set; }
        public int Count { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }

        public int PagesCount
        {
            get
            {
                return (int)Math.Ceiling((double)this.Count / this.ItemsPerPage);
            }
        }
    }
}
