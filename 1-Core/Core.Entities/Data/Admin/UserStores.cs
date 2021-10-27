
using Core.Entities.Data.PartData;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Data.Admin
{
    public class UserStores : BaseEntity

    {
        [Required]
        public virtual string UserId { get; set; }
        [Required]
        public virtual int WhouseID { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Store UserRTMStore { get; set; }

    }
}
