
using Core.Entities.Data.PartData;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Data.Admin
{
    public class UserCabinets : BaseEntity

    {
        [Required]
        public virtual string UserId { get; set; }
        [Required]
        public virtual int CabinetID { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Cabinet UserRTMCabinet { get; set; }

    }
}
