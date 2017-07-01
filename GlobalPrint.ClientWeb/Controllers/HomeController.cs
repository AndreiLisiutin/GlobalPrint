using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Printers;
using GlobalPrint.ServerBusinessLogic.Models.Business.Printers;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb
{
    public class HomeController : BaseController
    {
        PrinterUnit _printerUnit;
        public HomeController()
            :this(new PrinterUnit())
        {
        }
        public HomeController(PrinterUnit printerUnit)
        {
            _printerUnit = printerUnit;
        }
        
        /// <summary>
        /// Entry point of the application.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        //http://stackoverflow.com/questions/16941317/a-public-action-method-was-not-found-on-controller
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Получить список принтеров по фильтру.
        /// </summary>
        /// <param name="filter">Фильтр для принтеров.</param>
        /// <returns>Список найденных принтеров.</returns>
        [HttpGet]
        public ActionResult GetPrinters(PrinterSearchFilter filter)
        {
            filter = filter ?? new PrinterSearchFilter();
            var printers = _printerUnit.GetFullByFilter(filter);
            return Json(printers);
        }

        /// <summary>
        /// Найти ближайший принтер.
        /// </summary>
        /// <param name="latitude">Широта текущего местоположения.</param>
        /// <param name="longtitude">Долгота текущего местоположения.</param>
        /// <returns>Ближайший принтер.</returns>
        [HttpGet]
        public ActionResult GetClosestPrinter(float latitude, float longtitude)
        {
            PrinterFullInfoModel printer = _printerUnit.GetClosest(latitude, longtitude);
            if (printer == null)
            {
                return Json("");
            }
            return Json(printer);
        }
    }
}
