using GlobalPrint.ClientWeb.Filters;
using GlobalPrint.Configuration.DI;
using GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Users;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new BaseErrorHandleAttribute());
            filters.Add(new UpdateActivityAttribute(IoC.Instance.Resolve<IUserUnit>()));
        }
    }
}