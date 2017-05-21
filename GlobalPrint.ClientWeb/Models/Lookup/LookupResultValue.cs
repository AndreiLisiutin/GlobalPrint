using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobalPrint.ClientWeb.Models.Lookup
{
    /// <summary>
    /// Value of the lookup entity property.
    /// </summary>
    public class LookupResultValue
    {
        public LookupResultValue()
            : this("Column name", "Value", 1, "Unknown", false, false)
        {
        }
        public LookupResultValue(string name, string value, int length, string identifier, bool isIdentifier = false, bool isText = false)
        {
            this.Name = name;
            this.Value = value;
            this.Length = length;
            this.IsIdentifier = isIdentifier;
            this.Identifier = identifier;
            this.IsText = isText;
        }

        public string Identifier { get; set; }

        public SortByEnum? SortOrder { get; set; }

        /// <summary>
        /// Property caption.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Property value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Property length in terms of Bootstrap col-**-{Length}.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// True if this property is identifier of the entity.
        /// </summary>
        public bool IsIdentifier { get; set; }

        /// <summary>
        /// True if this property is text property, display value of the entity.
        /// </summary>
        public bool IsText { get; set; }
    }
}