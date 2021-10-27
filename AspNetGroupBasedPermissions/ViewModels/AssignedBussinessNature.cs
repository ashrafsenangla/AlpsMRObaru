using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNetGroupBasedPermissions.ViewModels
{
    public class AssignedBussinessNature
    {
        public int BussinessID { get; set; }
        public string BussinessName { get; set; }
        public bool Assigned { get; set; }
    }
}