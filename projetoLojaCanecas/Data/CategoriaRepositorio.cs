using MySql.Data.MySqlClient;
using projetoLojaCanecas.Models;

namespace projetoLojaCanecas.Data
{
    public class CategoriaRepositorio
    {
        private readonly DataBase db = new DataBase();

        public List<Categoria> Listar(string? busca, int status)
        {
            var lista = new List<Categoria>();
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, "sp_categoria_listar", ("p_busca", busca ?? ""), ("p_status", status));
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(Mapear(reader));
            return lista;
        }

        public Categoria? Obter(int id)
        {
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, "sp_categoria_obter", ("p_id", id));
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Mapear(reader) : null;
        }

        public void Criar(Categoria c)
        {
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, "sp_categoria_criar", ("p_nome", c.Nome), ("p_descricao", c.Descricao));
            cmd.ExecuteNonQuery();
        }

        public void Editar(int id, Categoria c)
        {
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, "sp_categoria_editar", ("p_id", id), ("p_nome", c.Nome), ("p_descricao", c.Descricao));
            cmd.ExecuteNonQuery();
        }

        public void Excluir(int id) => Executar("sp_categoria_excluir", id);
        public void Reativar(int id) => Executar("sp_categoria_reativar", id);

        private void Executar(string procedure, int id)
        {
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, procedure, ("p_id", id));
            cmd.ExecuteNonQuery();
        }

        private static Categoria Mapear(MySqlDataReader r)
        {
            return new Categoria
            {
                Id = r.Int("id"),
                Nome = r.Str("nome"),
                Descricao = r.StrOuNulo("descricao"),
                DataCadastro = r.Data("data_cadastro"),
                TotalProdutos = r.Int("total_produtos"),
                Status = r.Int("status")
            };
        }
    }
}
