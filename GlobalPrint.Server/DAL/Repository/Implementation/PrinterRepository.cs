using GlobalPrint.Server.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Server.DAL
{
    public class PrinterRepository : BaseRepository<Printer>, IPrinterRepository
    {
        public PrinterRepository(DbContext context) 
            : base(context)
        {
        }
    }
}
