using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Printers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Printers
{
    /// <summary> Model for the Printer entity. 
    /// Contains schedules and services with alll the text values of any ID and internal entity.
    /// </summary>
    public class PrinterFullInfoModel
    {
        [DebuggerStepThrough]
        public PrinterFullInfoModel()
        {
        }
        [DebuggerStepThrough]
        public PrinterFullInfoModel(Printer printer, IEnumerable<PrinterSchedule> schedule, IEnumerable<PrinterServiceExtended> services)
        {
            this.Printer = printer;
            this.PrinterSchedule = schedule;
            this.PrinterServices = services;
        }

        public Printer Printer { get; set; }
        public IEnumerable<PrinterSchedule> PrinterSchedule { get; set; }
        public IEnumerable<PrinterServiceExtended> PrinterServices { get; set; }

        public bool IsAvailableNow
        {
            get
            {
                DateTime now = DateTime.Now;
                if (this.Printer.IsDisabled)
                {
                    //printer was marked as disabled by its owner.
                    return false;
                }

                bool isWorkingNow = this.PrinterSchedule.Any(s => 
                    s.DayOfWeek == (int)now.DayOfWeek 
                    && s.OpenTime <= now.TimeOfDay 
                    && s.CloseTime >= now.TimeOfDay
                );

                return isWorkingNow;
            }
        }
    }
}
