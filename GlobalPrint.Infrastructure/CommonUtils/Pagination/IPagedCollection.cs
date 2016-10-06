using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.CommonUtils.Pagination
{
    public interface IPagedCollection
    {
        int Count { get; set; }
        int ItemsPerPage { get; set; }
        int CurrentPage { get; set; }
        int PagesCount { get; }
    }
}
