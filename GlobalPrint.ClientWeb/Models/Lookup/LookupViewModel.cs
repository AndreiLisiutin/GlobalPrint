using GlobalPrint.Infrastructure.CommonUtils.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobalPrint.ClientWeb.Models.Lookup
{
    /// <summary>
    /// View model for lookup view.
    /// </summary>
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

        /// <summary>
        /// List of resulting data set columns.
        /// </summary>
        public List<LookupResultValue> Columns { get; set; }

        /// <summary>
        /// Resulting data set.
        /// </summary>
        public PagedList<List<LookupResultValue>> Values { get; set; }
    }
}