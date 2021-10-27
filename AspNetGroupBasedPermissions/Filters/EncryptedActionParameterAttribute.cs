using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using WebMatrix.WebData;
using Core.Entities.Data;
using System.Security.Cryptography;
using System.IO;
using AspNetGroupBasedPermissions.Helpers;


namespace AspNetGroupBasedPermissions.Filters
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class EncryptedActionParameterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            Dictionary<string, object> decryptedParameters = new Dictionary<string, object>();
            if (HttpContext.Current.Request.QueryString.Get("q") != null)
            {
                string encryptedQueryString = HttpContext.Current.Request.QueryString.Get("q");
                string decrptedString = EncryptionHelper.Decrypt(encryptedQueryString.ToString());
                string[] paramsArrs = decrptedString.Split('?');

                for (int i = 0; i < paramsArrs.Length; i++)
                {
                    string[] paramArr = paramsArrs[i].Split('=');
               

                    int value;
                    if (int.TryParse(paramArr[1], out value))
                    {
                        decryptedParameters.Add(paramArr[0], Convert.ToInt32(paramArr[1]));
                    }
                    else
                    {
                        decryptedParameters.Add(paramArr[0], Convert.ToString(paramArr[1]));
                    }
                                     
                }
            }
            for (int i = 0; i < decryptedParameters.Count; i++)
            {
                filterContext.ActionParameters[decryptedParameters.Keys.ElementAt(i)] = decryptedParameters.Values.ElementAt(i);
            }
            base.OnActionExecuting(filterContext);

        }

    }
}
