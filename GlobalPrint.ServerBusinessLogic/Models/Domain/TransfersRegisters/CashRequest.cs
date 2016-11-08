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
    [Table("cash_request", Schema = "public")]
    public class CashRequest : IDomainModel
    {
        [DebuggerStepThrough]
        public CashRequest()
        {
        }

        [Column("created_on")]
        public DateTime CreatedOn { get; set; }

        [Column("amount_of_money")]
        public decimal AmountOfMoney { get; set; }

        [Column("user_id")]
        public int UserID { get; set; }

        [Column("transfers_register_id")]
        public int? TransfersRegisterID { get; set; }

        [Column("cash_request_status_id")]
        public int CashRequestStatusID { get; set; }

        #region IDomainModel
        [Key]
        [Column("cash_request_id")]
        public int ID { get; set; }
        #endregion
    }
}
