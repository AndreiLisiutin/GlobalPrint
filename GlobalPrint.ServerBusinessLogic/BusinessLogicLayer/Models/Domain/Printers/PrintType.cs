using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Printers
{
    [Table("print_type", Schema = "public")]
    public class PrintType : IDomainModel
    {
        [Key]
        [Column("print_type_id")]
        public int PrintTypeID { get; set; }
        [Column("name")]
        public string Name { get; set; }

        #region IDomainModel
        [NotMapped]
        public int ID
        {
            get { return this.PrintTypeID; }
            set { this.PrintTypeID = value; }
        }
        #endregion
    }
}
