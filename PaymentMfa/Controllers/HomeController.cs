using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;

namespace PaymentMfa.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index() => View();

        // Protegida por el AuthorizeAttribute global de Global.asax.cs
        public async Task<ActionResult> Privado()
        {
            var usuario = (ClaimsPrincipal)User;
            var resultado = await HttpContext.GetOwinContext().Authentication.AuthenticateAsync("Cookies");

            ViewBag.Nombre = usuario.FindFirst("name")?.Value ?? "(sin nombre)";
            ViewBag.Oid = usuario.FindFirst("oid")?.Value ?? "(sin oid)";
            ViewBag.Tid = usuario.FindFirst("tid")?.Value ?? "(sin tid)";
            ViewBag.Amr = usuario.FindFirst("amr")?.Value ?? "(el token no incluye amr)";
            ViewBag.Expira = resultado?.Properties?.ExpiresUtc?.UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss") + " UTC";
            ViewBag.Claims = usuario.Claims.Select(c => c.Type + " = " + c.Value).ToList();
            return View();
        }
    }
}
