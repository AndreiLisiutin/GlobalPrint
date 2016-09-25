using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.Models.Domain.Printers
{
    [Table("print_size_print_type", Schema = "public")]
    public class PrintSizePrintType : IDomainModel
    {
        [DebuggerStepThrough]
        public PrintSizePrintType()
        {
        }

        [Key]
        [Column("print_size_print_type_id")]
        public int PrintSizePrintTypeID { get; set; }
        [Column("print_size_id")]
        public int PrintSizeID { get; set; }
        [Column("print_type_id")]
        public int PrintTypeID { get; set; }

        #region IDomainModel
        [NotMapped]
        public int ID
        {
            get { return this.PrintSizePrintTypeID; }
            set { this.PrintSizePrintTypeID = value; }
        }
        #endregion
    }
}
