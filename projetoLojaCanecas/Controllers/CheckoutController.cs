using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using projetoLojaCanecas.Autentication;
using projetoLojaCanecas.Data;
using projetoLojaCanecas.Helpers;
using projetoLojaCanecas.Models;

namespace projetoLojaCanecas.Controllers
{
    // Finalizacao da compra: exige cliente logado ([ClienteAuthorize])
    [ClienteAuthorize]
    public class CheckoutController : Controller
    {
        private readonly ProdutoRepositorio produtos = new ProdutoRepositorio();
        private readonly ClienteRepositorio clientes = new ClienteRepositorio();
        private readonly PedidoRepositorio pedidos = new PedidoRepositorio();

        private int ClienteId => HttpContext.Session.GetInt32(SessionKeys.ClienteId)!.Value;

        // GET: /Checkout
        public IActionResult Index()
        {
            var carrinho = CarrinhoHelper.Montar(HttpContext.Session, produtos, out bool alterado);
            if (carrinho.Linhas.Count == 0)
            {
                TempData["Erro"] = "Seu carrinho está vazio.";
                return RedirectToAction("Index", "Carrinho");
            }

            if (alterado)
                TempData["Erro"] = "Seu carrinho foi ajustado: algum item ficou indisponível ou com menos estoque.";

            var cliente = clientes.Obter(ClienteId);
            if (cliente == null)
                return RedirectToAction("Sair", "Conta");

            return View(new CheckoutViewModel
            {
                Carrinho = carrinho,
                EnderecoEntrega = cliente.EnderecoCompleto
            });
        }

        // POST: /Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(CheckoutViewModel model)
        {
            var carrinho = CarrinhoHelper.Montar(HttpContext.Session, produtos, out _);
            if (carrinho.Linhas.Count == 0)
            {
                TempData["Erro"] = "Seu carrinho está vazio.";
                return RedirectToAction("Index", "Carrinho");
            }

            model.Carrinho = carrinho;

            if (!RegrasLoja.FormasPagamento.Contains(model.FormaPagamento))
                ModelState.AddModelError(nameof(model.FormaPagamento), "Escolha uma forma de pagamento válida.");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                int pedidoId = pedidos.FinalizarCompra(
                    ClienteId, model.FormaPagamento, model.EnderecoEntrega.Trim(),
                    CarrinhoHelper.Obter(HttpContext.Session));

                CarrinhoHelper.Limpar(HttpContext.Session);
                return RedirectToAction("Sucesso", new { id = pedidoId });
            }
            catch (MySqlException ex)
            {
                // Ex.: estoque acabou enquanto o cliente decidia. Nada foi gravado (rollback).
                TempData["Erro"] = ex.Message;
                return RedirectToAction("Index", "Carrinho");
            }
        }

        // GET: /Checkout/Sucesso/10
        public IActionResult Sucesso(int id)
        {
            var pedido = pedidos.Obter(id);
            if (pedido == null || pedido.ClienteId != ClienteId)
                return NotFound();

            return View(pedido);
        }
    }
}
