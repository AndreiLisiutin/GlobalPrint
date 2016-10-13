using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.DI;
using GlobalPrint.ServerBusinessLogic.Models.Business.Users;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units
{
    /// <summary> Base business logic layer unit.
    /// </summary>
    public class BaseUnit
    {
        /// <summary> Connection string to the data source.
        /// </summary>
        private string _connectionString;

        /// <summary> Factory for data contexts.
        /// </summary>
        private IDataContextFactory _contextFactory;

        [DebuggerStepThrough]
        public BaseUnit()
        {
            this._contextFactory = IoC.Instance.Resolve<IDataContextFactory>();
            this._connectionString = ConfigurationManager.ConnectionStrings["GlobalPrint"].ConnectionString;
        }

        /// <summary> Create a data connection context. Opens the connection to DB.
        /// </summary>
        /// <returns>Created data context.</returns>
        protected IDataContext Context()
        {
            return _contextFactory.CreateContext(this._connectionString);
        }

        /// <summary> Create a typed repository by its interface.
        /// </summary>
        /// <typeparam name="T">Repository interface.</typeparam>
        /// <param name="context">Data connection context.</param>
        /// <returns>Created repository.</returns>
        protected T Repository<T>(IDataContext context)
        {
            Type repositoryType = typeof(T);
            //string parameterName = repositoryType
            //    .GetConstructors(BindingFlags.Public)
            //    .Where(e =>
            //    {
            //        var parameters = e.GetParameters();
            //        return parameters.Count() == 1 && 
            //            parameters.First().ParameterType.IsAssignableFrom(typeof(IDataContext));
            //    })
            //    .Select(e => e.GetParameters().First().Name)
            //    .SingleOrDefault();

            //if (string.IsNullOrEmpty(parameterName))
            //{
            //    throw new InvalidOperationException(string.Format(
            //        "Service {0} doesn't support constructor with 1 parameter of kind IRepository(IDataContext context).",
            //            repositoryType.Name
            //    ));
            //}

            try
            {
                return IoC.Instance.Resolve<T>(IoC.Argument("context", context));
            }
            catch (Exception)
            {
                throw new InvalidOperationException(string.Format(
                    "Service {0} doesn't support constructor with 1 parameter of kind \"IRepository(IDataContext context)\".",
                        repositoryType.Name
                ));
            }
        }

        /// <summary>
        /// Log into database user's action. Better to place into separate class and use IoC.
        /// </summary>
        /// <param name="userActionType">Type of action.</param>
        /// <param name="logMessage">Text log message.</param>
        /// <param name="userID">Identifier of the acting user.</param>
        /// <param name="dataContext">Data context.</param>
        protected void Log(UserActionTypeEnum userActionType, string logMessage, int userID, IDataContext dataContext)
        {
            IUserActionLogRepository repo = this.Repository<IUserActionLogRepository>(dataContext);
            UserActionLog log = new UserActionLog()
            {
                ID = 0,
                Date = DateTime.Now,
                Log = logMessage,
                UserID = userID,
                UserActionTypeID = (int)userActionType
            };
            repo.Insert(log);
        }
    }
}
