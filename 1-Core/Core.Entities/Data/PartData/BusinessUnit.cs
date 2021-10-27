using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Data.PartData
{
    public class BusinessUnit : BaseEntity
    {
        [Key]
        public int BUID { get; set; }

        [StringLength(150), Required]
        public string BusinessUnitName { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
    }
}