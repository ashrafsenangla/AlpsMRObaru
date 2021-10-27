using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Data.PartData
{
    public class Maker : BaseEntity

    {
        [Key, Required, DisplayName("Maker ID")]
        public int MakerID { get; set; }
        [StringLength(150), Required]
        public string MakerName { get; set; }
        [StringLength(300)]
        public string Description { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }


    }
}