using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.DI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units
{
    public class BaseUnit
    {
        string _connectionString;
        IDataContextFactory _contextFactory;
        public BaseUnit()
        {
            this._contextFactory = IoC.Instance.Resolve<IDataContextFactory>();
            this._connectionString = ConfigurationManager.ConnectionStrings["GlobalPrint"].ConnectionString;
        }

        protected IDataContext Context()
        {
            return _contextFactory.CreateContext(this._connectionString);
        }

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
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format(
                    "Service {0} doesn't support constructor with 1 parameter of kind \"IRepository(IDataContext context)\".",
                        repositoryType.Name
                ));
            }
        }

        private void _Test()
        {
            using (IDataContext context = this.Context())
            {
                var userActionLogRepo = this.Repository<IUserActionLogRepository>(context);
            }
        }

    }
}
