using MySql.Data.MySqlClient;
using projetoLojaCanecas.Models;

namespace projetoLojaCanecas.Data
{
    public class DashboardRepositorio
    {
        private readonly DataBase db = new DataBase();

        // Monta o painel com os dados dos ultimos "dias" dias
        public DashboardViewModel Montar(int dias)
        {
            var painel = new DashboardViewModel { Dias = dias };

            using var conn = db.GetConnection();

            // 1) Indicadores
            using (var cmd = db.Comando(conn, "sp_dashboard_resumo", ("p_dias", dias)))
            using (var r = cmd.ExecuteReader())
            {
                if (r.Read())
                {
                    painel.Faturamento = r.Dec("faturamento");
                    painel.TotalVendas = r.Int("total_vendas");
                    painel.CanecasVendidas = r.Int("canecas_vendidas");
                    painel.TicketMedio = r.Dec("ticket_medio");
                    painel.PedidosPendentes = r.Int("pedidos_pendentes");
                    painel.PedidosAEnviar = r.Int("pedidos_a_enviar");
                    painel.TotalClientes = r.Int("total_clientes");
                    painel.ClientesNovos = r.Int("clientes_novos");
                    painel.ProdutosAtivos = r.Int("produtos_ativos");
                    painel.EstoqueBaixo = r.Int("estoque_baixo");
                }
            }

            // 2) Vendas por dia (a procedure so devolve dias COM venda; completamos os demais com zero)
            var porDia = new Dictionary<DateTime, VendaDia>();
            using (var cmd = db.Comando(conn, "sp_dashboard_vendas_por_dia", ("p_dias", dias)))
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    var dia = r.Data("dia").Date;
                    porDia[dia] = new VendaDia { Dia = dia, QtdPedidos = r.Int("qtd_pedidos"), Valor = r.Dec("valor") };
                }
            }
            for (int i = dias - 1; i >= 0; i--)
            {
                var dia = DateTime.Today.AddDays(-i);
                painel.VendasPorDia.Add(porDia.TryGetValue(dia, out var v) ? v : new VendaDia { Dia = dia });
            }

            // 3) Rankings
            painel.TopProdutos = LerRanking(conn, "sp_dashboard_top_produtos", dias, "nome", true);
            painel.VendasPorCategoria = LerRanking(conn, "sp_dashboard_vendas_por_categoria", dias, "nome", false);

            // Pedidos por situacao, sempre na ordem do andamento
            var situacoes = LerRanking(conn, "sp_dashboard_pedidos_por_situacao", dias, "situacao", false);
            foreach (var s in RegrasLoja.Situacoes)
                painel.PedidosPorSituacao.Add(situacoes.FirstOrDefault(x => x.Nome == s) ?? new ItemRanking { Nome = s });

            // 4) Estoque baixo
            using (var cmd = db.Comando(conn, "sp_dashboard_estoque_baixo"))
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    painel.ProdutosEstoqueBaixo.Add(new Produto
                    {
                        Id = r.Int("id"),
                        Nome = r.Str("nome"),
                        Imagem = r.StrOuNulo("imagem"),
                        Estoque = r.Int("estoque")
                    });
                }
            }

            return painel;
        }

        private List<ItemRanking> LerRanking(MySqlConnection conn, string procedure, int dias, string colunaNome, bool temImagem)
        {
            var lista = new List<ItemRanking>();
            using var cmd = db.Comando(conn, procedure, ("p_dias", dias));
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                lista.Add(new ItemRanking
                {
                    Nome = r.Str(colunaNome),
                    Imagem = temImagem ? r.StrOuNulo("imagem") : null,
                    Quantidade = r.Int("quantidade"),
                    Valor = r.Dec("valor")
                });
            }
            return lista;
        }
    }
}
