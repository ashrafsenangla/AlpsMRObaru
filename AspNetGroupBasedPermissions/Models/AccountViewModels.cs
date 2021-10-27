using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using Core.Entities.Data;
using Infrastructures.Data;
using Core.Entities.Data.Admin;
using Core.Entities.Data.PartData;

namespace AspNetGroupBasedPermissions.Models
{
    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage =
            "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage =
            "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }


    public class PasswordResetViewModel
    {
        [Required]
        [Display(Name = "Token")]
        public string Token { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }

    public class BeforePasswordResetViewModel
    {
        [Required]
        [Display(Name = "E-mail address")]
        [EmailAddress]
        public string Email { get; set; }

    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }


    public class RegisterViewModel
    {

        public int Id { get; set; }

        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage =
            "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage =
            "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        // New Fields added to extend Application User class:

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        public string ResetCode { get; set; }


        [Display(Name = "Last Name")]
        public string LastName { get; set; }

     
        [Display(Name = "Login Type")]
        public string LoginType { get; set; }


        [Required]
        public string Email { get; set; }

        // Return a pre-poulated instance of AppliationUser:
        public ApplicationUser GetUser()
        {
            var user = new ApplicationUser()
            {
                UserName = this.UserName,
                FirstName = this.FirstName,
                LastName = this.LastName,
                Email = this.Email,
            };
        return user;
        }

        
        public ApplicationUser GetUserNew(string userName, string firstName,string lastName, string email)
        {
            var user = new ApplicationUser()
            {
                UserName = userName,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
            };
            return user;
        }
    }


    public class EditUserViewModel
    {
        public EditUserViewModel() { }

        // Allow Initialization with an instance of ApplicationUser:
        public EditUserViewModel(ApplicationUser user)
        {
            this.UserName = user.UserName;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.Email = user.Email;
        }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }


    }

    // Used to display a single role with a checkbox, within a list structure:
    public class SelectRoleEditorViewModel
    {
        public SelectRoleEditorViewModel() { }

        // Update this to accept an argument of type ApplicationRole:
        public SelectRoleEditorViewModel(ApplicationRole role)
        {
            this.RoleName = role.Name;

            // Assign the new Descrption property:
            this.Description = role.Description;
        }

        public bool Selected { get; set; }

        [Required]
        public string RoleName { get; set; }

        // Add the new Description property:
        public string Description { get; set; }
    }


    public class RoleViewModel
    {
        public string RoleName { get; set; }
        public string Description { get; set; }

        public RoleViewModel() { }
        public RoleViewModel(ApplicationRole role)
        {
            this.RoleName = role.Name;
            this.Description = role.Description;
        }
    }


    public class EditRoleViewModel
    {
        public string OriginalRoleName { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }

        public EditRoleViewModel() { }
        public EditRoleViewModel(ApplicationRole role)
        {
            this.OriginalRoleName = role.Name;
            this.RoleName = role.Name;
            this.Description = role.Description;
        }
    }
  


    // Wrapper for SelectGroupEditorViewModel to select user group membership:
    public class SelectUserGroupsViewModel
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<SelectGroupEditorViewModel> Groups { get; set; }

        public SelectUserGroupsViewModel()
        {
            this.Groups = new List<SelectGroupEditorViewModel>();
        }

        public SelectUserGroupsViewModel(ApplicationUser user)
            : this()
        {
            this.UserName = user.UserName;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;

            var Db = new ApplicationDbContext();

            // Add all available groups to the public list:
            var allGroups = Db.Groups;
            foreach (var role in allGroups)
            {
                // An EditorViewModel will be used by Editor Template:
                var rvm = new SelectGroupEditorViewModel(role);
                this.Groups.Add(rvm);
            }

            // Set the Selected property to true where user is already a member:
            foreach (var group in user.Groups)
            {
//              var checkUserRole = this.Groups.Find(r => r.GroupName == group.Group.Name);
                var checkUserRole = this.Groups.Find(r => r.GroupName == group.Group.Name);

                checkUserRole.Selected = true;
            }
        }
    }


    public class SelectUserPlantsViewModel
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<SelectPlantEditorViewModel> Plants { get; set; }

        public SelectUserPlantsViewModel()
        {
            this.Plants = new List<SelectPlantEditorViewModel>();
        }

        public SelectUserPlantsViewModel(ApplicationUser user) : this()
        {
            this.UserName = user.UserName;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;

            var db = new ApplicationDbContext();
            var allGroups = db.Plants;
            foreach (var role in allGroups)
            {
                if (role.IsActive)
                {
                    var rvm = new SelectPlantEditorViewModel(role);
                    this.Plants.Add(rvm);
                }
            }

            foreach (var group in user.Plants)
            {
                var checkUserRole = this.Plants.Find(r => r.PlantName == group.UserRTMPlant.PlantName);
                checkUserRole.Selected = true;
            }
        }
    }



    public class SelectUserStoresViewModel
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<SelectStoreEditorViewModel> Stores { get; set; }

        public SelectUserStoresViewModel()
        {
            this.Stores = new List<SelectStoreEditorViewModel>();
        }

        public SelectUserStoresViewModel(ApplicationUser user) : this()
        {
            this.UserName = user.UserName;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;

            var db = new ApplicationDbContext();
            var allGroups = db.Stores;
            foreach (var role in allGroups)
            {
                if (role.IsActive)
                {
                    var rvm = new SelectStoreEditorViewModel(role);
                    this.Stores.Add(rvm);
                }
            }

            foreach (var group in user.Stores)
            {
                var checkUserRole = this.Stores.Find(r => r.WhouseName == group.UserRTMStore.WhouseName);
                checkUserRole.Selected = true;
            }
        }
    }




    public class SelectUserCostCentresViewModel
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<SelectCostCentreEditorViewModel> CostCentres { get; set; }

        public SelectUserCostCentresViewModel()
        {
            this.CostCentres = new List<SelectCostCentreEditorViewModel>();
        }

        public SelectUserCostCentresViewModel(ApplicationUser user) : this()
        {
            this.UserName = user.UserName;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;

            var db = new ApplicationDbContext();
            var allGroups = db.CostCentres;
            foreach (var role in allGroups)
            {
                if (role.IsActive)
                {
                    var rvm = new SelectCostCentreEditorViewModel(role);
                    this.CostCentres.Add(rvm);
                }
            }

            foreach (var group in user.CostCentres)
            {
                var checkUserRole = this.CostCentres.Find(r => r.CostCentreName == group.UserRTMCostCentre.CostCentreName);
                checkUserRole.Selected = true;
            }
        }
    }


    public class SelectUserSectionsViewModel
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<SelectSectionEditorViewModel> Sections { get; set; }

        public SelectUserSectionsViewModel()
        {
            this.Sections = new List<SelectSectionEditorViewModel>();
        }

        public SelectUserSectionsViewModel(ApplicationUser user) : this()
        {
            this.UserName = user.UserName;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;

            var db = new ApplicationDbContext();
            var allGroups = db.Sections;
            foreach (var role in allGroups)
            {
                if (role.IsActive)
                {
                    var rvm = new SelectSectionEditorViewModel(role);
                    this.Sections.Add(rvm);
                }
            }

            foreach (var group in user.Sections)
            {
                var checkUserRole = this.Sections.Find(r => r.SectionName == group.UserRTMSection.SectionName);
                checkUserRole.Selected = true;
            }
        }
    }


    public class SelectUserCabinetsViewModel
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<SelectCabinetEditorViewModel> Cabinets { get; set; }

        public SelectUserCabinetsViewModel()
        {
            this.Cabinets = new List<SelectCabinetEditorViewModel>();
        }

        public SelectUserCabinetsViewModel(ApplicationUser user) : this()
        {
            this.UserName = user.UserName;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;

            var db = new ApplicationDbContext();
            var allGroups = db.Cabinets;
            foreach (var role in allGroups)
            {
                if (role.IsActive)
                {
                    var rvm = new SelectCabinetEditorViewModel(role);
                    this.Cabinets.Add(rvm);
                }
            }

            foreach (var group in user.Cabinets)
            {
                var checkUserRole = this.Cabinets.Find(r => r.CabinetName == group.UserRTMCabinet.CabinetName);
                checkUserRole.Selected = true;
            }
        }
    }


    public class SelectUserBusinessUnitsViewModel
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<SelectBusinessUnitEditorViewModel> BusinessUnits { get; set; }

        public SelectUserBusinessUnitsViewModel()
        {
            this.BusinessUnits = new List<SelectBusinessUnitEditorViewModel>();
        }

        public SelectUserBusinessUnitsViewModel(ApplicationUser user) : this()
        {
            this.UserName = user.UserName;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;

            var db = new ApplicationDbContext();
            var allGroups = db.BusinessUnits;
            foreach (var role in allGroups)
            {
                if (role.IsActive)
                {
                    var rvm = new SelectBusinessUnitEditorViewModel(role);
                    this.BusinessUnits.Add(rvm);
                }
            }

            foreach (var group in user.BusinessUnits)
            {
                var checkUserRole = this.BusinessUnits.Find(r => r.BusinessUnitName == group.UserRTMBusinessUnit.BusinessUnitName);
                checkUserRole.Selected = true;
            }
        }
    }



    public class SelectUserCostCentersViewModel
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyCode { get; set; }
        public List<SelectCostCenterEditorViewModel> RTMCostCenters { get; set; }

        public SelectUserCostCentersViewModel()
        {
            this.RTMCostCenters = new List<SelectCostCenterEditorViewModel>();
        }
    }

    public class SelectUserServicesViewModel
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<SelectServiceEditorViewModel> Services { get; set; }

        public SelectUserServicesViewModel()
        {
            this.Services = new List<SelectServiceEditorViewModel>();
        }

        public SelectUserServicesViewModel(ApplicationUser user)
            : this()
        {
            this.UserName = user.UserName;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;

            var Db = new ApplicationDbContext();

            // Add all available groups to the public list:
            var allServices = Db.Services;
            foreach (var service in allServices)
            {
                // An EditorViewModel will be used by Editor Template:
                var rvm = new SelectServiceEditorViewModel(service);
                this.Services.Add(rvm);
            }

            // Set the Selected property to true where user is already a member:
            foreach (var service in user.Services)
            {
                var checkUserRole = this.Services.Find(r => r.ServiceName == service.Service.NameOption);

                checkUserRole.Selected = true;
            }
        }
    }


    // Used to display a single role group with a checkbox, within a list structure:
    public class SelectGroupEditorViewModel
    {
        public SelectGroupEditorViewModel() { }
        public SelectGroupEditorViewModel(Group group)
        {
            this.GroupName = group.Name;
            this.GroupId = group.Id;
        }

        public bool Selected { get; set; }

        [Required]
        public int GroupId { get; set; }
        public string GroupName { get; set; }
    }
    

    public class SelectPlantEditorViewModel
    {
        public SelectPlantEditorViewModel() { }
        public SelectPlantEditorViewModel(Plant plant)
        {
            this.PlantName = plant.PlantName;
            this.PlantID = plant.PlantID;
        }

        public bool Selected { get; set; }

        [Required]
        public int PlantID { get; set; }
        public string PlantName { get; set; }
    }



    public class SelectStoreEditorViewModel
    {
        public SelectStoreEditorViewModel() { }
        public SelectStoreEditorViewModel(Store store)
        {
            this.WhouseName = store.WhouseName;
            this.WhouseID = store.WhouseID;
        }

        public bool Selected { get; set; }

        [Required]
        public int WhouseID { get; set; }
        public string WhouseName { get; set; }
    }




    public class SelectCostCentreEditorViewModel
    {
        public SelectCostCentreEditorViewModel() { }
        public SelectCostCentreEditorViewModel(CostCentre costcentre)
        {
            this.CostCentreName = costcentre.CostCentreName;
            this.CostCentreID = costcentre.CostCentreID;
        }

        public bool Selected { get; set; }

        [Required]
        public int CostCentreID { get; set; }
        public string CostCentreName { get; set; }
    }




    public class SelectSectionEditorViewModel
    {
        public SelectSectionEditorViewModel() { }
        public SelectSectionEditorViewModel(Section section)
        {
            this.SectionName = section.SectionName;
            this.SectionID = section.SectionID;
        }

        public bool Selected { get; set; }

        [Required]
        public int SectionID { get; set; }
        public string SectionName { get; set; }
    }


    public class SelectCabinetEditorViewModel
    {
        public SelectCabinetEditorViewModel() { }
        public SelectCabinetEditorViewModel(Cabinet cabinet)
        {
            this.CabinetName = cabinet.CabinetName;
            this.CabinetID = cabinet.CabinetID;
        }

        public bool Selected { get; set; }

        [Required]
        public int CabinetID { get; set; }
        public string CabinetName { get; set; }
    }


    public class SelectBusinessUnitEditorViewModel
    {
        public SelectBusinessUnitEditorViewModel() { }
        public SelectBusinessUnitEditorViewModel(BusinessUnit businessunit)
        {
            this.BusinessUnitName = businessunit.BusinessUnitName;
            this.BUID = businessunit.BUID;
        }

        public bool Selected { get; set; }

        [Required]
        public int BUID { get; set; }
        public string BusinessUnitName { get; set; }
    }


    public class SelectCostCenterEditorViewModel
   {
       public SelectCostCenterEditorViewModel() { }

       public bool Selected { get; set; }
       public string CompanyCode { get; set; }

       [Required]
       public int CostCentreID { get; set; }
       public string CostCenterName { get; set; }


       public bool BulkPurchase { get; set; }

       public bool PurchaseOnBehalf { get; set; }
   }

  
    public class SelectServiceEditorViewModel
    {
        public SelectServiceEditorViewModel() { }
        public SelectServiceEditorViewModel(Service service)
        {
            this.ServiceName = service.NameOption;
            this.ServiceId = service.Id;
        }

        public bool Selected { get; set; }

        [Required]
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
    }

    
    public class SelectGroupRolesViewModel
    {
        public SelectGroupRolesViewModel()
        {
            this.Roles = new List<SelectRoleEditorViewModel>();
        }


        // Enable initialization with an instance of ApplicationUser:
        public SelectGroupRolesViewModel(Group group)
            : this()
        {
            this.GroupId = group.Id;
            this.GroupName = group.Name;

            var Db = new ApplicationDbContext();

            // Add all available roles to the list of EditorViewModels:
            var allRoles = Db.Roles;
            foreach (var role in allRoles)
            {
                // An EditorViewModel will be used by Editor Template:
                var rvm = new SelectRoleEditorViewModel(role);
                this.Roles.Add(rvm);
            }

            // Set the Selected property to true for those roles for 
            // which the current user is a member:
            foreach (var groupRole in group.Roles)
            {
                var checkGroupRole =
                    this.Roles.Find(r => r.RoleName == groupRole.Role.Name);
                checkGroupRole.Selected = true;
            }
        }

        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public List<SelectRoleEditorViewModel> Roles { get; set; }
    }


    public class UserPermissionsViewModel
    {
        public UserPermissionsViewModel()
        {
            this.Roles = new List<RoleViewModel>();
        }


        // Enable initialization with an instance of ApplicationUser:
        public UserPermissionsViewModel(ApplicationUser user)
            : this()
        {
            this.UserName = user.UserName;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            foreach (var role in user.Roles)
            {
                var appRole = (ApplicationRole)role.Role;
                var pvm = new RoleViewModel(appRole);
                this.Roles.Add(pvm);
            }
        }

        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<RoleViewModel> Roles { get; set; }
    }


}

