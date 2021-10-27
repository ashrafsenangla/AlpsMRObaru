using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Core.Entities.Data.Admin;
using Core.Entities.Data.PartData;

namespace Core.Entities.Data
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
            : base()
        {
        
            this.Plants = new HashSet<UserPlants>();
            this.BusinessUnits = new HashSet<UserBusinessUnits>();
            this.Stores = new HashSet<UserStores>();
            this.Sections = new HashSet<UserSections>();
            this.Cabinets = new HashSet<UserCabinets>();
            this.CostCentres = new HashSet<UserCostCentres>();
            this.Groups = new HashSet<ApplicationUserGroup>();
            this.Services = new HashSet<ApplicationUserService>();
         

        }
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public string LoginType { get; set; }
        public string Department { get; set; }
  
        public virtual ICollection<UserPlants> Plants { get; set; }
        public virtual ICollection<UserStores> Stores { get; set; }
        public virtual ICollection<UserCostCentres> CostCentres { get; set; }

        public virtual ICollection<UserSections> Sections { get; set; }
        public virtual ICollection<UserBusinessUnits> BusinessUnits { get; set; }
        public virtual ICollection<UserCabinets> Cabinets { get; set; }
        public virtual ICollection<ApplicationUserGroup> Groups { get; set; }
        public virtual ICollection<ApplicationUserService> Services { get; set; }

    }
}