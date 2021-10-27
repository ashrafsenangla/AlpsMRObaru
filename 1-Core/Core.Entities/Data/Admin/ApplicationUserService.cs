using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Core.Entities.Data.Admin
{
    public class ApplicationUserService : BaseEntity
    {
        [Required]
        public virtual string UserId { get; set; }
        [Required]
        public virtual int ServiceId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Service Service { get; set; }
    }
}