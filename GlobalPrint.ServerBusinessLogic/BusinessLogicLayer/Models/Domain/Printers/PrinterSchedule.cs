using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Printers
{
    [Table("printer_schedule", Schema = "public")]
    public class PrinterSchedule : IDomainModel
    {
        private int PrinterScheduleID { get; set; }
        [Column("printer_id")]
        public int PrinterID { get; set; }
        [Column("day_of_week")]
        public int DayOfWeek { get; set; }
        [Column("open_time")]
        public TimeSpan OpenTime { get; set; }
        [Column("close_time")]
        public TimeSpan CloseTime { get; set; }

        #region IDomainModel
        [Key]
        [Column("printer_schedule_id")]
        public int ID
        {
            get { return this.PrinterScheduleID; }
            set { this.PrinterScheduleID = value; }
        }
        #endregion
    }
}