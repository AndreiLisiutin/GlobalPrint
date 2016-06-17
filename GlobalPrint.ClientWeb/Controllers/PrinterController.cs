using GlobalPrint.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb
{
    public class Printer_PrintViewModel
    {
        [Required]
        public Printer printer { get; set; }
        [Required(ErrorMessage = "Не выбран файл для печати")]
        public HttpPostedFileBase fileToPrint { get; set; }
    }

    public class PrinterController : Controller
    {
        [HttpGet]
        public ActionResult Print(int printerID)
        {
            Printer printer = new PrinterBll().GetPrinterByID(printerID);
            var vm = new Printer_PrintViewModel()
            {
                printer = printer,
                fileToPrint = null
            };
            return View(vm);
        }


        [HttpPost]
        public ActionResult Print(Printer_PrintViewModel model)
        {
            if (!ModelState.IsValid || model.fileToPrint == null || model.fileToPrint.ContentLength == 0)
            {
                return View("Print", model);
            }

            string path = HttpContext.Server.MapPath("~/App_Data");
            path = Path.Combine(path, model.fileToPrint.FileName);

            using (var fileStream = System.IO.File.Create(path))
            {
                model.fileToPrint.InputStream.Seek(0, SeekOrigin.Begin);
                model.fileToPrint.InputStream.CopyTo(fileStream);
            }
            return RedirectToAction("PrintConfirmation");
        }

        [HttpGet]
        public ActionResult PrintConfirmation()
        {
            return View();
        }
    }
}