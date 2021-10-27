using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using Core.Entities.Data;
using Infrastructures.Data;
using Core.Entities.Data.Admin;
namespace AspNetGroupBasedPermissions.Models
{
    public class TestModel
    {
        [Required]
        public int MyProperty1 { get; set; }
        [Required]
        public string MyProperty2 { get; set; }
        [Required]
        public string MyProperty3 { get; set; }
    }
}


