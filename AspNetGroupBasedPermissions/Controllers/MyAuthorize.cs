using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Diagnostics.CodeAnalysis;
using System.Security.Principal;

namespace AspNetGroupBasedPermissions
{

    // The left bit shifting gives the values 1, 2, 4, 8, 16 and so on.
    [Flags]
    public enum Roles
    {
        Admin = 1,
        User = 1 << 1
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string adminGroupName;

        private readonly string userGroupName;

        public CustomAuthorizeAttribute()
            : this("Admin", "User")
    
    //        : this("Domain Admins", "Domain Users")
        {
        }

        private CustomAuthorizeAttribute(string adminGroupName, string userGroupName)
        {
            this.adminGroupName = adminGroupName;
            this.userGroupName = userGroupName;
        }

        /// <summary>
        /// Gets or sets the allowed roles.
        /// </summary>
        public new Roles Roles { get; set; }
    
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return false;
            }

            var usersRoles = this.GetUsersRoles(httpContext.User);

            return this.Roles == 0 || usersRoles.Any(role => (this.Roles & role) == role);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            filterContext.Result = new ViewResult { ViewName = "NotAuthorized" };
        }

        private IEnumerable<Roles> GetUsersRoles(IPrincipal principal)
        {
            var roles = new List<Roles>();

            if (principal.IsInRole(this.adminGroupName))
            {
                roles.Add(Roles.Admin);
            }

            if (principal.IsInRole(this.userGroupName))
            {
                roles.Add(Roles.User);
            }

            return roles;
        }
    }

}