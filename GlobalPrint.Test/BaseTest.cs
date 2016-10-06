using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Test
{
    /// <summary>
    /// Parent for all test classes
    /// </summary>
    public abstract class BaseTest
    {
        /// <summary>
        /// User name for identity config.
        /// </summary>
        protected string CurrentUserName = ConfigurationManager.AppSettings["TestUserName"];

        /// <summary>
        /// User ID for identity config.
        /// </summary>
        protected int CurrentUserID = Int32.Parse(ConfigurationManager.AppSettings["TestUserID"]);
    }
}
