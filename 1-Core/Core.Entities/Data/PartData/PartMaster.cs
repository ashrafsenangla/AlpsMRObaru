using Core.Entities.Data.PartRequest;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Data.PartData
{
    public class PartMaster : BaseEntity

    {
        [Key, Required, DisplayName("Part ID")]
        public int PartID { get; set; }
        [StringLength(80), Required]
        public string PartNo { get; set; }
        [StringLength(80), Required]
        public string ItemNo { get; set; }
        [StringLength(250), Required]
        public string PartName { get; set; }
        public byte[] PartImage { get; set; }
   
        public byte[] PartDrawingImage { get; set; }
        public string PartImagePath { get; set; }
        public string PartDrawingPath { get; set; }
        public string PartImageFileType { get; set; }
        public string PartDrawingFileType { get; set; }

        public int CategoryID { get; set; }
        [ForeignKey("CategoryID")]
        public virtual Category RefCategoryID { get; set; }
        [StringLength(300)]
        public string Description { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }

    }
}