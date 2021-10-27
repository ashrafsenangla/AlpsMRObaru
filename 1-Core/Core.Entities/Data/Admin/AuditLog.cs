using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Core.Entities.Data.Admin
{
 
    public class AuditLog
    {
        public Guid auditlogid { get; set; }
        public string userid { get; set; }
        public DateTime eventdateutc { get; set; }
        public string eventtype { get; set; }
        public string tablename { get; set; }
        public string recordid { get; set; }
        public string columnname { get; set; }
        public string originalvalue { get; set; }
        public string newvalue { get; set; }
        public string costCenterCode { get; set; }
        //public string datatype { get; set; }

        //[DefaultValue(0)]
        //public int flag { get; set; }
        //public string originalAuditLogID { get; set; }
        //public string filename { get; set; }
    }

}