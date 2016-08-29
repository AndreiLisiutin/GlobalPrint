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
    [Table("print_size", Schema = "public")]
    public class PrintSize : IDomainModel
    {
        [DebuggerStepThrough]
        public PrintSize()
        {
        }

        [Key]
        [Column("print_size_id")]
        public int PrintSizeID { get; set; }
        [Column("name")]
        public string Name { get; set; }

        #region IDomainModel
        [NotMapped]
        public int ID
        {
            get { return this.PrintSizeID; }
            set { this.PrintSizeID = value; }
        }
        #endregion
    }
}
