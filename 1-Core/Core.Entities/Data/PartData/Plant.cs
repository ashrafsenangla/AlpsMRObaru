
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Data.PartData
{
    public class Plant: BaseEntity

    {
        [Key, Required, DisplayName("Plant ID")]
        public int PlantID { get; set; }

        [StringLength(30), Required]
        public string PlantName { get; set; }

        [StringLength(300)]
        public string Description { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }


    }
}