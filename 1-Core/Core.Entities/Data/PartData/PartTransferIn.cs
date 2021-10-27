
using Core.Entities.Data.Admin;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Data.PartData
{
    public class PartTransferIn : BaseEntity

    {
        [Key, Required, DisplayName("Part TransferIn ID")]
        public int PartTransferInID { get; set; }
        public int PartID { get; set; }
        [ForeignKey("PartID ")]
        public virtual PartMaster RefPartID { get; set; }
        public int Qty { get; set; }
        public int BUID { get; set; }
        [ForeignKey("BUID")]
        public virtual BusinessUnit RefBUnitID { get; set; }
        public int CabinetID { get; set; }
        [ForeignKey("CabinetID")]
        public virtual Cabinet RefCabinetID { get; set; }
        public string TransferType { get; set; }

        [StringLength(300)]
        public string Description { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }


    }
}