using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Core.Entities.Data
{
    public class BaseEntity
    {
        [Display(Name = "Created Date")]
        public DateTime? DateCreated { get; set; }
        [Display(Name = "Modified Date")]
        public DateTime? DateModified { get; set; }
        [Display(Name = "Created User")]
        public string UserCreated { get; set; }
        [Display(Name = "Modified User")]
        public string UserModified { get; set; }
    }
}