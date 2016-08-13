using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Printers;

namespace GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Printers
{
    /// <summary>
    /// Интерфейс репозитория принтеров
    /// </summary>
    public interface IPrinterRepository : IRepository<Printer>
    {
    }
}
