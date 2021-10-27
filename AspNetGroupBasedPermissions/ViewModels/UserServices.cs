using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNetGroupBasedPermissions.ViewModels
{
    public class UserServices
    {
        public int Id { get; set; }
        public string NameOption { get; set; }
        public int ParentId { get; set; }
        public string Action { get; set; }
        public string ImageClass { get; set; }
        public bool IsParent { get; set; }
        public bool IsActive { get; set; }
        public string Controller { get; set; }
        public int Sequence { get; set; }

    }
}