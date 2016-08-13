using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobalPrint.Infrastructure.LogUtility
{
    public interface ILogFactory
    {
        /// <summary>
        /// Creates a logger with the Callsite of the given Type T
        /// </summary>
        /// <typeparam name="T">Callsite type</typeparam>
        /// <returns></returns>
        ILogUtility GetLogUtility<T>();

        /// <summary>
        /// Creates a logger with the Callsite of current type
        /// </summary>
        /// <returns></returns>
        ILogUtility GetCurrentClassLogUtility();
    }
}