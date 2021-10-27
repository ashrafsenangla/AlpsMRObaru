
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Data.PartData
{
    public class StatusRequest : BaseEntity

    {
        [Key, Required, DisplayName("Status ID")]
        public int StatusID { get; set; }
        [StringLength(50), Required]
        public string StatusName { get; set; }
        [StringLength(300)]
        public string Description { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }

    }
}