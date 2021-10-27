
using Core.Entities.Data.PartData;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Data.Admin
{
    public class UserSections : BaseEntity

    {
        [Required]
        public virtual string UserId { get; set; }
        [Required]
        public virtual int SectionID { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Section UserRTMSection { get; set; }

    }
}
