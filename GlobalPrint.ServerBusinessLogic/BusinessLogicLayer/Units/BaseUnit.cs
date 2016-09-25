using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.DI;
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
    }
}
