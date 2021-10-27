using Core.Entities.Data;
using Infrastructures.Data;
using Interfaces.Repository;
using Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
namespace Services.EPR
{
    public class ExceptionLogService : IExceptionLogService
    {
        private IExceptionLogRepository _exceptionLogRepository;
        public ExceptionLogService()
        {
            _exceptionLogRepository = new ExceptionLogRepository();
        }
        public ExceptionLogService(IExceptionLogRepository exceptionLogRepository)
        {
            _exceptionLogRepository = exceptionLogRepository;
        }
        public ExceptionLog Save(Exception ex, object param=null, [CallerMemberName]string memberName = "")
        {
            try
            {
                var log = new ExceptionLog();
                log.StackTrace = ex.StackTrace;
                log.Source = ex.Source;
                log.Message = ex.Message;
                log.ExceptionType = ex.GetType().ToString();
                log.FunctionName = memberName;
                if (param != null)
                {
                    var json = new JavaScriptSerializer().Serialize(param);
                    log.Param = json;
                }
                _exceptionLogRepository.Save(log);
                
                return log;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
