using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using projetoLojaCanecas.Autentication;
using projetoLojaCanecas.Data;

namespace projetoLojaCanecas.Controllers
{
    // "Minhas compras": o cliente so enxerga os proprios pedidos
    [ClienteAuthorize]
    public class PedidosController : Controller
    {
        private readonly PedidoRepositorio pedidos = new PedidoRepositorio();

        private int ClienteId => HttpContext.Session.GetInt32(SessionKeys.ClienteId)!.Value;

        // GET: /Pedidos
        public IActionResult Index()
        {
            return View(pedidos.ListarDoCliente(ClienteId));
        }

        // GET: /Pedidos/Detalhes/10
        public IActionResult Detalhes(int id)
        {
            var pedido = pedidos.Obter(id);

            // pedido de outro cliente (ou excluido) = nao encontrado
            if (pedido == null || pedido.ClienteId != ClienteId || pedido.Status != 1)
                return NotFound();

            return View(pedido);
        }

        // POST: /Pedidos/Cancelar/10  (so enquanto estiver Pendente)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancelar(int id)
        {
            try
            {
                pedidos.CancelarDoCliente(id, ClienteId);
                TempData["Mensagem"] = $"Pedido #{id} cancelado.";
            }
            catch (MySqlException ex)
            {
                TempData["Erro"] = ex.Message;
            }
            return RedirectToAction("Detalhes", new { id });
        }
    }
}
