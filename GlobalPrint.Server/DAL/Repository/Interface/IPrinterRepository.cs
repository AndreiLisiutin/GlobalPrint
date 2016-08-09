using GlobalPrint.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Server.DAL
{
    /// <summary>
    /// Интерфейс репозитория принтеров
    /// </summary>
    public interface IPrinterRepository : IRepository<Printer>
    {
    }
}
