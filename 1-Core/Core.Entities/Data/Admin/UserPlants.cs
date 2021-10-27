using Core.Entities.Data.PartData;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Data.Admin
{
    public class UserPlants : BaseEntity

    {
        [Required]
        public virtual string UserId { get; set; }
        [Required]
        public virtual int PlantID { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Plant UserRTMPlant { get; set; }

    }
}
