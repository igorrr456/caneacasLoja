using Microsoft.AspNetCore.Mvc;
using projetoLojaCanecas.Autentication;
using projetoLojaCanecas.Data;

namespace projetoLojaCanecas.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class DashboardController : Controller
    {
        private readonly DashboardRepositorio dashboard = new DashboardRepositorio();
        private readonly PedidoRepositorio pedidos = new PedidoRepositorio();

        // GET: /Admin?dias=30   (7, 30 ou 90 dias)
        public IActionResult Index(int dias = 30)
        {
            if (dias != 7 && dias != 30 && dias != 90)
                dias = 30;

            var painel = dashboard.Montar(dias);
            painel.UltimosPedidos = pedidos.UltimosPedidos();
            return View(painel);
        }
    }
}
