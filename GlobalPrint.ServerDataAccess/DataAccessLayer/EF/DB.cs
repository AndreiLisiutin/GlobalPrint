using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Orders;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Printers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Users;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerDataAccess.EF
{

    public class DB : DbContext
    {
        static DB()
        {
            Database.SetInitializer<DB>(new NullDatabaseInitializer<DB>());
        }

        public DB() : base("GlobalPrint")
        {

        }
        public DB(DbConnection existingConnection, bool contextOwnsConnection) 
            : base(existingConnection, contextOwnsConnection)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Printer> Printers { get; set; }
        public DbSet<PrintOrder> PrintOrders { get; set; }
        public DbSet<PrinterSchedule> PrintSchedules { get; set; }
        public DbSet<PrintOrderStatus> PrintOrderStatuses { get; set; }
    }
}
