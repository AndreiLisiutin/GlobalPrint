using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerDataAccess.EF;
using System;
using System.Data;
using System.Data.Common;

namespace GlobalPrint.ServerDataAccess.DataAccessLayer.DataContext
{
    public class DbConnectionContext : IDataContext
    {
        private DbConnection _connection;
        private IDbTransaction _transaction;
        private Lazy<DB> _db;
        private bool _isDisposed;
        public DbConnectionContext(DbConnection connection)
        {
            this._connection = connection;
            this._isDisposed = false;
            this._db = new Lazy<DB>(() => new DB(connection, false));
        }

        public DB DB
        {
            get
            {
                return this._db.Value;
            }
        }

        public void BeginTransaction()
        {
            if (this.IsTransactionAlive())
            {
                throw new InvalidOperationException("Another transaction is active.");
            }
            this._transaction = this._connection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (!this.IsTransactionAlive())
            {
                throw new InvalidOperationException("Transaction is not active.");
            }
            this._transaction.Commit();
            this._transaction.Dispose();
            this._transaction = null;
        }

        public void Dispose()
        {
            if (this._isDisposed || this._connection == null)
            {
                return;
            }
            if (this.IsTransactionAlive())
            {
                this.RollbackTransaction();
            }
            if (this._db != null && this._db.IsValueCreated)
            {
                this._db.Value.Dispose();
                this._db = null;
            }
            if (this._connection.State != ConnectionState.Closed)
            {
                this._connection.Close();
            }
            this._connection.Dispose();
            this._connection = null;
            this._isDisposed = true;
        }

        public void RollbackTransaction()
        {
            if (!this.IsTransactionAlive())
            {
                throw new InvalidOperationException("Transaction is not active.");
            }
            this._transaction.Rollback();
            this._transaction.Dispose();
            this._transaction = null;
        }

        public bool IsTransactionAlive()
        {
            return this._transaction != null;
        }

        /// <summary>
        /// Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>
        /// The number of state entries written to the underlying database. This can include
        /// state entries for entities and/or relationships. Relationship state entries are
        /// created for many-to-many relationships and relationships where there is no foreign
        /// key property included in the entity class (often referred to as independent associations).
        /// </returns>
        public int Save()
        {
            if (this._db != null)
            {
                return this._db.Value.SaveChanges();
            }
            return 0;
        }
    }
}
