using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using projetoLojaCanecas.Autentication;
using projetoLojaCanecas.Data;
using projetoLojaCanecas.Models;

namespace projetoLojaCanecas.Areas.Admin.Controllers
{
    // Login do painel administrativo: /Admin/Login
    [Area("Admin")]
    public class LoginController : Controller
    {
        private readonly UsuarioRepositorio usuarios = new UsuarioRepositorio();

        // GET: /Admin/Login
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32(SessionKeys.AdminId) != null)
                return RedirectToAction("Index", "Dashboard");

            return View();
        }

        // POST: /Admin/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = usuarios.Login(model.Login, model.Senha);
            if (usuario == null)
            {
                ViewBag.Erro = "Login ou senha inválidos.";
                return View(model);
            }

            HttpContext.Session.SetInt32(SessionKeys.AdminId, usuario.Id);
            HttpContext.Session.SetString(SessionKeys.AdminNome, usuario.Nome);
            HttpContext.Session.SetString(SessionKeys.AdminLogin, usuario.Login);
            HttpContext.Session.SetString(SessionKeys.AdminPerfil, usuario.Perfil);

            return RedirectToAction("Index", "Dashboard");
        }

        // GET: /Admin/Login/Sair
        public IActionResult Sair()
        {
            HttpContext.Session.Remove(SessionKeys.AdminId);
            HttpContext.Session.Remove(SessionKeys.AdminNome);
            HttpContext.Session.Remove(SessionKeys.AdminLogin);
            HttpContext.Session.Remove(SessionKeys.AdminPerfil);
            return RedirectToAction("Index");
        }

        // GET: /Admin/Login/AlterarSenha
        [AdminAuthorize]
        public IActionResult AlterarSenha()
        {
            return View();
        }

        // POST: /Admin/Login/AlterarSenha
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorize]
        public IActionResult AlterarSenha(AlterarSenhaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                usuarios.AlterarSenha(HttpContext.Session.GetInt32(SessionKeys.AdminId)!.Value, model.SenhaAtual, model.NovaSenha);
            }
            catch (MySqlException ex)
            {
                ViewBag.Erro = ex.Message;
                return View(model);
            }

            TempData["Mensagem"] = "Senha alterada com sucesso!";
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
