using GlobalPrint.ClientWeb.Models.Lookup.LookupManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobalPrint.ClientWeb.Models.Lookup
{
    /// <summary>
    /// Base lookup manager factory.
    /// </summary>
    public class LookupManagerFactory : ILookupManagerFactory
    {
        /// <summary>
        /// Creates lookup manager by its type.
        /// </summary>
        /// <param name="lookupType">Type of lookup.</param>
        /// <returns>Lookup manager according to the type.</returns>
        public ILookupManager CreateLookupManager(LookupTypeEnum lookupType)
        {
            switch (lookupType)
            {
                case LookupTypeEnum.User:
                    return new UserLookupManager();
                default:
                    throw new InvalidOperationException("Unknown type of lookup manager.");
            }
        }
    }
}