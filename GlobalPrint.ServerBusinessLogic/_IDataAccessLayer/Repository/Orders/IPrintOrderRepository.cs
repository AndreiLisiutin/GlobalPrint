using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Orders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Orders
{
    public interface IPrintOrderRepository : IRepository<PrintOrder>
    {
    }
}