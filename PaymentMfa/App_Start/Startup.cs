using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;

[assembly: OwinStartup(typeof(PaymentMfa.Startup))]

namespace PaymentMfa
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var tenantId = Requerido("Entra:TenantId");
            var subdominio = Requerido("Entra:Subdominio");

            // Conserva los nombres originales de los claims: "oid", "name", "sub"
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                CookieName = ".PaymentMfa.Auth",
                CookieHttpOnly = true,
                CookieSecure = CookieSecureOption.Always,
                CookieSameSite = SameSiteMode.Lax,
                ExpireTimeSpan = TimeSpan.FromMinutes(30),
                SlidingExpiration = false,
                // Evita que IIS pierda cookies de OWIN
                CookieManager = new SystemWebCookieManager()
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = Requerido("Entra:ClientId"),
                ClientSecret = Requerido("Entra:ClientSecret"),
                Authority = $"https://{subdominio}.ciamlogin.com/{tenantId}/v2.0",
                RedirectUri = Requerido("Entra:RedirectUri"),
                PostLogoutRedirectUri = Requerido("Entra:PostLogoutRedirectUri"),
                ResponseType = OpenIdConnectResponseType.Code,
                ResponseMode = OpenIdConnectResponseMode.Query,
                RedeemCode = true,
                UsePkce = true,
                // La duración de la sesión la define la cookie, no el token
                UseTokenLifetime = false,
                Scope = OpenIdConnectScope.OpenIdProfile,
                CookieManager = new SystemWebCookieManager(),
                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    // El issuer usa el tenant ID en el host, no el subdominio
                    ValidIssuer = $"https://{tenantId}.ciamlogin.com/{tenantId}/v2.0"
                },
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    RedirectToIdentityProvider = n =>
                    {
                        if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.Authentication)
                        {
                            string prompt = null;
                            n.OwinContext.Authentication.AuthenticationResponseChallenge?
                                .Properties?.Dictionary.TryGetValue("prompt", out prompt);
                            // login: obliga a autenticarse de nuevo. create: abre el registro.
                            n.ProtocolMessage.Prompt = prompt == "create" ? "create" : "login";
                        }
                        return Task.CompletedTask;
                    },
                    AuthenticationFailed = n =>
                    {
                        // El detalle queda en el log; al usuario solo se le muestra un mensaje genérico.
                        System.Diagnostics.Trace.TraceError("Error de autenticación: {0}", n.Exception);
                        n.HandleResponse();
                        n.Response.Redirect("/Error/Autenticacion");
                        return Task.CompletedTask;
                    }
                }
            });
        }

        private static string Requerido(string clave)
        {
            var valor = ConfigurationManager.AppSettings[clave]?.Trim();
            if (string.IsNullOrEmpty(valor))
            {
                throw new ConfigurationErrorsException($"Falta la configuración '{clave}'. Agrégala en Azure: Configuration > Application settings.");
            }
            return valor;
        }
    }
}
