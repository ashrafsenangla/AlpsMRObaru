using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNetGroupBasedPermissions.ViewModels
{
    public class Option<T>
    {
        public T Key { get; set; }
        public string Value { get; set; }
        public bool Selected { get; set; }
    }
}