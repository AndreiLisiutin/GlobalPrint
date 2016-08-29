using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext
{
    public class DbConnectionContextFactory : IDataContextFactory
    {
        [DebuggerStepThrough]
        public DbConnectionContextFactory()
        {

        }
        public IDataContext CreateContext(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ConfigurationErrorsException("Connection string not found! Check configuration file!");
            }
            DbConnection connection = new Npgsql.NpgsqlConnectionFactory().CreateConnection(connectionString);
            return new DbConnectionContext(connection);
        }
    }
}
