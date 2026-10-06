using Microsoft.AspNetCore.Mvc;
using projetoLojaCanecas.Data;

namespace projetoLojaCanecas.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly ProdutoRepositorio produtos = new ProdutoRepositorio();

        // GET: /Produtos/Detalhes/5
        public IActionResult Detalhes(int id)
        {
            var produto = produtos.Obter(id);
            if (produto == null || produto.Status != 1)
                return NotFound();

            ViewBag.Relacionados = produtos.Relacionados(produto.Id, produto.CategoriaId);
            return View(produto);
        }
    }
}
