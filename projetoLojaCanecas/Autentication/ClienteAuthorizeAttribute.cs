using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace projetoLojaCanecas.Autentication
{
    // Uso: [ClienteAuthorize] -> so deixa entrar quem esta logado como CLIENTE.
    // Se nao estiver, vai para o login e depois volta para a pagina que queria (returnUrl).
    public class ClienteAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var http = context.HttpContext;

            if (http.Session.GetInt32(SessionKeys.ClienteId) == null)
            {
                if (context.Controller is Controller c)
                    c.TempData["Erro"] = "Entre na sua conta (ou cadastre-se) para continuar.";

                // So volta automaticamente para paginas acessadas por GET
                string? voltar = http.Request.Method == "GET"
                    ? http.Request.Path + http.Request.QueryString
                    : null;

                context.Result = new RedirectToActionResult("Login", "Conta", new { returnUrl = voltar, area = "" });
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
