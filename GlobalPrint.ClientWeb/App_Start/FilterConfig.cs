using GlobalPrint.ClientWeb.Filters;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new BaseErrorHandleAttribute());
        }
    }
}