# Paquetes NuGet

Paquetes de la aplicación ASP.NET MVC 5 con login por Microsoft Entra, sobre .NET Framework 4.8. Usa estas versiones exactas.

## Si abres `PaymentMfa.sln`

No instales nada a mano, porque el proyecto ya declara sus 32 paquetes en `packages.config`. Solo tienes que restaurarlos:

1. Haz clic derecho en la solución → **Restore NuGet Packages**.
2. Compila con **Build → Rebuild Solution**.

Si sigue habiendo referencias con un triángulo amarillo, ejecuta esto en **Tools → NuGet Package Manager → Package Manager Console**:

```powershell
Update-Package -Reinstall
```

## Si creas tu propio proyecto

Crea el proyecto con **.NET Framework 4.8** y la plantilla **Empty + MVC**. Después, ejecuta esto en la **Package Manager Console**, en este orden, con tu proyecto seleccionado en **Default project**:

```powershell
Update-Package Microsoft.AspNet.Mvc -Version 5.3.0
Install-Package Microsoft.Owin.Host.SystemWeb -Version 4.2.3
Install-Package Microsoft.Owin.Security.Cookies -Version 4.2.3
Install-Package Microsoft.Owin.Security.OpenIdConnect -Version 4.2.3
Install-Package System.IdentityModel.Tokens.Jwt -Version 8.23.0
Install-Package Microsoft.IdentityModel.Tokens -Version 8.23.0
Install-Package Microsoft.IdentityModel.Protocols.OpenIdConnect -Version 8.23.0
Add-BindingRedirect
```

| Paquete | Para qué sirve |
|---|---|
| `Microsoft.AspNet.Mvc` | ASP.NET MVC 5 |
| `Microsoft.Owin.Host.SystemWeb` | Ejecuta OWIN sobre IIS |
| `Microsoft.Owin.Security.Cookies` | Guarda la sesión en una cookie |
| `Microsoft.Owin.Security.OpenIdConnect` | Hace el login con Entra |
| `System.IdentityModel.Tokens.Jwt` | Lee y valida los tokens |
| `Microsoft.IdentityModel.Tokens` | Valida las firmas de los tokens |
| `Microsoft.IdentityModel.Protocols.OpenIdConnect` | Lee la configuración de Entra |

**No omitas los tres últimos paquetes de la lista de comandos.** Sin ellos, los paquetes OWIN instalan IdentityModel 5.3.0, que tiene una vulnerabilidad moderada ([GHSA-59j7-ghrg-fj52](https://github.com/advisories/GHSA-59j7-ghrg-fj52)).

Después de instalar, revisa que `Views/Web.config` diga `System.Web.Mvc, Version=5.3.0.0`. Si dice otra versión, cámbiala.

## Cómo debe quedar `packages.config`

Los demás paquetes se instalan solos como dependencias. Sirve para comparar: en un proyecto propio puede haber otros paquetes de la plantilla, o versiones algo más recientes de algunas dependencias.

| Paquete | Versión |
|---|---|
| Microsoft.AspNet.Mvc | 5.3.0 |
| Microsoft.AspNet.Razor | 3.3.0 |
| Microsoft.AspNet.WebPages | 3.3.0 |
| Microsoft.Bcl.AsyncInterfaces | 8.0.0 |
| Microsoft.Bcl.Cryptography | 10.0.2 |
| Microsoft.Bcl.TimeProvider | 8.0.1 |
| Microsoft.Extensions.Logging.Abstractions | 2.1.0 |
| Microsoft.IdentityModel.Abstractions | 8.23.0 |
| Microsoft.IdentityModel.JsonWebTokens | 8.23.0 |
| Microsoft.IdentityModel.Logging | 8.23.0 |
| Microsoft.IdentityModel.Protocols | 8.23.0 |
| Microsoft.IdentityModel.Protocols.OpenIdConnect | 8.23.0 |
| Microsoft.IdentityModel.Tokens | 8.23.0 |
| Microsoft.Owin | 4.2.3 |
| Microsoft.Owin.Host.SystemWeb | 4.2.3 |
| Microsoft.Owin.Security | 4.2.3 |
| Microsoft.Owin.Security.Cookies | 4.2.3 |
| Microsoft.Owin.Security.OpenIdConnect | 4.2.3 |
| Microsoft.Web.Infrastructure | 1.0.0.0 |
| Newtonsoft.Json | 13.0.1 |
| Owin | 1.0 |
| System.Buffers | 4.6.1 |
| System.Diagnostics.DiagnosticSource | 6.0.2 |
| System.Formats.Asn1 | 10.0.2 |
| System.IdentityModel.Tokens.Jwt | 8.23.0 |
| System.Memory | 4.6.3 |
| System.Numerics.Vectors | 4.6.1 |
| System.Runtime.CompilerServices.Unsafe | 6.1.2 |
| System.Text.Encodings.Web | 8.0.0 |
| System.Text.Json | 8.0.5 |
| System.Threading.Tasks.Extensions | 4.5.4 |
| System.ValueTuple | 4.6.1 |

## Si algo falla

| Síntoma | Qué revisar |
|---|---|
| `Unable to find package` o `Unable to load the service index` | En **Tools → Options → NuGet Package Manager → Package Sources**, debe estar activa `https://api.nuget.org/v3/index.json`. En una red corporativa, confirma también que el proxy o firewall permita ese sitio |
| Los comandos se aplican al proyecto equivocado | La lista **Default project** de la consola |
| `does not contain any assemblies that are compatible` | Que el proyecto esté en .NET Framework 4.8 |
| `The type or namespace name 'X' could not be found` | Que los paquetes se hayan restaurado. Si persiste, usa `Update-Package -Reinstall` |
| `Could not load file or assembly` al ejecutar | Ejecuta `Add-BindingRedirect` y revisa la sección `<runtime>` de `Web.config` |
| Sigue fallando después de lo anterior | **Tools → NuGet Package Manager → Package Manager Settings → Clear All NuGet Storage**, y restaura de nuevo |
