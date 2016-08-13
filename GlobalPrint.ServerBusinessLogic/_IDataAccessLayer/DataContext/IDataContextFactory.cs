using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext
{

    public interface IDataContextFactory
    {
        IDataContext CreateContext(string connectionString);
    }
}
