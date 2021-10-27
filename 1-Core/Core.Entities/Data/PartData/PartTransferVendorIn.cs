
using Core.Entities.Data.Admin;
using Core.Entities.Data.PartRequest;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Data.PartData
{
    public class PartTransferVendorIn : BaseEntity

    {
        [Key, Required, DisplayName("Part TransferVendorIn ID")]
        public int PartTransferVendorInID { get; set; }
        public int PartID { get; set; }
        [ForeignKey("PartID ")]
        public virtual PartMaster RefPartID { get; set; }
        public int Qty { get; set; }
        public int Balance { get; set; }

        public int VendorID { get; set; }
        [ForeignKey("VendorID")]
        public virtual Vendor RefVendorID { get; set; }

        public int MakerID { get; set; }
        [ForeignKey("MakerID")]
        public virtual Maker RefMakerID { get; set; }
        public decimal NewPrice { get; set; }
        public int CabinetID { get; set; }
        [ForeignKey("CabinetID")]
        public virtual Cabinet RefCabinetID { get; set; }
        public string TransferType { get; set; }
        public string ReferenceNo { get; set; }
        [StringLength(300)]
        public string Description { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }


    }
}