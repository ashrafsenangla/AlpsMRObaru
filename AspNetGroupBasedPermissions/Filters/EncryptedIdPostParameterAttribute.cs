using AspNetGroupBasedPermissions.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AspNetGroupBasedPermissions.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class EncryptedIdPostParameterAttribute : ActionFilterAttribute
    {
        private static string encryptedIdParamName = "id";
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var encryptedId = filterContext.ActionParameters[encryptedIdParamName];
            string decryptedId = EncryptionHelper.Decrypt(encryptedId.ToString());
            filterContext.ActionParameters[encryptedIdParamName] = decryptedId;
            base.OnActionExecuting(filterContext);

        }
    }
}