using System;

namespace GlobalPrint.Infrastructure.CommonUtils
{
    /// <summary>
    /// Класс для удобной обработки условий обязательности полей/параметров.
    /// </summary>
    public static class Argument
    {
        /// <summary>
        /// Объект должен быть не default(T) (не null).
        /// </summary>
        /// <typeparam name="T">Тип объекта.</typeparam>
        /// <param name="object">Проверяемый объект.</param>
        /// <param name="exception">Сообщение об ошибке.</param>
        public static void NotNull<T>(T @object, string exception)
        {
            if (@object == null || @object.Equals(default(T)))
            {
                throw new Exception(exception);
            }
        }

        /// <summary>
        /// Строка должна быть не пустой и не пробелом(ами).
        /// </summary>
        /// <param name="object">Проверяемая строка.</param>
        /// <param name="exception">Сообщение об ошибке.</param>
        public static void NotNullOrWhiteSpace(string @object, string exception)
        {
            if (string.IsNullOrWhiteSpace(@object?.Trim()))
            {
                throw new Exception(exception);
            }
        }

        /// <summary>
        /// Число должно быть положительным (и, само собой, не null).
        /// </summary>
        /// <param name="object">Проверяемое число.</param>
        /// <param name="exception">Сообщение об ошибке.</param>
        public static void Positive(decimal? @object, string exception)
        {
            if (@object == null || @object <= 0)
            {
                throw new Exception(exception);
            }
        }

        /// <summary>
        /// Число должно быть положительным (и, само собой, не null).
        /// </summary>
        /// <param name="object">Проверяемое число.</param>
        /// <param name="exception">Сообщение об ошибке.</param>
        public static void Positive(double? @object, string exception)
        {
            if (@object == null || @object <= 0)
            {
                throw new Exception(exception);
            }
        }

        /// <summary>
        /// Число должно быть положительным (и, само собой, не null).
        /// </summary>
        /// <param name="object">Проверяемое число.</param>
        /// <param name="exception">Сообщение об ошибке.</param>
        public static void Positive(long? @object, string exception)
        {
            if (@object == null || @object <= 0)
            {
                throw new Exception(exception);
            }
        }

        /// <summary>
        /// Должно выполняться условие.
        /// </summary>
        /// <param name="condition">Проверяемое условие.</param>
        /// <param name="exception">Сообщение об ошибке.</param>
        public static void Require(bool condition, string exception)
        {
            if (!condition)
            {
                throw new Exception(exception);
            }
        }
    }
}
