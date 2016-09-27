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
    [Table("print_service", Schema = "public")]
    public class PrintService : IDomainModel
    {
        [DebuggerStepThrough]
        public PrintService()
        {
        }

        [NotMapped]
        private int PrintServiceID { get; set; }
        [Column("print_size_print_type_id")]
        public int PrintSizePrintTypeID { get; set; }
        [Column("is_colored")]
        public bool IsColored { get; set; }
        [Column("is_two_sided")]
        public bool IsTwoSided { get; set; }

        #region IDomainModel
        [Key]
        [Column("print_service_id")]
        public int ID
        {
            get { return this.PrintServiceID; }
            set { this.PrintServiceID = value; }
        }
        #endregion
    }
}
