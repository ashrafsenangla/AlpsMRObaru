using Core.Entities.Data.PartRequest;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Data.PartData
{
    public class PartInv : BaseEntity

    {
        [Key, Required, DisplayName("Part INV ID")]
        public int PartInvID { get; set; }
        public int PartID { get; set; }
        [ForeignKey("PartID")]
        public virtual PartMaster RefPartMasterPartInvID { get; set; }
        public int StockInv { get; set; }
        public int MinQty { get; set; }
        public int MaxQty { get; set; }
        public int LowQty { get; set; }
        public int LeadTime { get; set; }
        public decimal Price { get; set; }
        public int CabinetID { get; set; }
        [ForeignKey("CabinetID")]
        public virtual Cabinet RefCabinetID { get; set; }
        public int MakerID { get; set; }
        [ForeignKey("MakerID")]
        public virtual Maker RefMakerID { get; set; }
        public int VendorID { get; set; }
        [ForeignKey("VendorID")]
        public virtual Vendor RefVendorID { get; set; }
        [StringLength(300)]
        public string Description { get; set; }

        public string Status { get; set; }
        public bool IsActive { get; set; }


    }
}