using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.Models.Domain.Users
{
    /// <summary>
    /// Requisites of legal persons/units/etc.
    /// </summary>
    public interface ILegalRequisites
    {
        /// <summary>
        /// Бик банка.
        /// </summary>
        string BankBic { get; set; }

        /// <summary>
        /// Полное наименование.
        /// </summary>
        string LegalFullName { get; set; }

        /// <summary>
        /// Краткое наименование.
        /// </summary>
        string LegalShortName { get; set; }

        /// <summary>
        /// Юридический адрес.
        /// </summary>
        string LegalAddress { get; set; }

        /// <summary>
        /// Почтовый адрес.
        /// </summary>
        string PostAddress { get; set; }

        /// <summary>
        /// ИНН.
        /// </summary>
        string Inn { get; set; }

        /// <summary>
        /// КПП.
        /// </summary>
        string Kpp { get; set; }

        /// <summary>
        /// ОГРН.
        /// </summary>
        string Ogrn { get; set; }

        /// <summary>
        /// Расчетный счет.
        /// </summary>
        string PaymentAccount { get; set; }

        /// <summary>
        /// Наименование банка.
        /// </summary>
        string BankName { get; set; }

        /// <summary>
        /// Корреспондентский счет банка.
        /// </summary>
        string BankCorrespondentAccount { get; set; }
    }
}
