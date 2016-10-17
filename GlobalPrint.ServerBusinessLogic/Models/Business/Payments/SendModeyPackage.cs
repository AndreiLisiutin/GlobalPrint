using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.Models.Business.Payments
{
    /// <summary>
    /// Package for sending money between accounts.
    /// </summary>
    public class SendModeyPackage
    {
        /// <summary>
        /// Sender user identifier.
        /// </summary>
        public int SenderUserId { get; set; }
        /// <summary>
        /// Receiver user identifier.
        /// </summary>
        public int ReceiverUserId { get; set; }
        /// <summary>
        /// Amount of money to send.
        /// </summary>
        public decimal AmountOfMoney { get; set; }
    }
}
