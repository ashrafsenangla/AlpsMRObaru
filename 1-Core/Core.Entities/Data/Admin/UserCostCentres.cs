
using Core.Entities.Data.PartData;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Data.Admin
{
    public class UserCostCentres : BaseEntity

    {
        [Required]
        public virtual string UserId { get; set; }
        [Required]
        public virtual int CostCentreID { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual CostCentre UserRTMCostCentre { get; set; }

    }
}
