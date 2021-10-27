

using Core.Entities.Data.Admin;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Data.PartData
{
    public class Section : BaseEntity

    {
        [Key, Required, DisplayName("Section ID")]
        public int SectionID { get; set; }
        [StringLength(50), Required]
        public string SAPCode { get; set; }
        [StringLength(150), Required]
        public string SectionName { get; set; }
        public int CostCentreID { get; set; }
        [ForeignKey("CostCentreID ")]
        public virtual CostCentre RefCostCentreID { get; set; }
        public int BUID { get; set; }
        [ForeignKey("BUID")]
        public virtual BusinessUnit RefBUnitID { get; set; }
        public int PlantID { get; set; }
        [ForeignKey("PlantID")]
        public virtual Plant RefPlantID { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }


    }
}