
using Core.Entities.Data.PartData;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Data.Admin
{
    public class UserBusinessUnits : BaseEntity

    {
        [Required]
        public virtual string UserId { get; set; }
        [Required]
        public virtual int BUID { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual BusinessUnit UserRTMBusinessUnit { get; set; }

    }
}
