using System;

namespace GlobalPrint.Infrastructure.CommonUtils.ExtensionMethods
{
	/// <summary>
	/// Методы расширения для класса string.
	/// </summary>
	public static class StringExtensionMethods
	{
		/// <summary>
		/// Сравнить строки без учета регистра.
		/// </summary>
		/// <param name="originalString">Исходная строка, для которой вызывается метод.</param>
		/// <param name="stringToCompare">Строка для сравнения.</param>
		/// <returns>Равны ли строки с точностью до регистра.</returns>
		public static bool EqualsIgnoreCase(this string originalString, string stringToCompare)
		{
			return originalString.Equals(stringToCompare, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
