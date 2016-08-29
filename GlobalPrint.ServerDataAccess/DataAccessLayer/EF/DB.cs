using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Orders;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Printers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Users;
using System;
using System.Data.Common;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerDataAccess.EF
{
    public class DB : DbContext
    {
        [DebuggerStepThrough]
        static DB()
        {
            Database.SetInitializer<DB>(new NullDatabaseInitializer<DB>());
        }

        [DebuggerStepThrough]
        public DB() : base("GlobalPrint")
        {
        }
        [DebuggerStepThrough]
        public DB(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }

        //orders
        public DbSet<PrintOrder> PrintOrders { get; set; }
        public DbSet<PrintOrderStatus> PrintOrderStatuses { get; set; }
        //printers
        public DbSet<Printer> Printers { get; set; }
        public DbSet<PrinterSchedule> PrintSchedules { get; set; }
        public DbSet<PrinterService> PrinterServices { get; set; }
        public DbSet<PrintService> PrintServices { get; set; }
        public DbSet<PrintSize> PrintSizes { get; set; }
        public DbSet<PrintType> PrintTypes { get; set; }
        public DbSet<PrintSizePrintType> PrintSizePrintTypes { get; set; }
        //users
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserActionLog> UserActionLogs { get; set; }
        public DbSet<UserActionType> UserActionTypes { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

    }
}
