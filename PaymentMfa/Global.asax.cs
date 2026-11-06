using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;

namespace PaymentMfa
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            // No revela la versión de MVC en las respuestas
            MvcHandler.DisableMvcResponseHeader = true;
            // Exige sesión en toda la aplicación; lo público se marca con [AllowAnonymous]
            GlobalFilters.Filters.Add(new AuthorizeAttribute());
            // Sin esta línea, los formularios con antiforgery fallan al usar estos claims
            AntiForgeryConfig.UniqueClaimTypeIdentifier = "sub";

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
