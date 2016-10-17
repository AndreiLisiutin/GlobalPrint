using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.CommonUtils
{
    /// <summary>
    /// Validation class.
    /// </summary>
    public class Validation
    {
        List<string> _errors;
        public Validation()
        {
            this._errors = new List<string>();
        }

        public void NotNull<T>(T @object, string exception)
        {
            if (@object == null || @object.Equals(default(T)))
            {
                this._errors.Add(exception);
            }
        }
        public void NotNullOrWhiteSpace(string @object, string exception)
        {
            if (string.IsNullOrWhiteSpace(@object))
            {
                this._errors.Add(exception);
            }
        }
        public void Positive(decimal? @object, string exception)
        {
            if (@object == null || @object <= 0)
            {
                this._errors.Add(exception);
            }
        }
        public void Positive(double? @object, string exception)
        {
            if (@object == null || @object <= 0)
            {
                this._errors.Add(exception);
            }
        }
        public void Positive(long? @object, string exception)
        {
            if (@object == null || @object <= 0)
            {
                this._errors.Add(exception);
            }
        }
        public void Require(bool condition, string exception)
        {
            if (!condition)
            {
                this._errors.Add(exception);
            }
        }

        /// <summary>
        /// Is validation process successful.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return this._errors == null || this._errors.Count == 0;
            }
        }

        /// <summary>
        /// Validation errors.
        /// </summary>
        public List<string> Errors
        {
            get
            {
                return this._errors;
            }
        }

        /// <summary>
        /// Merge validation results and create united validation result.
        /// </summary>
        /// <param name="validation">Merged validation result.</param>
        public void Merge(Validation validation)
        {
            this._errors = this.Errors ?? new List<string>();
            this._errors.AddRange(validation.Errors);
        }

        /// <summary>
        /// Throw exception if validation wasn't passed.
        /// </summary>
        public void ThrowExceptionIfNotValid()
        {
            if (!this.IsValid)
            {
                throw new Exception(string.Join("; " + Environment.NewLine, _errors));
            }
        }
    }
}
