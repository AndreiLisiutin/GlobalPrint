using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.Models.Domain.TransfersRegisters
{
    [Table("cash_request_status", Schema = "public")]
    public class CashRequestStatus : IDomainModel
    {
        [DebuggerStepThrough]
        public CashRequestStatus()
        {
        }

        [Column("name")]
        public string Name { get; set; }

        #region IDomainModel
        [Key]
        [Column("cash_request_status_id")]
        public int ID { get; set; }
        #endregion
    }
}
