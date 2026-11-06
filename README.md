# PaymentMfa: proyecto de ejemplo

Aplicación ASP.NET MVC 5 sobre .NET Framework 4.8 con login por Microsoft Entra External ID. Es el resultado de seguir `GUIA-DESARROLLO-APP.md`, y sirve para **comparar** cuando algo no funcione. Para aprender, construye tu propio proyecto con la guía y consulta este solo como referencia.

**Estado:** compila contra .NET Framework 4.8, y sus paquetes no tienen vulnerabilidades conocidas. Aún no se ha abierto en Visual Studio ni se ha ejecutado en IIS o contra Entra.

## Requisitos

En **Visual Studio Installer → Modify**, Visual Studio 2022 con:

- La carga de trabajo **ASP.NET and web development**
- Sus componentes **.NET Framework project and item templates** y **.NET Framework 4.8 development tools**

Si al abrir la solución aparece *Unsupported* junto a `PaymentMfa.csproj`, normalmente falta la carga de trabajo. Para comprobarlo, ejecuta esto en PowerShell. Si no devuelve nada, no está instalada:

```powershell
& "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -products * -requires Microsoft.VisualStudio.Workload.NetWeb -property displayName
```

## Ejecutar

1. Abre `PaymentMfa.sln`. Si no se descargan los paquetes, usa **clic derecho en la solución → Restore NuGet Packages**.
2. Copia `PaymentMfa/secretos.config.example` como `PaymentMfa/secretos.config` y completa `Entra:ClientId` y `Entra:ClientSecret`. Pide esos valores a quien administra Entra; **ese archivo no se sube a git**.
3. Confirma con quien administra Entra que esté registrada esta URL, en la plataforma **Web**: `https://localhost:44380/signin-oidc`
4. Presiona **F5** y acepta el certificado de desarrollo de IIS Express si te lo pide.

Las pruebas y la solución de errores están en las secciones 8 y "Si algo falla" de la guía.

## Qué mirar

| Archivo | Qué hace |
|---|---|
| `App_Start/Startup.cs` | Configura la cookie de sesión y el login con Entra |
| `Global.asax.cs` | Exige sesión en toda la aplicación |
| `Controllers/CuentaController.cs` | Entrar, registrarse y salir |
| `Controllers/HomeController.cs` | Página pública y página privada, que muestra los claims |
| `Web.config` | Encabezados de seguridad, cookies y redirecciones de ensamblados |

La página privada muestra los claims solo porque esto es un ejemplo. No lo hagas en producción.
