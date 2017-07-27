using System.Linq;

namespace GlobalPrint.Infrastructure.CommonUtils.ExtensionMethods
{
    /// <summary>
    /// Методы расширения для Enumerable.
    /// </summary>
    public static class EnumerableExtensionMethods
    {
        /// <summary>
        /// Содержится ли элемент в коллекции.
        /// </summary>
        /// <typeparam name="T">Тип элемента.</typeparam>
        /// <param name="item">Сам элемент, наличие которого в коллекции проверяется.</param>
        /// <param name="collection">Коллекция для проверки.</param>
        /// <returns>Содержится ли элемент в коллекции.</returns>
        public static bool In<T>(this T item, params T[] collection)
        {
            if (!(collection?.Any() ?? false))
            {
                return false;
            }

            return collection.Contains(item);
        }
    }
}
