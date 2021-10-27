
using Core.Entities.Data.PartData;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Data.PartRequest
{
    public class PartVendor : BaseEntity

    {
        [Key, Required, DisplayName("Part Vendor ID")]
        public int PartVendorID { get; set; }
        public int PartID { get; set; }
        [ForeignKey("PartID")]
        public virtual PartMaster RefPartMasterID { get; set; }
        public int VendorID { get; set; }
        [ForeignKey("VendorID")]
        public virtual Vendor RefVendorID { get; set; }
      
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }


    }
}