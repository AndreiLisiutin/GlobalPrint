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
    [Table("printer", Schema = "public")]
    public class Printer : IDomainModel
    {
        [DebuggerStepThrough]
        public Printer()
        {
        }

        [NotMapped]
        private int PrinterID { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("location")]
        public string Location { get; set; }
        [Column("user_id_owner")]
        public int OwnerUserID { get; set; }
        [Column("latitude")]
        public float Latitude { get; set; }
        [Column("longtitude")]
        public float Longtitude { get; set; }
        [Column("phone")]
        public string Phone { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("user_id_operator")]
        public int OperatorUserID { get; set; }
        [Column("is_disabled")]
        public bool IsDisabled { get; set; }

        #region IDomainModel
        [Key]
        [Column("printer_id")]
        public int ID
        {
            get { return this.PrinterID; }
            set { this.PrinterID = value; }
        }
        #endregion
    }
}