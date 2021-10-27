using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Data.Admin
{
    public class Service : BaseEntity
    {

        [Key]
        [Required]
        public virtual int Id { get; set; }

        [DisplayName("Service Name")]
        [StringLength(100)]
        [Required]
        public virtual string NameOption { get; set; }

        
        [DisplayName("Path Name")]
        [StringLength(100)]
        public virtual string Controller { get; set; }

        [DisplayName("Action")]
        [StringLength(100)]
        public virtual string Action { get; set; }

        [DisplayName("Area")]
        [StringLength(100)]
        public virtual string Area { get; set; }

        [DisplayName("Image Class")]
        [StringLength(50)]
        public virtual string ImageClass { get; set; }

        [DisplayName("Active")]
        public bool IsActive { get; set; }

        [DisplayName("Status")]
        public virtual string Status { get; set; }
        [DisplayName("Description")]
        public virtual string Description { get; set; }


        [DisplayName("Parent Name")]
        [Required]
        public virtual int ParentId { get; set; }

        [DisplayName("Is Parent?")]
        public virtual bool IsParent { get; set; }

        [DisplayName("Has Child?")]
        public virtual bool HasChild { get; set; }


        [DisplayName("Sequence")]
        public virtual int Sequence { get; set; }

    }
}
