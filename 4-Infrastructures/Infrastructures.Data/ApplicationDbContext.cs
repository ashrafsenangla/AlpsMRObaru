using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entities.Data.Admin;
using System.Data.Entity.Infrastructure;
using System.Reflection;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Core.Objects;
using System.Configuration;
using System.Linq.Expressions;
using System.Data.Entity.ModelConfiguration.Configuration;
using Core.Entities.Data;
using Core.Entities;
using Core.Entities.Data.PartData;

namespace Infrastructures.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        //Administration DB Context
        public virtual IDbSet<BusinessUnit> BusinessUnits { get; set; }
        public virtual IDbSet<Plant> Plants { get; set; }
        public virtual IDbSet<CostCentre> CostCentres { get; set; }
        public virtual IDbSet<Store> Stores { get; set; }
        public virtual IDbSet<Section> Sections { get; set; }
        public virtual IDbSet<Cabinet> Cabinets { get; set; }
        public virtual IDbSet<Group> Groups { get; set; }
        public virtual IDbSet<Service> Services { get; set; }
        public virtual IDbSet<ApplicationUserService> ApplicationUserService { get; set; }
        new public virtual IDbSet<ApplicationRole> Roles { get; set; }
        public System.Data.Entity.DbSet<Core.Entities.Data.ExceptionLog> ExceptionLogs { get; set; }
        public System.Data.Entity.DbSet<Core.Entities.Data.Admin.AuditLog> AuditLogs { get; set; }

        // Alps Table Part Data
       // public System.Data.Entity.DbSet<Core.Entities.Data.PartData.BUnit> BUnits { get; set; }
        public System.Data.Entity.DbSet<Core.Entities.Data.PartData.Category> Categorys { get; set; }
        public System.Data.Entity.DbSet<Core.Entities.Data.PartData.Maker> Makers { get; set; }
        public System.Data.Entity.DbSet<Core.Entities.Data.PartData.PartMaster> PartMasters { get; set; }

        public System.Data.Entity.DbSet<Core.Entities.Data.PartData.PartInv> PartInvs { get; set; }
        public System.Data.Entity.DbSet<Core.Entities.Data.PartData.PartMaker> PartMakers { get; set; }
        public System.Data.Entity.DbSet<Core.Entities.Data.PartRequest.PartVendor> PartVendors { get; set; }
        public System.Data.Entity.DbSet<Core.Entities.Data.Admin.UserBusinessUnits> UserBizUnits { get; set; }
        public System.Data.Entity.DbSet<Core.Entities.Data.Admin.UserStores> UserWareHouses { get; set; }

        public System.Data.Entity.DbSet<Core.Entities.Data.PartData.PartTransferIn> PartTransferIns { get; set; }
        public System.Data.Entity.DbSet<Core.Entities.Data.PartData.PartTransferVendorIn> PartTransferVendorIns { get; set; }

        public System.Data.Entity.DbSet<Core.Entities.Data.PartData.PartTransferOut> PartTransferOuts { get; set; }
        public System.Data.Entity.DbSet<Core.Entities.Data.PartData.PartTransferChargeOut> PartTransferChargeOuts { get; set; }

        public System.Data.Entity.DbSet<Core.Entities.Data.PartData.StatusRequest> StatusRequests { get; set; }
     
        public System.Data.Entity.DbSet<Core.Entities.Data.PartData.Authority> Authoritys { get; set; }

        // Setting DB Part Request
        public System.Data.Entity.DbSet<Core.Entities.Data.PartRequest.Vendor> Vendors { get; set; }


        // Setting DB Context


        public ApplicationDbContext()
            : base("DefaultConnection")
        {   }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException("modelBuilder");
            }

            // Keep this:
            modelBuilder.Entity<IdentityUser>().ToTable("AspNetUsers");

            // Change TUser to ApplicationUser everywhere else - IdentityUser and ApplicationUser essentially 'share' the AspNetUsers Table in the database:
            EntityTypeConfiguration<ApplicationUser> table =
                modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers");

            table.Property((ApplicationUser u) => u.UserName).IsRequired();

            // EF won't let us swap out IdentityUserRole for ApplicationUserRole here:
            modelBuilder.Entity<ApplicationUser>().HasMany<IdentityUserRole>((ApplicationUser u) => u.Roles);
            modelBuilder.Entity<IdentityUserRole>().HasKey((IdentityUserRole r) =>
                new { UserId = r.UserId, RoleId = r.RoleId }).ToTable("AspNetUserRoles");

            // Add the group stuff here:
            modelBuilder.Entity<ApplicationUser>().HasMany<ApplicationUserGroup>((ApplicationUser u) => u.Groups);
            modelBuilder.Entity<ApplicationUserGroup>().HasKey((ApplicationUserGroup r) => new { UserId = r.UserId, GroupId = r.GroupId }).ToTable("ApplicationUserGroups");

            //// Add the group stuff here:
            modelBuilder.Entity<ApplicationUser>().HasMany<UserPlants>((ApplicationUser u) => u.Plants);
            modelBuilder.Entity<UserPlants>().HasKey((UserPlants r) => new { UserId = r.UserId, PlantID = r.PlantID }).ToTable("UserPlants");

            modelBuilder.Entity<ApplicationUser>().HasMany<UserBusinessUnits>((ApplicationUser u) => u.BusinessUnits);
            modelBuilder.Entity<UserBusinessUnits>().HasKey((UserBusinessUnits r) => new { UserId = r.UserId, BUID = r.BUID }).ToTable("UserBusinessUnits");


            modelBuilder.Entity<ApplicationUser>().HasMany<UserCostCentres>((ApplicationUser u) => u.CostCentres);
            modelBuilder.Entity<UserCostCentres>().HasKey((UserCostCentres r) => new { UserId = r.UserId, CostCentreID = r.CostCentreID }).ToTable("UserCostCentres");

            modelBuilder.Entity<ApplicationUser>().HasMany<UserStores>((ApplicationUser u) => u.Stores);
            modelBuilder.Entity<UserStores>().HasKey((UserStores r) => new { UserId = r.UserId, WhouseID = r.WhouseID }).ToTable("UserStores");

            modelBuilder.Entity<ApplicationUser>().HasMany<UserCabinets>((ApplicationUser u) => u.Cabinets);
            modelBuilder.Entity<UserCabinets>().HasKey((UserCabinets r) => new { UserId = r.UserId, CabinetID = r.CabinetID }).ToTable("UserCabinets");
            modelBuilder.Entity<ApplicationUser>().HasMany<UserSections>((ApplicationUser u) => u.Sections);
            modelBuilder.Entity<UserSections>().HasKey((UserSections r) => new { UserId = r.UserId, SectionID = r.SectionID }).ToTable("UserSections");


            // Add the group stuff here:
            modelBuilder.Entity<ApplicationUser>().HasMany<ApplicationUserService>((ApplicationUser u) => u.Services);
            modelBuilder.Entity<ApplicationUserService>().HasKey((ApplicationUserService r) => new { UserId = r.UserId, ServiceId = r.ServiceId }).ToTable("ApplicationUserService");

            // And here:
            modelBuilder.Entity<Group>().HasMany<ApplicationRoleGroup>((Group g) => g.Roles);
            modelBuilder.Entity<ApplicationRoleGroup>().HasKey((ApplicationRoleGroup gr) => new { RoleId = gr.RoleId, GroupId = gr.GroupId }).ToTable("ApplicationRoleGroups");


            //And Here:
            EntityTypeConfiguration<Group> groupsConfig = modelBuilder.Entity<Group>().ToTable("Groups");
            groupsConfig.Property((Group r) => r.Name).IsRequired();
            //comment cni

            // Leave this alone:
            EntityTypeConfiguration<IdentityUserLogin> entityTypeConfiguration =
                modelBuilder.Entity<IdentityUserLogin>().HasKey((IdentityUserLogin l) =>
                    new { UserId = l.UserId, LoginProvider = l.LoginProvider, ProviderKey = l.ProviderKey }).ToTable("AspNetUserLogins");

            entityTypeConfiguration.HasRequired<IdentityUser>((IdentityUserLogin u) => u.User);
            EntityTypeConfiguration<IdentityUserClaim> table1 = modelBuilder.Entity<IdentityUserClaim>().ToTable("AspNetUserClaims");
            table1.HasRequired<IdentityUser>((IdentityUserClaim u) => u.User);

            // Add this, so that IdentityRole can share a table with ApplicationRole:
            modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles");

            // Change these from IdentityRole to ApplicationRole:
            EntityTypeConfiguration<ApplicationRole> entityTypeConfiguration1 = modelBuilder.Entity<ApplicationRole>().ToTable("AspNetRoles");
            entityTypeConfiguration1.Property((ApplicationRole r) => r.Name).IsRequired();

            //add one to many relationship
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            foreach (Type classType in from t in Assembly.GetAssembly(typeof(DecimalPrecisionAttribute)).GetTypes()
                                       where t.IsClass && t.Namespace == "Core.Entities.Data.EPR"
                                       select t)
            {
                foreach (var propAttr in classType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.GetCustomAttribute<DecimalPrecisionAttribute>() != null).Select(
                       p => new { prop = p, attr = p.GetCustomAttribute<DecimalPrecisionAttribute>(true) }))
                {

                    var entityConfig = modelBuilder.GetType().GetMethod("Entity").MakeGenericMethod(classType).Invoke(modelBuilder, null);
                    ParameterExpression param = ParameterExpression.Parameter(classType, "c");
                    Expression property = Expression.Property(param, propAttr.prop.Name);
                    LambdaExpression lambdaExpression = Expression.Lambda(property, true,
                                                                             new ParameterExpression[] { param });
                    DecimalPropertyConfiguration decimalConfig;
                    if (propAttr.prop.PropertyType.IsGenericType && propAttr.prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        MethodInfo methodInfo = entityConfig.GetType().GetMethods().Where(p => p.Name == "Property").ToList()[7];
                        decimalConfig = methodInfo.Invoke(entityConfig, new[] { lambdaExpression }) as DecimalPropertyConfiguration;
                    }
                    else
                    {
                        MethodInfo methodInfo = entityConfig.GetType().GetMethods().Where(p => p.Name == "Property").ToList()[6];
                        decimalConfig = methodInfo.Invoke(entityConfig, new[] { lambdaExpression }) as DecimalPropertyConfiguration;
                    }

                    decimalConfig.HasPrecision(propAttr.attr.Precision, propAttr.attr.Scale);
                }
            }

        }

        public override int SaveChanges()
        {
            var entities = ChangeTracker.Entries().Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            var currentUsername = HttpContext.Current != null && HttpContext.Current.User != null
                ? HttpContext.Current.User.Identity.Name
                : "Anonymous";

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((BaseEntity)entity.Entity).DateCreated = DateTime.Now;
                    ((BaseEntity)entity.Entity).UserCreated = currentUsername;
                    ((BaseEntity)entity.Entity).DateModified = DateTime.Now;
                    ((BaseEntity)entity.Entity).UserModified = currentUsername;
                }
                else
                {
                    ((BaseEntity)entity.Entity).DateModified = DateTime.Now;
                    ((BaseEntity)entity.Entity).UserModified = currentUsername;
                }
            }

            List<DbEntityEntry> modifiedDeletedEnt = this.ChangeTracker.Entries().Where(p => p.State == EntityState.Deleted || p.State == EntityState.Modified).ToList() ?? new List<DbEntityEntry>();
            List<DbEntityEntry> addedEnt = this.ChangeTracker.Entries().Where(p => p.State == EntityState.Added).ToList() ?? new List<DbEntityEntry>();
            int countAddedEnt = 0;
            for (int i = 0; i < modifiedDeletedEnt.Count; i++)
            {
                foreach (AuditLog x in GetAuditRecordsForChange(modifiedDeletedEnt[i], currentUsername))
                {
                    this.AuditLogs.Add(x);
                }
            }
            for (int i = 0; i < addedEnt.Count; i++)
            {
                if (countAddedEnt == 0)
                {
                    base.SaveChanges();
                    countAddedEnt += 1;
                }
                foreach (AuditLog x in GetAuditRecordsForChangeInsert(addedEnt[i], currentUsername))
                {
                    this.AuditLogs.Add(x);
                }
            }
            return base.SaveChanges();
        }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        //}

        public interface IDescribableEntity
        {
            // Override this method to provide a description of the entity for audit purposes
            string Describe();
        }

        private List<AuditLog> GetAuditRecordsForChange(DbEntityEntry dbEntry, string userId)
        {
            string costCenter = ConfigurationManager.AppSettings["InstalledCostCenterCode"];
            List<AuditLog> result = new List<AuditLog>();

            DateTime changeTime = DateTime.Now;


            // Get the Table() attribute, if one exists
            //TableAttribute tableAttr = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;

            TableAttribute tableAttr = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), true).SingleOrDefault() as TableAttribute;

            // Get table name (if it has a Table attribute, use that, otherwise get the pluralized name)
            //string tableName = tableAttr != null ? tableAttr.Name : dbEntry.Entity.GetType().Name;
            string tableName = tableAttr != null ? tableAttr.Name : ObjectContext.GetObjectType(dbEntry.Entity.GetType()).Name;

            // Get primary key value (If you have more than one key column, this will need to be adjusted)
            var objectStateEntry = ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager.GetObjectStateEntry(dbEntry.Entity);
            string primaryKey = null;
            try
            {
                primaryKey = objectStateEntry.EntityKey.EntityKeyValues[0].Value.ToString();
            }
            catch (Exception) { }

            if (dbEntry.State == EntityState.Deleted)
            {
                // Same with deletes, do the whole record, and use either the description from Describe() or ToString()
                result.Add(new AuditLog()
                {
                    auditlogid = Guid.NewGuid(),
                    userid = userId,
                    eventdateutc = changeTime,
                    eventtype = "D", // Deleted
                    tablename = tableName,
                    columnname = "*ALL",
                    recordid = primaryKey,
                    newvalue = (dbEntry.OriginalValues.ToObject() is IDescribableEntity) ? (dbEntry.OriginalValues.ToObject() as IDescribableEntity).Describe() : dbEntry.OriginalValues.ToObject().ToString(),
                    costCenterCode = costCenter
                }
                    );
            }
            else if (dbEntry.State == EntityState.Modified)
            {
                foreach (string propertyName in dbEntry.OriginalValues.PropertyNames)
                {
                    // For updates, we only want to capture the columns that actually changed
                    //    if (!object.Equals(dbEntry.OriginalValues.GetValue<object>(propertyName), dbEntry.CurrentValues.GetValue<object>(propertyName)))
                    if (!object.Equals(dbEntry.GetDatabaseValues().GetValue<object>(propertyName), dbEntry.CurrentValues.GetValue<object>(propertyName)) && (propertyName != "DateModified" && propertyName != "DateCreated"))
                    {
                        result.Add(new AuditLog()
                        {
                            auditlogid = Guid.NewGuid(),
                            userid = userId,
                            eventdateutc = changeTime,
                            eventtype = "M",    // Modified
                            tablename = tableName,
                            columnname = propertyName,
                            recordid = primaryKey,
                            originalvalue = dbEntry.GetDatabaseValues().GetValue<object>(propertyName) == null ? null : dbEntry.GetDatabaseValues().GetValue<object>(propertyName).ToString(),
                            newvalue = dbEntry.CurrentValues.GetValue<object>(propertyName) == null ? null : dbEntry.CurrentValues.GetValue<object>(propertyName).ToString(),
                            costCenterCode = costCenter,
                            //datatype = dbEntry.CurrentValues.GetValue<object>(propertyName) == null ? (typeof(DBNull)).FullName : dbEntry.CurrentValues.GetValue<object>(propertyName).GetType().FullName
                        }
                            );
                    }
                }
            }
            // Otherwise, don't do anything, we don't care about Unchanged or Detached entities

            return result;
        }
        private List<AuditLog> GetAuditRecordsForChangeInsert(DbEntityEntry dbEntry, string userId)
        {
            string costCenter = ConfigurationManager.AppSettings["InstalledCostCenterCode"];
            List<AuditLog> result = new List<AuditLog>();

            DateTime changeTime = DateTime.Now;
            // Get the Table() attribute, if one exists
            //TableAttribute tableAttr = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;

            TableAttribute tableAttr = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), true).SingleOrDefault() as TableAttribute;

            // Get table name (if it has a Table attribute, use that, otherwise get the pluralized name)
            string tableName = tableAttr != null ? tableAttr.Name : dbEntry.Entity.GetType().Name;

            // Get primary key value (If you have more than one key column, this will need to be adjusted)
            var objectStateEntry = ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager.GetObjectStateEntry(dbEntry.Entity);
            string primaryKey = null;
            try
            {
                primaryKey = objectStateEntry.EntityKey.EntityKeyValues[0].Value.ToString();
            }
            catch (Exception) { }

            // For Inserts, just add the whole record
            // If the entity implements IDescribableEntity, use the description from Describe(), otherwise use ToString()

            foreach (string propertyName in dbEntry.CurrentValues.PropertyNames)
            {
                if (propertyName != "DateModified" && propertyName != "DateCreated" && propertyName != "UserModified" && propertyName != "UserCreated")
                {
                    result.Add(new AuditLog()
                    {
                        auditlogid = Guid.NewGuid(),
                        userid = userId,
                        eventdateutc = changeTime,
                        eventtype = "A",    // Added
                        tablename = tableName,
                        columnname = propertyName,
                        recordid = primaryKey,
                        newvalue = dbEntry.CurrentValues.GetValue<object>(propertyName) == null ? null : dbEntry.CurrentValues.GetValue<object>(propertyName).ToString(),
                        costCenterCode = costCenter,
                        //datatype = dbEntry.CurrentValues.GetValue<object>(propertyName) == null ? (typeof(DBNull)).FullName : dbEntry.CurrentValues.GetValue<object>(propertyName).GetType().FullName
                    });
                }
            }
            // Otherwise, don't do anything, we don't care about Unchanged or Detached entities

            return result;
        }

    }
}