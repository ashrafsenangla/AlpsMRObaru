
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Data.PartData
{
    public class Cabinet : BaseEntity

    {
        [Key, Required, DisplayName("Cabinet ID")]
        public int CabinetID { get; set; }
        [StringLength(50), Required]
        public string CabinetName { get; set; }
        public int WhouseID { get; set; }
        [ForeignKey("WhouseID")]
        public virtual Store RefStoreID { get; set; }
        [StringLength(300)]
        public string Description { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }


    }
}