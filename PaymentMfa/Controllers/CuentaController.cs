using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;

namespace PaymentMfa.Controllers
{
    [AllowAnonymous]
    public class CuentaController : Controller
    {
        public void Entrar(string returnUrl = "/Home/Privado") => Retar("login", returnUrl);
        public void Registrarse() => Retar("create", "/Home/Privado");

        private void Retar(string prompt, string returnUrl)
        {
            var props = new AuthenticationProperties
            {
                // Evita redirigir a sitios externos (open redirect)
                RedirectUri = Url.IsLocalUrl(returnUrl) ? returnUrl : "/"
            };
            props.Dictionary["prompt"] = prompt;
            HttpContext.GetOwinContext().Authentication.Challenge(
                props, OpenIdConnectAuthenticationDefaults.AuthenticationType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void Salir()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(
                OpenIdConnectAuthenticationDefaults.AuthenticationType,
                CookieAuthenticationDefaults.AuthenticationType);
        }
    }
}
