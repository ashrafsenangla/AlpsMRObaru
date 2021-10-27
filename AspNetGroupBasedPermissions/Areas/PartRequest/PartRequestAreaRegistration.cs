using System.Web.Mvc;

namespace AspNetGroupBasedPermissions.Areas.PartRequest
{
    public class PartRequestAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "PartRequest";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "PartRequest_default",
                "PartRequest/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}