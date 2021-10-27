using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Data
{
    public class ExceptionLog : BaseEntity
    {
        public long Id { get; set; }
        public string StackTrace { get; set; }
        public string Source { get; set; }
        public string Message { get; set; }
        public string ExceptionType { get; set; }
        public string FunctionName { get; set; }
        public string Param { get; set; }
    }
}
