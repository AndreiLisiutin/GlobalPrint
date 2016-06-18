using GlobalPrint.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb
{
    public class Printer_PrintViewModel : Printer_PrintPostModel
    {
        [Required]
        public PrinterInfo printer { get; set; }
        public List<SelectListItem> formatStore { get; set; }
    }
    public class Printer_PrintPostModel
    {
        public PrintOrder order { get; set; }
        [Required(ErrorMessage = "Не выбран файл для печати")]
        public HttpPostedFileBase fileToPrint { get; set; }
    }
}