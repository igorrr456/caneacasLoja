using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using projetoLojaCanecas.Autentication;
using projetoLojaCanecas.Data;
using projetoLojaCanecas.Models;

namespace projetoLojaCanecas.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class ProdutosController : Controller
    {
        private static readonly string[] ExtensoesPermitidas = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        private const long TamanhoMaximo = 2 * 1024 * 1024;   // 2 MB

        private readonly ProdutoRepositorio produtos = new ProdutoRepositorio();
        private readonly CategoriaRepositorio categorias = new CategoriaRepositorio();
        private readonly IWebHostEnvironment env;

        public ProdutosController(IWebHostEnvironment env)
        {
            this.env = env;
        }

        // GET: /Admin/Produtos?busca=azul&status=1
        public IActionResult Index(string? busca, int status = 1)
        {
            ViewBag.Busca = busca;
            ViewBag.Status = status;
            return View(produtos.Listar(busca, status));
        }

        public IActionResult Cadastrar()
        {
            CarregarCombos();
            return View(new Produto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cadastrar(Produto produto)
        {
            string? erroFoto = ValidarFoto(produto.ImagemArquivo);
            if (erroFoto != null)
                ModelState.AddModelError(nameof(produto.ImagemArquivo), erroFoto);

            if (!ModelState.IsValid)
            {
                CarregarCombos();
                return View(produto);
            }

            try
            {
                string? arquivo = SalvarFoto(produto.ImagemArquivo);
                produtos.Criar(produto, arquivo);
            }
            catch (MySqlException ex)
            {
                ModelState.AddModelError("", ex.Message);
                CarregarCombos();
                return View(produto);
            }

            TempData["Mensagem"] = "Caneca cadastrada com sucesso!";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var produto = produtos.Obter(id);
            if (produto == null)
                return NotFound();

            CarregarCombos();
            return View(produto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Produto produto)
        {
            produto.Id = id;   // o id vem da rota; garante que o formulario reexibido aponte para o registro certo
            string? erroFoto = ValidarFoto(produto.ImagemArquivo);
            if (erroFoto != null)
                ModelState.AddModelError(nameof(produto.ImagemArquivo), erroFoto);

            if (!ModelState.IsValid)
            {
                // a foto atual nao vem no formulario: busca de novo para continuar mostrando
                produto.Imagem = produtos.Obter(id)?.Imagem;
                CarregarCombos();
                return View(produto);
            }

            try
            {
                // sem foto nova -> null -> a procedure mantem a foto atual
                string? arquivo = SalvarFoto(produto.ImagemArquivo);
                produtos.Editar(id, produto, arquivo);
            }
            catch (MySqlException ex)
            {
                ModelState.AddModelError("", ex.Message);
                produto.Imagem = produtos.Obter(id)?.Imagem;
                CarregarCombos();
                return View(produto);
            }

            TempData["Mensagem"] = "Caneca alterada com sucesso!";
            return RedirectToAction("Index");
        }

        // Exclusao logica (status = 0): a caneca some da loja, mas continua nos pedidos antigos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            produtos.Excluir(id);
            TempData["Mensagem"] = "Caneca excluída com sucesso!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reativar(int id)
        {
            try
            {
                produtos.Reativar(id);
                TempData["Mensagem"] = "Caneca reativada com sucesso!";
            }
            catch (MySqlException ex)
            {
                TempData["Erro"] = ex.Message + " Reative a categoria antes.";
                return RedirectToAction("Index", new { status = 0 });
            }
            return RedirectToAction("Index");
        }

        // ------------------------------------------------------------------
        private void CarregarCombos()
        {
            ViewBag.Categorias = new SelectList(categorias.Listar(null, 1), "Id", "Nome");
        }

        // Confere extensao e tamanho da foto enviada (null = ok / nenhuma foto)
        private static string? ValidarFoto(IFormFile? arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
                return null;

            string ext = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
            if (!ExtensoesPermitidas.Contains(ext))
                return "Envie uma imagem JPG, PNG, WEBP ou GIF.";

            if (arquivo.Length > TamanhoMaximo)
                return "A imagem deve ter no máximo 2 MB.";

            return null;
        }

        // Grava a foto em wwwroot/img/produtos com nome unico e devolve o nome do arquivo
        private string? SalvarFoto(IFormFile? arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
                return null;

            string pasta = Path.Combine(env.WebRootPath, "img", "produtos");
            Directory.CreateDirectory(pasta);

            string nome = Guid.NewGuid().ToString("N") + Path.GetExtension(arquivo.FileName).ToLowerInvariant();
            using var stream = new FileStream(Path.Combine(pasta, nome), FileMode.Create);
            arquivo.CopyTo(stream);
            return nome;
        }
    }
}
