

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Data.PartData
{
    public class CostCentre : BaseEntity

    {
        [Key, Required, DisplayName("Cost Centre ID")]
        public int CostCentreID { get; set; }
        [StringLength(30), Required]
        public string CostCentreName { get; set; }
        [StringLength(300)]
        public string Description { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }


    }
}