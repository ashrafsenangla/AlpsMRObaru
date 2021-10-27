using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Data.PartData
{
    public class Category : BaseEntity

    {
        [Key, Required, DisplayName("Categorty ID")]
        public int CategoryID { get; set; }
        [StringLength(150), Required]
        public string CategoryName { get; set; }
        [StringLength(300)]
        public string Description { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }


    }
}