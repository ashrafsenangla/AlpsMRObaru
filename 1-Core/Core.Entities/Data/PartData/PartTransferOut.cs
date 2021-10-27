using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Data.PartData
{
    public class PartTransferOut : BaseEntity

    {
        [Key, Required, DisplayName("Part TransferOut ID")]
        public int PartTransferOutID { get; set; }
        public int PartID { get; set; }
        [ForeignKey("PartID ")]
        public virtual PartMaster RefPartID { get; set; }
        public int Qty { get; set; }
        public int FromCabinetID { get; set; }
        [ForeignKey("FromCabinetID")]
        public virtual Cabinet RefCabinetID { get; set; }
        public int ToStoreID { get; set; }
        [ForeignKey("ToStoreID")]
        public virtual Store RefStoreID { get; set; }
        [StringLength(300)]
        public string TransferType { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }


    }
}