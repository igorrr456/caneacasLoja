using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using projetoLojaCanecas.Autentication;
using projetoLojaCanecas.Data;

namespace projetoLojaCanecas.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class PedidosController : Controller
    {
        private readonly PedidoRepositorio pedidos = new PedidoRepositorio();

        // GET: /Admin/Pedidos?busca=maria&situacao=Pago&status=1
        public IActionResult Index(string? busca, string? situacao, int status = 1)
        {
            ViewBag.Busca = busca;
            ViewBag.Situacao = situacao ?? "";
            ViewBag.Status = status;
            return View(pedidos.Listar(busca, situacao, status));
        }

        // GET: /Admin/Pedidos/Detalhes/10
        public IActionResult Detalhes(int id)
        {
            var pedido = pedidos.Obter(id);
            if (pedido == null)
                return NotFound();

            return View(pedido);
        }

        // POST: avanca o pedido (Pago, Enviado, Entregue) ou cancela
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AlterarSituacao(int id, string situacao)
        {
            try
            {
                pedidos.AlterarSituacao(id, situacao);
                TempData["Mensagem"] = $"Pedido #{id} atualizado para \"{situacao}\".";
            }
            catch (MySqlException ex)
            {
                TempData["Erro"] = ex.Message;
            }
            return RedirectToAction("Detalhes", new { id });
        }

        // Exclusao logica (status = 0)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            pedidos.Excluir(id);
            TempData["Mensagem"] = $"Pedido #{id} excluído.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reativar(int id)
        {
            pedidos.Reativar(id);
            TempData["Mensagem"] = $"Pedido #{id} reativado.";
            return RedirectToAction("Index");
        }
    }
}
