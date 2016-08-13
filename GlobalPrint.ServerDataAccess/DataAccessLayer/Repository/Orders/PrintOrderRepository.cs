using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Orders;
using GlobalPrint.ServerDataAccess.DataAccessLayer.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Orders;

namespace GlobalPrint.ServerDataAccess.DataAccessLayer.Repository.Orders
{
    public class PrintOrderRepository : BaseRepository<PrintOrder>, IPrintOrderRepository
    {
        public PrintOrderRepository(DbConnectionContext context) : base(context)
        {
        }
    }
}