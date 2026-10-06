using MySql.Data.MySqlClient;
using projetoLojaCanecas.Models;

namespace projetoLojaCanecas.Data
{
    public class PedidoRepositorio
    {
        private readonly DataBase db = new DataBase();

        // Fecha a compra inteira numa TRANSACAO: ou grava tudo (pedido + itens + baixa de estoque)
        // ou nao grava nada. Se faltar estoque, a procedure dispara erro e damos Rollback.
        // Devolve o numero do pedido.
        public int FinalizarCompra(int clienteId, string formaPagamento, string endereco, List<CarrinhoItem> itens)
        {
            using var conn = db.GetConnection();
            using var tx = conn.BeginTransaction();

            try
            {
                int pedidoId;
                using (var cmd = db.Comando(conn, "sp_pedido_criar",
                    ("p_cliente_id", clienteId), ("p_forma_pagamento", formaPagamento), ("p_endereco", endereco)))
                {
                    cmd.Transaction = tx;
                    pedidoId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                foreach (var item in itens)
                {
                    using var cmd = db.Comando(conn, "sp_pedido_item_adicionar",
                        ("p_pedido_id", pedidoId), ("p_produto_id", item.ProdutoId), ("p_quantidade", item.Quantidade));
                    cmd.Transaction = tx;
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = db.Comando(conn, "sp_pedido_finalizar", ("p_pedido_id", pedidoId)))
                {
                    cmd.Transaction = tx;
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
                return pedidoId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        // "Minhas compras" do cliente
        public List<Pedido> ListarDoCliente(int clienteId)
        {
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, "sp_pedido_listar_cliente", ("p_cliente_id", clienteId));
            return LerLista(cmd);
        }

        // Painel administrativo. situacao vazia = todas
        public List<Pedido> Listar(string? busca, string? situacao, int status)
        {
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, "sp_pedido_listar",
                ("p_busca", busca ?? ""), ("p_situacao", situacao ?? ""), ("p_status", status));
            return LerLista(cmd);
        }

        public List<Pedido> UltimosPedidos()
        {
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, "sp_dashboard_ultimos_pedidos");
            return LerLista(cmd);
        }

        // Pedido + itens
        public Pedido? Obter(int id)
        {
            using var conn = db.GetConnection();

            Pedido? pedido;
            using (var cmd = db.Comando(conn, "sp_pedido_obter", ("p_id", id)))
            using (var reader = cmd.ExecuteReader())
            {
                pedido = reader.Read() ? Mapear(reader) : null;
            }

            if (pedido == null)
                return null;

            using (var cmd = db.Comando(conn, "sp_pedido_itens", ("p_pedido_id", id)))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    pedido.Itens.Add(new PedidoItem
                    {
                        Id = reader.Int("id"),
                        PedidoId = reader.Int("pedido_id"),
                        ProdutoId = reader.Int("produto_id"),
                        ProdutoNome = reader.Str("produto_nome"),
                        Imagem = reader.StrOuNulo("imagem"),
                        Quantidade = reader.Int("quantidade"),
                        PrecoUnitario = reader.Dec("preco_unitario")
                    });
                }
            }

            return pedido;
        }

        public void AlterarSituacao(int id, string situacao)
        {
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, "sp_pedido_alterar_situacao", ("p_id", id), ("p_situacao", situacao));
            cmd.ExecuteNonQuery();
        }

        public void CancelarDoCliente(int pedidoId, int clienteId)
        {
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, "sp_pedido_cancelar_cliente", ("p_id", pedidoId), ("p_cliente_id", clienteId));
            cmd.ExecuteNonQuery();
        }

        public void Excluir(int id) => Executar("sp_pedido_excluir", id);
        public void Reativar(int id) => Executar("sp_pedido_reativar", id);

        private void Executar(string procedure, int id)
        {
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, procedure, ("p_id", id));
            cmd.ExecuteNonQuery();
        }

        private static List<Pedido> LerLista(MySqlCommand cmd)
        {
            var lista = new List<Pedido>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(Mapear(reader));
            return lista;
        }

        private static Pedido Mapear(MySqlDataReader r)
        {
            return new Pedido
            {
                Id = r.Int("id"),
                ClienteId = r.Int("cliente_id"),
                ClienteNome = r.Str("cliente_nome"),
                ClienteEmail = r.Str("cliente_email"),
                DataPedido = r.Data("data_pedido"),
                Situacao = r.Str("situacao"),
                FormaPagamento = r.Str("forma_pagamento"),
                EnderecoEntrega = r.Str("endereco_entrega"),
                ValorProdutos = r.Dec("valor_produtos"),
                ValorFrete = r.Dec("valor_frete"),
                ValorTotal = r.Dec("valor_total"),
                QtdItens = r.Int("qtd_itens"),
                Status = r.Int("status")
            };
        }
    }
}
