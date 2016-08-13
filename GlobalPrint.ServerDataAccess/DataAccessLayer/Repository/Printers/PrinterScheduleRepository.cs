using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Printers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Printers;
using GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext;
using GlobalPrint.ServerDataAccess.DataAccessLayer.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Printers
{
    public class PrinterScheduleRepository : BaseRepository<PrinterSchedule>, IPrinterScheduleRepository
    {
        public PrinterScheduleRepository(DbConnectionContext context) : base(context)
        {
        }
    }
}