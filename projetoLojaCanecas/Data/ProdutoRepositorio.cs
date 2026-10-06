using MySql.Data.MySqlClient;
using projetoLojaCanecas.Models;

namespace projetoLojaCanecas.Data
{
    // Acesso ao banco: tudo via stored procedures (sem Entity Framework)
    public class ProdutoRepositorio
    {
        private readonly DataBase db = new DataBase();

        // Painel administrativo (status 1 = ativos, 0 = excluidos)
        public List<Produto> Listar(string? busca, int status)
        {
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, "sp_produto_listar", ("p_busca", busca ?? ""), ("p_status", status));
            return LerLista(cmd);
        }

        // Vitrine da loja
        public List<Produto> ListarLoja(string? busca, int categoriaId, string? ordem)
        {
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, "sp_produto_listar_loja",
                ("p_busca", busca ?? ""), ("p_categoria_id", categoriaId), ("p_ordem", ordem ?? ""));
            return LerLista(cmd);
        }

        public List<Produto> Destaques()
        {
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, "sp_produto_listar_destaques");
            return LerLista(cmd);
        }

        public List<Produto> Relacionados(int produtoId, int categoriaId)
        {
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, "sp_produto_relacionados", ("p_id", produtoId), ("p_categoria_id", categoriaId));
            return LerLista(cmd);
        }

        // Usado pelo carrinho: busca varios produtos de uma vez
        public List<Produto> ListarPorIds(IEnumerable<int> ids)
        {
            string lista = string.Join(",", ids);
            if (lista == "")
                return new List<Produto>();

            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, "sp_produto_listar_por_ids", ("p_ids", lista));
            return LerLista(cmd);
        }

        public Produto? Obter(int id)
        {
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, "sp_produto_obter", ("p_id", id));
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Mapear(reader) : null;
        }

        public void Criar(Produto p, string? imagem)
        {
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, "sp_produto_criar",
                ("p_categoria_id", p.CategoriaId), ("p_nome", p.Nome), ("p_descricao", p.Descricao),
                ("p_capacidade_ml", p.CapacidadeMl), ("p_material", p.Material), ("p_cor", p.Cor),
                ("p_preco", p.Preco), ("p_estoque", p.Estoque), ("p_imagem", imagem),
                ("p_destaque", p.Destaque ? 1 : 0));
            cmd.ExecuteNonQuery();
        }

        // imagem == null mantem a foto atual
        public void Editar(int id, Produto p, string? imagem)
        {
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, "sp_produto_editar",
                ("p_id", id), ("p_categoria_id", p.CategoriaId), ("p_nome", p.Nome), ("p_descricao", p.Descricao),
                ("p_capacidade_ml", p.CapacidadeMl), ("p_material", p.Material), ("p_cor", p.Cor),
                ("p_preco", p.Preco), ("p_estoque", p.Estoque), ("p_imagem", imagem),
                ("p_destaque", p.Destaque ? 1 : 0));
            cmd.ExecuteNonQuery();
        }

        public void Excluir(int id) => Executar("sp_produto_excluir", id);
        public void Reativar(int id) => Executar("sp_produto_reativar", id);

        private void Executar(string procedure, int id)
        {
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, procedure, ("p_id", id));
            cmd.ExecuteNonQuery();
        }

        private static List<Produto> LerLista(MySqlCommand cmd)
        {
            var lista = new List<Produto>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(Mapear(reader));
            return lista;
        }

        private static Produto Mapear(MySqlDataReader r)
        {
            return new Produto
            {
                Id = r.Int("id"),
                CategoriaId = r.Int("categoria_id"),
                CategoriaNome = r.Str("categoria_nome"),
                Nome = r.Str("nome"),
                Descricao = r.StrOuNulo("descricao"),
                CapacidadeMl = r.Int("capacidade_ml"),
                Material = r.StrOuNulo("material"),
                Cor = r.StrOuNulo("cor"),
                Preco = r.Dec("preco"),
                Estoque = r.Int("estoque"),
                Imagem = r.StrOuNulo("imagem"),
                Destaque = r.Bool("destaque"),
                DataCadastro = r.Data("data_cadastro"),
                Status = r.Int("status")
            };
        }
    }
}
