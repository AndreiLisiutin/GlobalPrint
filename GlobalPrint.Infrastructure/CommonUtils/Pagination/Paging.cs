using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.CommonUtils.Pagination
{
    public class Paging
    {
        public Paging()
            :this(1, 10)
        {

        }
        public Paging(int currentPage, int itemsPerPage)
        {
            this.CurrentPage = currentPage;
            this.ItemsPerPage = itemsPerPage;
        }

        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }

        public int Skip
        {
            get
            {
                return Math.Max(0, (this.CurrentPage - 1) * ItemsPerPage);
            }
        }
    }
}
