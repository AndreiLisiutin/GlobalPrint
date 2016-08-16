using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.EmailUtility
{
    public interface IEmailUtility
    {
        void Send(string destination, string subject, string body);
        Task SendAsync(string destination, string subject, string body);
    }
}
