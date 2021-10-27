using Core.Entities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Service
{
    public interface IExceptionLogService
    {
        ExceptionLog Save(Exception ex, object param = null, [CallerMemberName]string memberName = "");
    }
}
