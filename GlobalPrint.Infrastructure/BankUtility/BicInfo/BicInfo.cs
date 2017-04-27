using System;

namespace GlobalPrint.Infrastructure.BankUtility.BicInfo
{
    /// <summary>
    /// Данные о банке из сервиса bik-info.
    /// </summary>
    /// <remarks>
    /// Сервис: http://www.bik-info.ru.
    /// </remarks>
    public class BicInfo : IBankInfo
    {
        /// <summary>
        /// БИК.
        /// </summary>
        public string Bic { get; set; }

        /// <summary>
        /// Регистрационный номер.
        /// </summary>
        public string RegNumber { get; set; }

        /// <summary>
        /// Корреспондентский счет.
        /// </summary>
        public string CorrespondentAccount { get; set; }

        /// <summary>
        /// Полное наименование.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Краткое наименование.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Телефон.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Индекс.
        /// </summary>
        public string Index { get; set; }

        /// <summary>
        /// Населенный пункт.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Адрес.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Код по ОКАТО.
        /// </summary>
        public string Okato { get; set; }

        /// <summary>
        /// Код ОКПО.
        /// </summary>
        public string Okpo { get; set; }

        /// <summary>
        /// Срок прохождения документов (дней).
        /// </summary>
        public string DocumentsPeriod { get; set; }

        /// <summary>
        /// Дата добавление информации.
        /// </summary>
        public DateTime? DateAdd { get; set; }

        /// <summary>
        /// Дата последнего изменения информации.
        /// </summary>
        public DateTime? DateChange { get; set; }        
    }
}
