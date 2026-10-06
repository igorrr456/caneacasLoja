using Microsoft.AspNetCore.Mvc;
using projetoLojaCanecas.Data;
using projetoLojaCanecas.Helpers;
using projetoLojaCanecas.Models;

namespace projetoLojaCanecas.Controllers
{
    // O carrinho pode ser usado SEM login. So o checkout exige cliente logado.
    public class CarrinhoController : Controller
    {
        private const int MaximoPorItem = 99;
        private readonly ProdutoRepositorio produtos = new ProdutoRepositorio();

        // GET: /Carrinho
        public IActionResult Index()
        {
            var carrinho = CarrinhoHelper.Montar(HttpContext.Session, produtos, out bool alterado);

            if (alterado)
                TempData["Erro"] = "Seu carrinho foi ajustado: algum item ficou indisponível ou com menos estoque.";

            return View(carrinho);
        }

        // POST: /Carrinho/Adicionar   (botoes "Comprar" e "Adicionar ao carrinho")
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Adicionar(int id, int qtd = 1, bool comprarAgora = false, string? returnUrl = null)
        {
            var produto = produtos.Obter(id);
            if (produto == null || produto.Status != 1)
            {
                TempData["Erro"] = "Produto não encontrado.";
                return RedirectToAction("Index", "Home");
            }

            if (qtd < 1) qtd = 1;

            if (!produto.Disponivel)
            {
                TempData["Erro"] = $"\"{produto.Nome}\" está esgotada no momento.";
                return VoltarPara(returnUrl, id);
            }

            var itens = CarrinhoHelper.Obter(HttpContext.Session);
            var item = itens.FirstOrDefault(i => i.ProdutoId == id);

            int limite = Math.Min(produto.Estoque, MaximoPorItem);
            int novaQtd = (item?.Quantidade ?? 0) + qtd;
            if (novaQtd > limite)
            {
                novaQtd = limite;
                TempData["Erro"] = $"Só temos {produto.Estoque} unidade(s) de \"{produto.Nome}\" em estoque.";
            }
            else
            {
                TempData["Mensagem"] = $"\"{produto.Nome}\" foi adicionada ao carrinho.";
            }

            if (item == null)
                itens.Add(new CarrinhoItem { ProdutoId = id, Quantidade = novaQtd });
            else
                item.Quantidade = novaQtd;

            CarrinhoHelper.Salvar(HttpContext.Session, itens);

            if (comprarAgora)
                return RedirectToAction("Index");

            return VoltarPara(returnUrl, id);
        }

        // POST: /Carrinho/Atualizar   (muda a quantidade; 0 remove)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Atualizar(int id, int qtd)
        {
            var itens = CarrinhoHelper.Obter(HttpContext.Session);
            var item = itens.FirstOrDefault(i => i.ProdutoId == id);

            if (item != null)
            {
                if (qtd <= 0)
                {
                    itens.Remove(item);
                }
                else
                {
                    var produto = produtos.Obter(id);
                    int limite = Math.Min(produto?.Estoque ?? 0, MaximoPorItem);
                    if (qtd > limite)
                    {
                        qtd = limite;
                        TempData["Erro"] = $"Só temos {limite} unidade(s) desse produto em estoque.";
                    }
                    item.Quantidade = qtd;
                }
                CarrinhoHelper.Salvar(HttpContext.Session, itens);
            }

            return RedirectToAction("Index");
        }

        // POST: /Carrinho/Remover
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remover(int id)
        {
            var itens = CarrinhoHelper.Obter(HttpContext.Session);
            itens.RemoveAll(i => i.ProdutoId == id);
            CarrinhoHelper.Salvar(HttpContext.Session, itens);
            return RedirectToAction("Index");
        }

        // POST: /Carrinho/Limpar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Limpar()
        {
            CarrinhoHelper.Limpar(HttpContext.Session);
            return RedirectToAction("Index");
        }

        // Volta para a pagina de onde o cliente veio (so enderecos do proprio site)
        private IActionResult VoltarPara(string? returnUrl, int produtoId)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Detalhes", "Produtos", new { id = produtoId });
        }
    }
}
