using Microsoft.AspNetCore.Mvc;
using projetoLojaCanecas.Autentication;
using projetoLojaCanecas.Data;

namespace projetoLojaCanecas.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class ClientesController : Controller
    {
        private readonly ClienteRepositorio clientes = new ClienteRepositorio();
        private readonly PedidoRepositorio pedidos = new PedidoRepositorio();

        // GET: /Admin/Clientes?busca=maria&status=1
        public IActionResult Index(string? busca, int status = 1)
        {
            ViewBag.Busca = busca;
            ViewBag.Status = status;
            return View(clientes.Listar(busca, status));
        }

        // GET: /Admin/Clientes/Detalhes/3  (dados + historico de compras)
        public IActionResult Detalhes(int id)
        {
            var cliente = clientes.Obter(id);
            if (cliente == null)
                return NotFound();

            ViewBag.Pedidos = pedidos.ListarDoCliente(id);
            return View(cliente);
        }

        // Exclusao logica: o cliente nao consegue mais entrar, mas o historico de pedidos fica
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            clientes.Excluir(id);
            TempData["Mensagem"] = "Cliente excluído com sucesso!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reativar(int id)
        {
            clientes.Reativar(id);
            TempData["Mensagem"] = "Cliente reativado com sucesso!";
            return RedirectToAction("Index");
        }
    }
}
