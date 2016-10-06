using GlobalPrint.ServerBusinessLogic.Models.Business.Printers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Printers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GlobalPrint.ClientWeb.Models.PrinterController
{
    public class Printer_EditViewMoel
    {
        public Printer Printer { get; set; }
        public List<_Schedule> Schedule { get; set; }
        public List<_Service> Services { get; set; }
        //public bool NeedPrinterOwnerOffer { get; set; }
        
        //[Required(ErrorMessage = "Вы должны подтвердить свое согласие с условиями оферты.")]
        //[Range(typeof(bool), "true", "true", ErrorMessage = "Вы должны подтвердить свое согласие с условиями оферты.")]
        //public bool IsAgreeWithOffer { get; set; }

        public class _Schedule
        {
            public _Schedule()
            {

            }
            public _Schedule(int DayOfWeek, string DayOfWeekName, bool isOpened, TimeSpan? OpenTime, TimeSpan? CloseTime, int PrinterScheduleID)
            {
                this.DayOfWeek = DayOfWeek;
                this.DayOfWeekName = DayOfWeekName;
                this.isOpened = isOpened;
                this.OpenTime = OpenTime;
                this.CloseTime = CloseTime;
                this.PrinterScheduleID = PrinterScheduleID;
            }

            public int PrinterScheduleID { get; set; }
            public int DayOfWeek { get; set; }
            public string DayOfWeekName { get; set; }
            public bool isOpened { get; set; }
            public TimeSpan? OpenTime { get; set; }
            public TimeSpan? CloseTime { get; set; }
        }
        public class _Service
        {
            public _Service()
            {

            }
            public _Service(PrintServiceExtended PrintService, bool Supported, decimal? Price, int PrinterServiceID)
            {
                this.PrintServiceID = PrintService.PrintService.ID;
                this.PrintSize = PrintService.PrintSize.Name;
                this.PrintType = PrintService.PrintType.Name;
                this.IsColored = PrintService.PrintService.IsColored;
                this.IsTwoSided = PrintService.PrintService.IsTwoSided;


                this.IsSupported = Supported;
                this.Price = Price;
                this.PrinterServiceID = PrinterServiceID;
            }
            public int PrinterServiceID { get; set; }

            public int PrintServiceID { get; set; }
            public string PrintSize { get; set; }
            public string PrintType { get; set; }
            public bool IsColored { get; set; }
            public bool IsTwoSided { get; set; }

            public bool IsSupported { get; set; }
            public decimal? Price { get; set; }
        }
    }
}