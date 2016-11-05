using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.BankUtility
{
    /// <summary>
    /// Bank info from web service.
    /// </summary>
    public interface IBankInfo
    {
        /// <summary>
        /// БИК.
        /// </summary>
        string Bic { get; set; }

        /// <summary>
        /// Корреспондентский счет.
        /// </summary>
        string CorrespondentAccount { get; set; }

        /// <summary>
        /// Полное наименование.
        /// </summary>
        string FullName { get; set; }

        /// <summary>
        /// Краткое наименование.
        /// </summary>
        string ShortName { get; set; }

        /// <summary>
        /// Регистрационный номер.
        /// </summary>
        short? RegNumber { get; set; }

        /// <summary>
        /// Телефон.
        /// </summary>
        string Phone { get; set; }
    }
}
