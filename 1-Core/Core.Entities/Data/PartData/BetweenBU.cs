using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Data.PartData
{
    public class BetweenBU : BaseEntity

    {
        [Key, Required, DisplayName("Part Between BU ID")]
        public int BetweenBUID { get; set; }
        public int PartID { get; set; }
        [ForeignKey("PartID ")]
        public virtual PartMaster RefPartID { get; set; }
        public int MakerID { get; set; }
        [ForeignKey("MakerID ")]
        public virtual PartInv RefMakerID { get; set; }
        public int VendorID { get; set; }
        [ForeignKey("VendorID ")]
        public virtual PartInv RefVendorID { get; set; }
        public int Qty { get; set; }
        public int Balance { get; set; }
        public decimal Price { get; set; }

        public int FromCabinetID { get; set; }
        [ForeignKey("FromCabinetID")]
        public virtual Cabinet RefCabinetID { get; set; }
       
        [StringLength(30)]
        public string TransferType { get; set; }
        [StringLength(50)]
        public string ReferenceNo { get; set; }
        [StringLength(300)]
        public string Description { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }


    }
}