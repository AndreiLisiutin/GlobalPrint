using GlobalPrint.ServerBusinessLogic.Models.Domain.Printers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace GlobalPrint.ServerBusinessLogic.Models.Business.Printers
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
        public PrinterFullInfoModel(Printer printer, User @operator, IEnumerable<PrinterSchedule> schedule, IEnumerable<PrinterServiceExtended> services)
        {
            this.Printer = printer;
            this.PrinterSchedule = schedule;
            this.PrinterServices = services;
            this.Operator = @operator;
        }

        public Printer Printer { get; set; }
        public User Operator { get; set; }
        public IEnumerable<PrinterSchedule> PrinterSchedule { get; set; }
        public IEnumerable<PrinterServiceExtended> PrinterServices { get; set; }

        public bool IsOperatorAlive
        {
            get
            {
                TimeSpan threshold = TimeSpan.FromMinutes(double.Parse(WebConfigurationManager.AppSettings["ActivityCheckerThreshold"]));
                return this.Operator.LastActivityDate > DateTime.Now.Subtract(threshold);
            } 
        }

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
