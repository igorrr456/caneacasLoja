using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using projetoLojaCanecas.Autentication;
using projetoLojaCanecas.Data;
using projetoLojaCanecas.Models;

namespace projetoLojaCanecas.Areas.Admin.Controllers
{
    // Usuarios do painel: somente o Administrador gerencia
    [Area("Admin")]
    [AdminAuthorize(Perfis.Administrador)]
    public class UsuariosController : Controller
    {
        private readonly UsuarioRepositorio usuarios = new UsuarioRepositorio();

        private int LogadoId => HttpContext.Session.GetInt32(SessionKeys.AdminId) ?? 0;

        // GET: /Admin/Usuarios?status=1
        public IActionResult Index(int status = 1)
        {
            ViewBag.Status = status;
            return View(usuarios.Listar(status));
        }

        public IActionResult Cadastrar()
        {
            return View(new Usuario());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cadastrar(Usuario usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.Senha) || usuario.Senha.Length < 6)
                ModelState.AddModelError(nameof(usuario.Senha), "A senha deve ter pelo menos 6 caracteres.");

            if (!Perfis.Todos.Contains(usuario.Perfil))
                ModelState.AddModelError(nameof(usuario.Perfil), "Perfil inválido.");

            if (!ModelState.IsValid)
                return View(usuario);

            try
            {
                usuarios.Criar(usuario);
            }
            catch (MySqlException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(usuario);
            }

            TempData["Mensagem"] = "Usuário cadastrado com sucesso!";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var usuario = usuarios.Obter(id);
            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Usuario usuario)
        {
            usuario.Id = id;   // o id vem da rota; garante que o formulario reexibido aponte para o registro certo
            if (!string.IsNullOrEmpty(usuario.Senha) && usuario.Senha.Length < 6)
                ModelState.AddModelError(nameof(usuario.Senha), "A senha deve ter pelo menos 6 caracteres.");

            if (!Perfis.Todos.Contains(usuario.Perfil))
                ModelState.AddModelError(nameof(usuario.Perfil), "Perfil inválido.");

            if (!ModelState.IsValid)
                return View(usuario);

            try
            {
                usuarios.Editar(id, usuario);

                // se editou a si mesmo, atualiza a sessao
                if (id == LogadoId)
                {
                    HttpContext.Session.SetString(SessionKeys.AdminNome, usuario.Nome.Trim());
                    HttpContext.Session.SetString(SessionKeys.AdminLogin, usuario.Login.Trim());
                    HttpContext.Session.SetString(SessionKeys.AdminPerfil, usuario.Perfil);
                }
            }
            catch (MySqlException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(usuario);
            }

            TempData["Mensagem"] = "Usuário alterado com sucesso!";
            return RedirectToAction("Index");
        }

        // Exclusao logica (status = 0)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (id == LogadoId)
            {
                TempData["Erro"] = "Você não pode excluir o próprio usuário.";
                return RedirectToAction("Index");
            }

            try
            {
                usuarios.Excluir(id);
                TempData["Mensagem"] = "Usuário excluído com sucesso!";
            }
            catch (MySqlException ex)
            {
                TempData["Erro"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reativar(int id)
        {
            usuarios.Reativar(id);
            TempData["Mensagem"] = "Usuário reativado com sucesso!";
            return RedirectToAction("Index");
        }
    }
}
