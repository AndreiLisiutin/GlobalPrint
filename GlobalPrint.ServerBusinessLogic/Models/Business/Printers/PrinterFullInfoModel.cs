using GlobalPrint.ServerBusinessLogic.Models.Domain.Printers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Configuration;

namespace GlobalPrint.ServerBusinessLogic.Models.Business.Printers
{
    /// <summary> 
    /// Model for the Printer entity. 
    /// Contains schedules and services with alll the text values of any ID and internal entity.
    /// </summary>
    public class PrinterFullInfoModel
    {
        [DebuggerStepThrough]
        public PrinterFullInfoModel()
        {
        }
        [DebuggerStepThrough]
        public PrinterFullInfoModel(Printer printer, User @operator, User printerOwner, IEnumerable<PrinterSchedule> schedule, IEnumerable<PrinterServiceExtended> services)
        {
            Printer = printer;
            PrinterSchedule = schedule;
            PrinterServices = services;
            PrinterOwner = printerOwner;
            Operator = @operator;
        }

        /// <summary>
        /// Принтер.
        /// </summary>
        public Printer Printer { get; set; }

        /// <summary>
        /// Оператор принтера.
        /// </summary>
        public User Operator { get; set; }

        /// <summary>
        /// Владелец принтера.
        /// </summary>
        public User PrinterOwner { get; set; }

        /// <summary>
        /// Расписание работы принтера.
        /// </summary>
        public IEnumerable<PrinterSchedule> PrinterSchedule { get; set; }

        /// <summary>
        /// Услуги и цены принтера.
        /// </summary>
        public IEnumerable<PrinterServiceExtended> PrinterServices { get; set; }

        /// <summary>
        /// Активен (онлайн) ли оператор сейчас.
        /// </summary>
        public bool IsOperatorAlive
        {
            get
            {
                TimeSpan threshold = TimeSpan.FromMinutes(double.Parse(WebConfigurationManager.AppSettings["ActivityCheckerThreshold"]));
                return Operator.LastActivityDate > DateTime.Now.Subtract(threshold);
            }
        }

        /// <summary>
        /// Работает ли принтер сейчас.
        /// </summary>
        public bool IsAvailableNow
        {
            get
            {
                DateTime now = DateTime.Now;
                if (Printer.IsDisabled)
                {
                    //printer was marked as disabled by its owner.
                    return false;
                }

                bool isWorkingNow = PrinterSchedule.Any(s =>
                    s.DayOfWeek == (int)now.DayOfWeek
                    && s.OpenTime <= now.TimeOfDay
                    && s.CloseTime >= now.TimeOfDay
                );

                return isWorkingNow;
            }
        }
    }
}
