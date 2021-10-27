
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Data.PartData
{
    public class PartMaker : BaseEntity

    {
        [Key, Required, DisplayName("Part Maker ID")]
        public int PartMakerID { get; set; }
        public int PartID { get; set; }
        [ForeignKey("PartID")]
        public virtual PartMaster RefPartMasterID { get; set; }
        public int MakerID { get; set; }
        [ForeignKey("MakerID")]
        public virtual Maker RefMakerID { get; set; }
        [StringLength(300)]
        public string Description { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }


    }
}