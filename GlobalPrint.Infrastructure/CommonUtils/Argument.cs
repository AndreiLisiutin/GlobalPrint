using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.CommonUtils
{
    public static class Argument
    {
        public static void NotNull<T>(T @object, string exception)
        {
            if (@object == null || @object.Equals(default(T)))
            {
                throw new ArgumentException(exception);
            }
        }
        public static void NotNullOrWhiteSpace(string @object, string exception)
        {
            if (string.IsNullOrWhiteSpace(@object))
            {
                throw new ArgumentNullException(exception);
            }
        }
        public static void Positive(decimal? @object, string exception)
        {
            if (@object == null || @object <= 0)
            {
                throw new ArgumentException(exception);
            }
        }
        public static void Positive(double? @object, string exception)
        {
            if (@object == null || @object <= 0)
            {
                throw new ArgumentException(exception);
            }
        }
        public static void Positive(long? @object, string exception)
        {
            if (@object == null || @object <= 0)
            {
                throw new ArgumentException(exception);
            }
        }
        public static void Require(bool condition, string exception)
        {
            if (!condition)
            {
                throw new ArgumentException(exception);
            }
        }
    }
}
