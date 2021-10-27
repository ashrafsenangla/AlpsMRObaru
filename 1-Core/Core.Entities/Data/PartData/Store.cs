
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Data.PartData
{
    public class Store : BaseEntity

    {
        [Key, Required, DisplayName("Warehouse ID")]
        public int WhouseID { get; set; }
        [StringLength(50), Required]
        public string WhouseName { get; set; }
        public int SectionID { get; set; }
        [ForeignKey("SectionID")]
        public virtual Section RefSectionID { get; set; }
        [StringLength(300)]
        public string Description { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }


    }
}