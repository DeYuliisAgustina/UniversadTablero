using System.Web;
using System.Web.Mvc;

namespace UniversadTablero.Service
{
    public class ValidarSesionAtributte : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (HttpContext.Current.Session["Usuario"] == null)
            {
                filterContext.Result = new RedirectResult("~/Acceso/Login");
            }

            base.OnActionExecuted(filterContext);
        }
    }
}