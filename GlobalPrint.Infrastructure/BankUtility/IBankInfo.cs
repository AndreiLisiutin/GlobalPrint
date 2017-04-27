namespace GlobalPrint.Infrastructure.BankUtility
{
    /// <summary>
    /// Данные о банке, полученные по БИК банка.
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
        string RegNumber { get; set; }

        /// <summary>
        /// Телефон.
        /// </summary>
        string Phone { get; set; }
    }
}
