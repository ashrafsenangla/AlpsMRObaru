using Core.Entities.Data;
using Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Data
{
    public class ExceptionLogRepository : BaseDAC, IExceptionLogRepository
    {
        public ExceptionLog Save(ExceptionLog log)
        {
            _db.ExceptionLogs.Add(log);
            _db.SaveChanges();
            return log;
        }
    }
}
