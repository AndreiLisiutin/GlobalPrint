using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Printers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Printers
{
    public interface IPrinterScheduleRepository : IRepository<PrinterSchedule>
    {
    }
}