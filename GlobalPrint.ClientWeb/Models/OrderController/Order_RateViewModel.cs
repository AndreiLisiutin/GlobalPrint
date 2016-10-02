using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobalPrint.ClientWeb.Models.OrderController
{
    /// <summary>
    /// View model for rating the order.
    /// </summary>
    public class Order_RateViewModel
    {
        public Order_RateViewModel()
        {

        }
        public int PrintOrderID { get; set; }
        public float? Rating { get; set; }
        public string Comment { get; set; }
    }
}