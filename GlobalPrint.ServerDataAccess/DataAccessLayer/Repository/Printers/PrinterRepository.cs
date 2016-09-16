﻿using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Printers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Printers;
using GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext;

namespace GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Printers
{
    public class PrinterRepository : BaseRepository<Printer>, IPrinterRepository
    {
        public PrinterRepository(DbConnectionContext context) : base(context)
        {
        }
    }
}
