using Core.Entities.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Agent
{
    public interface IEmailAgent
    {
        void Send(EmailSettings setting);
    }
}
