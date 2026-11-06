using System.Web.Mvc;

namespace PaymentMfa.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        public ActionResult Autenticacion() => View();
    }
}
