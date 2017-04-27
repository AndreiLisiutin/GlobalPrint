namespace GlobalPrint.Infrastructure.BankUtility
{
    /// <summary>
    /// Интерфейс утилиты, достающей информацию о банке по его БИК.
    /// </summary>
    public interface IBankUtility
    {
        /// <summary>
        /// Получить информацию о банке по его БИК.
        /// </summary>
        /// <param name="bicCode">БИК банка.</param>
        /// <returns>Информация о банке.</returns>
        IBankInfo GetBankInfo(string bicCode);
    }
}
