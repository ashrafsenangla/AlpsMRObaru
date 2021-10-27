using System.Web.Mvc;

namespace AspNetGroupBasedPermissions.Areas.Part
{
    public class PartAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Part";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Part_default",
                "Part/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}