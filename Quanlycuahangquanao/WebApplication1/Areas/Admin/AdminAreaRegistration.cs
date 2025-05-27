using System.Web.Mvc;

namespace WebApplication1.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Admin";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            // Route dành riêng cho Admin area
            context.MapRoute(
                name: "Admin_default",
                url: "Admin/{controller}/{action}/{id}",
                defaults: new { controller = "HomeAdmin", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "WebApplication1.Areas.Admin.Controllers" }
            );
        }
    }
}
