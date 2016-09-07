using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Printers
{
    [Table("print_type", Schema = "public")]
    public class PrintType : IDomainModel
    {
        [DebuggerStepThrough]
        public PrintType()
        {
        }
        
        [Column("name")]
        public string Name { get; set; }

        #region IDomainModel
        [Key]
        [Column("print_type_id")]
        public int ID { get; set; }
        #endregion
    }
}
