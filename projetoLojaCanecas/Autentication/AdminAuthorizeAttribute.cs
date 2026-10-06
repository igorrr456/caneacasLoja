using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace projetoLojaCanecas.Autentication
{
    // Uso:
    //   [AdminAuthorize]                      -> qualquer usuario do painel logado
    //   [AdminAuthorize(Perfis.Administrador)] -> so esse perfil
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string[] _perfis;

        public AdminAuthorizeAttribute(params string[] perfis)
        {
            _perfis = perfis;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;

            // 1) Nao esta logado no painel -> login do painel
            if (session.GetInt32(SessionKeys.AdminId) == null)
            {
                context.Result = new RedirectToActionResult("Index", "Login", new { area = "Admin" });
                return;
            }

            // 2) Esta logado, mas o perfil nao tem permissao
            if (_perfis.Length > 0)
            {
                var perfil = session.GetString(SessionKeys.AdminPerfil);
                if (perfil == null || !_perfis.Contains(perfil))
                {
                    if (context.Controller is Controller c)
                        c.TempData["Erro"] = "Você não tem permissão para acessar esta página.";

                    context.Result = new RedirectToActionResult("Index", "Dashboard", new { area = "Admin" });
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
