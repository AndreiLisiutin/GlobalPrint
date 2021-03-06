﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.FileUtility.FileExporters.Exportable
{

    /// <summary>
    /// Property of the entity to export into certain format.
    /// </summary>
    public class ExportableProperty
    {
        public ExportableProperty(string value, string name, int? length = null, ExportFormatting? format = null)
        {
            this.Value = value;
            this.PropertyName = name;
            this.Length = length ?? value?.Length;
            this.Format = format ?? ExportFormatting.Text;
        }
        /// <summary>
        /// Property value.
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Property name.
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// Property value length.
        /// </summary>
        public int? Length { get; set; }
        /// <summary>
        /// Property value length.
        /// </summary>
        public ExportFormatting? Format { get; set; }
    }

    public enum ExportFormatting
    {
        Text = 1,
        Number = 2,
        Money = 3,
        Date = 4
    }
}
