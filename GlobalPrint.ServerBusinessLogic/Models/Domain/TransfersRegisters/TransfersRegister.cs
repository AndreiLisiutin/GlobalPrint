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
    [Table("transfers_register", Schema = "public")]
    public class TransfersRegister : IDomainModel
    {
        [DebuggerStepThrough]
        public TransfersRegister()
        {
        }

        [Column("created_on")]
        public DateTime CreatedOn { get; set; }

        [Column("user_id")]
        public int UserID { get; set; }

        [Column("cash_request_total_count")]
        public int CashRequestsTotalCount { get; set; }

        [Column("cash_request_total_amount_of_money")]
        public decimal CashRequestsTotalAmountOfMoney { get; set; }

        #region IDomainModel
        [Key]
        [Column("transfers_register_id")]
        public int ID { get; set; }
        #endregion
    }
}
