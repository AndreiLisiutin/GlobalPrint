using GlobalPrint.Server.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Server
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

        public DbSet<User> Users { get; set; }
        public DbSet<Printer> Printers { get; set; }
        public DbSet<PrintOrder> PrintOrders { get; set; }
        public DbSet<PrinterSchedule> PrintSchedules { get; set; }
        public DbSet<PrintOrderStatus> PrintOrderStatuses { get; set; }
    }
}
