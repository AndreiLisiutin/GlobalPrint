using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobalPrint.ClientWeb.Models.Lookup
{
    /// <summary>
    /// Factory of lookup managers.
    /// </summary>
    public interface ILookupManagerFactory
    {
        /// <summary>
        /// Create lookup manager of certain lookup type.
        /// </summary>
        /// <param name="lookupType">Lookup type.</param>
        /// <returns>ILookupManager object.</returns>
        ILookupManager CreateLookupManager(LookupTypeEnum lookupType);
    }
}