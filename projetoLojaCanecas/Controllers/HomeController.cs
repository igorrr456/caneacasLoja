using Microsoft.AspNetCore.Mvc;
using projetoLojaCanecas.Data;
using projetoLojaCanecas.Models;
using System.Diagnostics;

namespace projetoLojaCanecas.Controllers
{
    // Vitrine publica: qualquer pessoa pode ver as canecas
    public class HomeController : Controller
    {
        private readonly ProdutoRepositorio produtos = new ProdutoRepositorio();
        private readonly CategoriaRepositorio categorias = new CategoriaRepositorio();

        // GET: /?busca=azul&categoriaId=2&ordem=menor
        public IActionResult Index(string? busca, int categoriaId = 0, string? ordem = null)
        {
            var vitrine = new VitrineViewModel
            {
                Busca = busca,
                CategoriaId = categoriaId,
                Ordem = ordem ?? "",
                Categorias = categorias.Listar(null, 1),
                Produtos = produtos.ListarLoja(busca, categoriaId, ordem)
            };

            if (vitrine.SemFiltro)
                vitrine.Destaques = produtos.Destaques();

            return View(vitrine);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
