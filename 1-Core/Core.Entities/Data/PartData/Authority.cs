using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Data.PartData
{
    public class Authority : BaseEntity

    {
        [Key, Required, DisplayName("Authority ID")]
        public int AuthorityID { get; set; }
        [StringLength(150), Required]
        public string AuthorityName { get; set; }
        [StringLength(300)]
        public string Description { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }

    }
}