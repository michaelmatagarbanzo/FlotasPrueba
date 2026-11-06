using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using PaymentMfa.Models;

namespace PaymentMfa.Controllers
{
    public class HomeController : Controller
    {
        private const string SinDato = "—";
        private static readonly string[] ClaimsDeFecha = { "exp", "iat", "nbf", "auth_time" };

        [AllowAnonymous]
        public ActionResult Index() => View();

        // Protegida por el AuthorizeAttribute global de Global.asax.cs
        public async Task<ActionResult> Dashboard()
        {
            // Evita que el navegador muestre estos datos con el botón Atrás después de cerrar sesión
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            var usuario = (ClaimsPrincipal)User;
            var sesion = await HttpContext.GetOwinContext().Authentication.AuthenticateAsync("Cookies");
            var expira = sesion?.Properties?.ExpiresUtc;

            var tid = Valor(usuario, "tid");
            var aud = Valor(usuario, "aud");
            var nombreUsuario = Valor(usuario, "preferred_username") ?? Valor(usuario, "email");
            var metodos = usuario.FindAll("amr").Select(c => c.Value).ToList();

            return View(new DashboardModelo
            {
                Nombre = Valor(usuario, "name") ?? nombreUsuario ?? "usuario",
                Usuario = nombreUsuario ?? SinDato,
                Oid = Valor(usuario, "oid") ?? SinDato,
                InicioSesion = FormatearFecha(Valor(usuario, "iat")),
                SesionExpira = expira.HasValue ? FormatearFecha(expira.Value) : SinDato,
                Metodos = metodos.Any()
                    ? string.Join(", ", metodos)
                    : "El token no los informa",
                Verificaciones = new List<Verificacion>
                {
                    Comprobar("Sesión iniciada en Entra", nombreUsuario ?? SinDato, usuario.Identity.IsAuthenticated),
                    VerificacionMfa(metodos),
                    Comprobar("Tenant correcto", "tid: " + (tid ?? SinDato), tid == Configuracion("Entra:TenantId")),
                    Comprobar("Token emitido para esta app", "aud: " + (aud ?? SinDato), aud == Configuracion("Entra:ClientId")),
                    Comprobar(
                        "Sesión vigente",
                        "expira: " + (expira.HasValue ? FormatearFecha(expira.Value) : SinDato),
                        expira.HasValue && expira.Value > DateTimeOffset.UtcNow)
                },
                Claims = usuario.Claims
                    .Select(c => new DatoClaim
                    {
                        Tipo = c.Type,
                        Valor = ClaimsDeFecha.Contains(c.Type) ? FormatearFecha(c.Value) : c.Value
                    })
                    .ToList()
            });
        }

        // External ID no incluye amr en el token, así que la aplicación no puede comprobar el MFA.
        // Solo se marca como aprobado si el token lo demuestra.
        private static Verificacion VerificacionMfa(List<string> metodos)
        {
            if (metodos.Contains("mfa"))
            {
                return new Verificacion
                {
                    Nombre = "MFA completado",
                    Detalle = "amr: " + string.Join(", ", metodos),
                    Estado = "si",
                    Etiqueta = "Pasa"
                };
            }

            return new Verificacion
            {
                Nombre = "MFA exigido por política",
                Detalle = "Lo exige Acceso Condicional. El token no permite comprobarlo; revísalo en los Sign-in logs de Entra.",
                Estado = "info",
                Etiqueta = "Política"
            };
        }

        private static Verificacion Comprobar(string nombre, string detalle, bool ok)
        {
            return new Verificacion
            {
                Nombre = nombre,
                Detalle = detalle,
                Estado = ok ? "si" : "no",
                Etiqueta = ok ? "Pasa" : "Falla"
            };
        }

        private static string Valor(ClaimsPrincipal usuario, string tipo)
        {
            return usuario.FindFirst(tipo)?.Value;
        }

        private static string Configuracion(string clave)
        {
            return ConfigurationManager.AppSettings[clave]?.Trim();
        }

        private static string FormatearFecha(string segundosUnix)
        {
            return long.TryParse(segundosUnix, out var segundos)
                ? FormatearFecha(DateTimeOffset.FromUnixTimeSeconds(segundos))
                : segundosUnix ?? SinDato;
        }

        private static string FormatearFecha(DateTimeOffset fecha)
        {
            return fecha.UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss") + " UTC";
        }
    }
}
