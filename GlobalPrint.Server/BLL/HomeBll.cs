using GlobalPrint.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Server
{
    public class HomeBll
    {
        public List<Printer> GetPrinters()
        {
            using (var db = new DB())
            {
                return db.Printers.ToList();
            }
        }
    }
}
