using GlobalPrint.ServerBusinessLogic.Models.Domain.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.Models.Business.Payments
{
    /// <summary>
    /// Full info model for payment action.
    /// </summary>
    public class PaymentActionFullInfo
    {
        public PaymentActionFullInfo()
        {

        }
        public PaymentActionFullInfo(PaymentAction paymentAction, PaymentTransaction paymentTransaction, 
            PaymentActionStatus paymentActionStatus, PaymentActionType paymentActionType)
        {
            this.PaymentAction = paymentAction;
            this.PaymentTransaction = paymentTransaction;
            this.PaymentActionStatus = paymentActionStatus;
            this.PaymentActionType = paymentActionType;
        }

        public PaymentAction PaymentAction { get; set; }
        public PaymentTransaction PaymentTransaction { get; set; }
        public PaymentActionStatus PaymentActionStatus { get; set; }
        public PaymentActionType PaymentActionType { get; set; }
    }
}
