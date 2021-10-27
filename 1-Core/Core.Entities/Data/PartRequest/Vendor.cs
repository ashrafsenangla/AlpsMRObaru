using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Data.PartRequest
{
    public class Vendor : BaseEntity

    {
        [Key, Required, DisplayName("Vendor ID")]
        public int VendorID { get; set; }
        [StringLength(20), Required]
        public string VendorNo { get; set; }
        [StringLength(20), Required]
        public string AccountGroup { get; set; }
        [StringLength(250), Required]
        public string VendorName { get; set; }
        [StringLength(20)]
        public string SAPVendorCode { get; set; }
        [StringLength(100)]
        public string PhoneNo { get; set; }
        [StringLength(250)]
        public string Email { get; set; }
        [StringLength(300)]
        public string Description { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }


    }
}