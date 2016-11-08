using GlobalPrint.ServerBusinessLogic.Models.Domain.Orders;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Payment;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Printers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.TransfersRegisters;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using System.Data.Common;
using System.Data.Entity;
using System.Diagnostics;

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

        // Models/Domain/Orders
        public DbSet<PrintOrder> PrintOrders { get; set; }
        public DbSet<PrintOrderStatus> PrintOrderStatuses { get; set; }
        // Models/Domain/Printers
        public DbSet<Printer> Printers { get; set; }
        public DbSet<PrinterSchedule> PrintSchedules { get; set; }
        public DbSet<PrinterService> PrinterServices { get; set; }
        public DbSet<PrintService> PrintServices { get; set; }
        public DbSet<PrintSize> PrintSizes { get; set; }
        public DbSet<PrintType> PrintTypes { get; set; }
        public DbSet<PrintSizePrintType> PrintSizePrintTypes { get; set; }
        // Models/Domain/Users
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserActionLog> UserActionLogs { get; set; }
        public DbSet<UserActionType> UserActionTypes { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        // Models/Domain/Offers
        //public DbSet<Offer> Offers { get; set; }
        //public DbSet<UserOffer> UserOffers { get; set; }
        //public DbSet<OfferType> OfferTypes { get; set; }
        // Models/Domain/Payment
        public DbSet<PaymentAction> PaymentActions { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<PaymentTransactionStatus> PaymentTransactionStatuses { get; set; }
        public DbSet<PaymentActionStatus> PaymentActionStatuses { get; set; }
        public DbSet<PaymentActionType> PaymentActionTypes { get; set; }

        public DbSet<CashRequest> CashRequests { get; set; }
        public DbSet<CashRequestStatus> CashRequestStatuses { get; set; }
        public DbSet<TransfersRegister> TransfersRegisters { get; set; }
    }
}
