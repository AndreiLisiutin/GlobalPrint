using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Server
{
    [Table("printer_schedule", Schema = "public")]
    public class PrinterSchedule
    {
        [Key]
        [Column("printer_schedule_id")]
        public int PrinterScheduleID { get; set; }
        [Column("printer_id")]
        public int PrinterID { get; set; }
        [Column("day_of_week")]
        public int DayOfWeek { get; set; }
        [Column("open_time")]
        public TimeSpan OpenTime { get; set; }
        [Column("close_time")]
        public TimeSpan CloseTime { get; set; }
    }
}