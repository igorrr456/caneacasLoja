using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using projetoLojaCanecas.Autentication;
using projetoLojaCanecas.Data;
using projetoLojaCanecas.Models;

namespace projetoLojaCanecas.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class CategoriasController : Controller
    {
        private readonly CategoriaRepositorio categorias = new CategoriaRepositorio();

        // GET: /Admin/Categorias?busca=...&status=1
        public IActionResult Index(string? busca, int status = 1)
        {
            ViewBag.Busca = busca;
            ViewBag.Status = status;
            return View(categorias.Listar(busca, status));
        }

        public IActionResult Cadastrar()
        {
            return View(new Categoria());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cadastrar(Categoria categoria)
        {
            if (!ModelState.IsValid)
                return View(categoria);

            try
            {
                categorias.Criar(categoria);
            }
            catch (MySqlException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(categoria);
            }

            TempData["Mensagem"] = "Categoria cadastrada com sucesso!";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var categoria = categorias.Obter(id);
            if (categoria == null)
                return NotFound();

            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Categoria categoria)
        {
            categoria.Id = id;   // o id vem da rota; garante que o formulario reexibido aponte para o registro certo
            if (!ModelState.IsValid)
                return View(categoria);

            try
            {
                categorias.Editar(id, categoria);
            }
            catch (MySqlException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(categoria);
            }

            TempData["Mensagem"] = "Categoria alterada com sucesso!";
            return RedirectToAction("Index");
        }

        // Exclusao logica (status = 0)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                categorias.Excluir(id);
                TempData["Mensagem"] = "Categoria excluída com sucesso!";
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
            categorias.Reativar(id);
            TempData["Mensagem"] = "Categoria reativada com sucesso!";
            return RedirectToAction("Index");
        }
    }
}
