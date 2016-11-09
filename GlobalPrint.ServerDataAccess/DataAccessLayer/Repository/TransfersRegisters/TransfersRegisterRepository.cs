﻿using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Printers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Printers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.TransfersRegisters;
using GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext;

namespace GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Printers
{
    public class TransfersRegisterRepository : BaseRepository<TransfersRegister>, ITransfersRegisterRepository
    {
        public TransfersRegisterRepository(DbConnectionContext context) : base(context)
        {
        }
    }
}
