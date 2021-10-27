using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Core.Entities.Data.Admin;
using Core.Entities.Data;
using Core.Entities.Data.PartData;

namespace Infrastructures.Data
{
    public class IdentityManager
    {
        // Swap ApplicationRole for IdentityRole:
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        private readonly RoleManager<ApplicationRole> _roleManager = new RoleManager<ApplicationRole>(
            new RoleStore<ApplicationRole>(new ApplicationDbContext()));

        private readonly UserManager<ApplicationUser> _userManager = new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(new ApplicationDbContext()));

        public bool RoleExists(string name)
        {
            return _roleManager.RoleExists(name);
        }

        public IdentityResult CreateRole(string name, string description = "")
        {
            // Swap ApplicationRole for IdentityRole:
            return _roleManager.Create(new ApplicationRole(name, description));
        }

        public IdentityResult CreateUser(ApplicationUser user, string password)
        {
            return _userManager.Create(user, password);
        }

        public IdentityResult AddUserToRole(string userId, string roleName)
        {
            return _userManager.AddToRole(userId, roleName);
        }


        public void ClearUserRoles(string userId)
        {
            ApplicationUser user = _userManager.FindById(userId);
            var currentRoles = new List<IdentityUserRole>();

            currentRoles.AddRange(user.Roles);
            foreach (IdentityUserRole role in currentRoles)
            {
                _userManager.RemoveFromRole(userId, role.Role.Name);
            }
        }

        public void RemoveFromRole(string userId, string roleName)
        {
            _userManager.RemoveFromRole(userId, roleName);
        }

        public void DeleteRole(string roleId)
        {
            IQueryable<ApplicationUser> roleUsers = _db.Users.Where(u => u.Roles.Any(r => r.RoleId == roleId));
            ApplicationRole role = _db.Roles.Find(roleId);

            foreach (ApplicationUser user in roleUsers)
            {
                RemoveFromRole(user.Id, role.Name);
            }
            _db.Roles.Remove(role);
            _db.SaveChanges();
        }

        public void CreateGroup(string groupName)
        {
            if (GroupNameExists(groupName))
            {
                throw new GroupExistsException(
                    "A group by that name already exists in the database. Please choose another name.");
            }

            var newGroup = new Group(groupName);
            _db.Groups.Add(newGroup);
            _db.SaveChanges();
        }

        public bool GroupNameExists(string groupName)
        {
            return _db.Groups.Any(gr => gr.Name == groupName);
        }


        public void ClearUserGroups(string userId)
        {
            ClearUserRoles(userId);
            ApplicationUser user = _db.Users.Find(userId);
            user.Groups.Clear();
            _db.SaveChanges();
        }

        public void ClearUserPlants(string userId)
        {
            ApplicationUser user = _db.Users.Find(userId);
            user.Plants.Clear();
            _db.SaveChanges();
        }


        public void ClearUserCostCentres(string userId)
        {
            ApplicationUser user = _db.Users.Find(userId);
            user.CostCentres.Clear();
            _db.SaveChanges();
        }

        public void ClearUserSections(string userId)
        {
            ApplicationUser user = _db.Users.Find(userId);
            user.Sections.Clear();
            _db.SaveChanges();
        }


        public void ClearUserStores(string userId)
        {
            ApplicationUser user = _db.Users.Find(userId);
            user.Stores.Clear();
            _db.SaveChanges();
        }


        public void ClearUserBusinessUnits(string userId)
        {
            ApplicationUser user = _db.Users.Find(userId);
            user.BusinessUnits.Clear();
            _db.SaveChanges();
        }

        public void ClearUserCabinets(string userId)
        {
            ApplicationUser user = _db.Users.Find(userId);
            user.Cabinets.Clear();
            _db.SaveChanges();
        }

        public void ClearStores(string userId)
        {
            //   ClearUserRoles(userId);
            ApplicationUser user = _db.Users.Find(userId);
            user.Stores.Clear();
            _db.SaveChanges();
        }

        public void ClearUserSites(string userId)
        {
            //   ClearUserRoles(userId);
            ApplicationUser user = _db.Users.Find(userId);
        
            _db.SaveChanges();
        }


        public void ClearUserServices(string userId)
        {
         //   ClearUserRoles(userId);
            ApplicationUser user = _db.Users.Find(userId);
            user.Services.Clear();
            _db.SaveChanges();
        }

        public void AddUserToGroup(string userId, int groupId)
        {
            Group group = _db.Groups.Find(groupId);
            ApplicationUser user = _db.Users.Find(userId);

            var userGroup = new ApplicationUserGroup
            {
                Group = group,
                GroupId = group.Id,
                User = user,
                UserId = user.Id
            };

            foreach (ApplicationRoleGroup role in group.Roles)
            {
                _userManager.AddToRole(userId, role.Role.Name);
            }
            user.Groups.Add(userGroup);
            _db.SaveChanges();
        }


        public void AddUserToPlant(string userId, int plantID)
        {
            Plant plant = _db.Plants.Find(plantID);
            ApplicationUser user = _db.Users.Find(userId);

            var userPlant = new UserPlants
            {
                UserRTMPlant = plant,
                PlantID = plant.PlantID,
                User = user,
                UserId = user.Id
            };

            user.Plants.Add(userPlant);
            _db.SaveChanges();
        }


        public void AddUserToStore(string userId, int whouseID)
        {
            Store store = _db.Stores.Find(whouseID);
            ApplicationUser user = _db.Users.Find(userId);

            var userStore = new UserStores
            {
                UserRTMStore = store,
                WhouseID = store.WhouseID,
                User = user,
                UserId = user.Id
            };

            user.Stores.Add(userStore);
            _db.SaveChanges();
        }


        public void AddUserToSection(string userId, int sectionID)
        {
            Section section = _db.Sections.Find(sectionID);
            ApplicationUser user = _db.Users.Find(userId);

            var userSection = new UserSections
            {
                UserRTMSection = section,
                SectionID = section.SectionID,
                User = user,
                UserId = user.Id
            };

            user.Sections.Add(userSection);
            _db.SaveChanges();
        }


        public void AddUserToCostCentre(string userId, int costcentreID)
        {
            CostCentre costcentre = _db.CostCentres.Find(costcentreID);
            ApplicationUser user = _db.Users.Find(userId);

            var userCostCentre = new UserCostCentres
            {
                UserRTMCostCentre = costcentre,
                CostCentreID = costcentre.CostCentreID,
                User = user,
                UserId = user.Id
            };

            user.CostCentres.Add(userCostCentre);
            _db.SaveChanges();
        }


        public void AddUserToCabinet(string userId, int cabinetID)
        {
            Cabinet cabinet = _db.Cabinets.Find(cabinetID);
            ApplicationUser user = _db.Users.Find(userId);

            var userCabinet = new UserCabinets
            {
                UserRTMCabinet = cabinet,
                CabinetID = cabinet.CabinetID,
                User = user,
                UserId = user.Id
            };

            user.Cabinets.Add(userCabinet);
            _db.SaveChanges();
        }


        public void AddUserToBusinessUnit(string userId, int businessunitID)
        {
            BusinessUnit businessunit = _db.BusinessUnits.Find(businessunitID);
            ApplicationUser user = _db.Users.Find(userId);

            var userBusinessUnit = new UserBusinessUnits
            {
                UserRTMBusinessUnit = businessunit,
                BUID = businessunit.BUID,
                User = user,
                UserId = user.Id
            };

            user.BusinessUnits.Add(userBusinessUnit);
            _db.SaveChanges();
        }



        public void AddUserToService(string userId, int serviceId)
        {
            Service service = _db.Services.Find(serviceId);
            ApplicationUser user = _db.Users.Find(userId);

            var userService = new Core.Entities.Data.Admin.ApplicationUserService
            {
                Service = service,
                ServiceId = service.Id,
                User = user,
                UserId = user.Id
            };

            user.Services.Add(userService);
            _db.SaveChanges();
        }



        public void ClearGroupRoles(int groupId)
        {
            Group group = _db.Groups.Find(groupId);
            IQueryable<ApplicationUser> groupUsers = _db.Users.Where(u => u.Groups.Any(g => g.GroupId == group.Id));

            foreach (ApplicationRoleGroup role in group.Roles)
            {
                string currentRoleId = role.RoleId;
                foreach (ApplicationUser user in groupUsers)
                {
                    // Is the user a member of any other groups with this role?
                    int groupsWithRole = user.Groups.Count(g => g.Group.Roles.Any(r => r.RoleId == currentRoleId));

                    // This will be 1 if the current group is the only one:
                    if (groupsWithRole == 1)
                    {
                        RemoveFromRole(user.Id, role.Role.Name);
                    }
                }
            }
            group.Roles.Clear();
            _db.SaveChanges();
        }

        public void AddRoleToGroup(int groupId, string roleName)
        {
            Group group = _db.Groups.Find(groupId);
            ApplicationRole role = _db.Roles.First(r => r.Name == roleName);
            
            var newgroupRole = new ApplicationRoleGroup
            {
                GroupId = group.Id,
                Group = group,
                RoleId = role.Id,
                Role = role
            };

            // make sure the groupRole is not already present
            if (!group.Roles.Contains(newgroupRole))
            {
                group.Roles.Add(newgroupRole);
                _db.SaveChanges();
            }

            // Add all of the users in this group to the new role:
            IQueryable<ApplicationUser> groupUsers = _db.Users.Where(u => u.Groups.Any(g => g.GroupId == group.Id));
            foreach (ApplicationUser user in groupUsers)
            {
                if (!(_userManager.IsInRole(user.Id, roleName)))
                {
                    AddUserToRole(user.Id, role.Name);
                }
            }
        }

        public void DeleteGroup(int groupId)
        {
            Group group = _db.Groups.Find(groupId);

            // Clear the roles from the group:
            ClearGroupRoles(groupId);
            _db.Groups.Remove(group);
            _db.SaveChanges();
        }

        public void DeleteService(int serviceId)
        {
            Service service = _db.Services.Find(serviceId);

            // Clear the roles from the group:
//            ClearGroupRoles(serviceId);
            _db.Services.Remove(service);
            _db.SaveChanges();
        }



    }

    [Serializable]
    public class GroupExistsException : Exception
    {
        public GroupExistsException()
        {
        }

        public GroupExistsException(string message) : base(message)
        {
        }

        public GroupExistsException(string message, Exception inner) : base(message, inner)
        {
        }

        protected GroupExistsException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}