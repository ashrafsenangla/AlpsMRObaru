using Core.Entities.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Service
{
    public interface IEmailService
    {
        void SendEmail(EmailSettings setting, object param, string code = "", bool logToDB = true);
    }
}
