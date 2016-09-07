using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Orders;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Printers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Orders;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Printers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb
{
    public class Order_NewViewModel : Order_NewPostModel
    {
        public Order_NewViewModel()
        {
        }
        public Order_NewViewModel(NewOrder newOrder, Printer printer)
            : base(newOrder)
        {
            this.Printer = printer;
        }

        public Printer Printer { get; set; }
    }

    public class Order_NewPostModel
    {
        public Order_NewPostModel()
        {
            this.Order = new NewOrder();
        }
        public Order_NewPostModel(NewOrder newOrder)
        {
            this.Order = newOrder;
        }
        public NewOrder Order { get; set; }
    }
}