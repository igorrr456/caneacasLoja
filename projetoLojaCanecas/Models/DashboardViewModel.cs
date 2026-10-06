namespace projetoLojaCanecas.Models
{
    public class DashboardViewModel
    {
        public int Dias { get; set; } = 30;

        // ---- Indicadores do periodo ----
        public decimal Faturamento { get; set; }
        public int TotalVendas { get; set; }
        public int CanecasVendidas { get; set; }
        public decimal TicketMedio { get; set; }

        // ---- Situacao atual da loja ----
        public int PedidosPendentes { get; set; }
        public int PedidosAEnviar { get; set; }
        public int TotalClientes { get; set; }
        public int ClientesNovos { get; set; }
        public int ProdutosAtivos { get; set; }
        public int EstoqueBaixo { get; set; }

        public List<VendaDia> VendasPorDia { get; set; } = new List<VendaDia>();
        public List<ItemRanking> TopProdutos { get; set; } = new List<ItemRanking>();
        public List<ItemRanking> VendasPorCategoria { get; set; } = new List<ItemRanking>();
        public List<ItemRanking> PedidosPorSituacao { get; set; } = new List<ItemRanking>();
        public List<Produto> ProdutosEstoqueBaixo { get; set; } = new List<Produto>();
        public List<Pedido> UltimosPedidos { get; set; } = new List<Pedido>();
    }

    public class VendaDia
    {
        public DateTime Dia { get; set; }
        public int QtdPedidos { get; set; }
        public decimal Valor { get; set; }
    }

    // Linha generica de ranking/grafico (nome + quantidade + valor)
    public class ItemRanking
    {
        public string Nome { get; set; } = string.Empty;
        public string? Imagem { get; set; }
        public int Quantidade { get; set; }
        public decimal Valor { get; set; }
    }
}
