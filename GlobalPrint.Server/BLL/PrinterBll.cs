using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Server
{
    public class PrinterBll
    {
        public Printer GetPrinterByID(int printerID)
        {
            using (var db = new DB())
            {
                return db.Printers.First(e => e.PrinterID == printerID);
            }
        }

        public List<Printer> GetUserPrinterList(int UserID)
        {
            using (var db = new DB())
            {
                return db.Printers.Where(e => e.UserID == UserID).ToList();
            }
        }
    }
}
