using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext
{
    public interface IDataContext : IDisposable
    {
        bool IsTransactionAlive();
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        int Save();
    }
}
