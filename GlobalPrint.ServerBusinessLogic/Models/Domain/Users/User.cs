using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.Models.Domain.Users
{
    [Table("user", Schema = "public")]
    public class User : IDomainModel, IUserProfile
    {
        [DebuggerStepThrough]
        public User()
        {
        }
        
        /// <summary>
        /// Имя пользователя.
        /// </summary>
        [Column("name")]
        public string UserName { get; set; }

        /// <summary>
        /// Email пользователя.
        /// </summary>
        [Column("email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// Объем денежных средств, доступных в системе пользователю.
        /// </summary>
        [Column("amount_of_money")]
        public decimal AmountOfMoney { get; set; }

        /// <summary>
        /// Телефон.
        /// </summary>
        [Column("phone")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Хэш пароля.
        /// </summary>
        [Column("password_hash")]
        public string PasswordHash { get; set; }

        /// <summary>
        /// Вспомогательное поле.
        /// </summary>
        [Column("security_stamp")]
        public string SecurityStamp { get; set; }

        /// <summary>
        /// Подтвержден ли телефон.
        /// </summary>
        [Column("phone_confirmed")]
        public bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// Подтвержден ли email.
        /// </summary>
        [Column("email_confirmed")]
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Последнее время активности.
        /// </summary>
        [Column("last_activity_date")]
        public DateTime LastActivityDate { get; set; }

        /// <summary>
        /// Идентификатор браузера пользователя.
        /// </summary>
        [Column("device_id")]
        [Obsolete("Используя FCM уже не актуален. Надо будет безопасно его убрать.")]
        public string DeviceID { get; set; }

        /// <summary>
        /// Дата создания.
        /// </summary>
        [Column("created_on")]
        public DateTime CreatedOn { get; set; }
        
        #region ILegalRequisites

        /// <summary>
        /// Бик банка.
        /// </summary>
        [Column("bank_bic")]
        public string BankBic { get; set; }

        /// <summary>
        /// Полное наименование.
        /// </summary>
        [Column("legal_full_name")]
        public string LegalFullName { get; set; }

        /// <summary>
        /// Краткое наименование.
        /// </summary>
        [Column("legal_short_name")]
        public string LegalShortName { get; set; }

        /// <summary>
        /// Юридический адрес.
        /// </summary>
        [Column("legal_address")]
        public string LegalAddress { get; set; }

        /// <summary>
        /// Почтовый адрес.
        /// </summary>
        [Column("post_address")]
        public string PostAddress { get; set; }

        /// <summary>
        /// ИНН.
        /// </summary>
        [Column("inn")]
        public string Inn { get; set; }

        /// <summary>
        /// КПП.
        /// </summary>
        [Column("kpp")]
        public string Kpp { get; set; }

        /// <summary>
        /// ОГРН.
        /// </summary>
        [Column("ogrn")]
        public string Ogrn { get; set; }

        /// <summary>
        /// Расчетный счет.
        /// </summary>
        [Column("payment_account")]
        public string PaymentAccount { get; set; }

        /// <summary>
        /// Наименование банка.
        /// </summary>
        [Column("bank_name")]
        public string BankName { get; set; }

        /// <summary>
        /// Корреспондентский счет банка.
        /// </summary>
        [Column("bank_correspondent_account")]
        public string BankCorrespondentAccount { get; set; }

        #endregion

        #region IDomainModel
        
        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        [Key, Column("user_id")]
        public int ID { get; set; }

        #endregion
    }
}
