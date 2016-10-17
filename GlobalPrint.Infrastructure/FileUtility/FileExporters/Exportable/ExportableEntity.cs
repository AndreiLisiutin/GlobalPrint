using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.FileUtility.FileExporters.Exportable
{

    /// <summary>
    /// Entity as set of its properties to export into certain format.
    /// </summary>
    public class ExportableEntity
    {
        public ExportableEntity(IEnumerable<ExportableProperty> properties)
        {
            this.Properties = properties ?? new List<ExportableProperty>();
        }

        /// <summary>
        /// List of entity properties.
        /// </summary>
        public IEnumerable<ExportableProperty> Properties { get; set; }
    }
}
