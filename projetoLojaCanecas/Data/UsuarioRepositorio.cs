using MySql.Data.MySqlClient;
using projetoLojaCanecas.Models;

namespace projetoLojaCanecas.Data
{
    // Usuarios do painel administrativo
    public class UsuarioRepositorio
    {
        private readonly DataBase db = new DataBase();

        public Usuario? Login(string login, string senha)
        {
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, "sp_usuario_login",
                ("p_login", login.Trim()), ("p_senha", Seguranca.GerarHash(senha)));
            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return null;

            return new Usuario
            {
                Id = reader.Int("id"),
                Nome = reader.Str("nome"),
                Login = reader.Str("login"),
                Perfil = reader.Str("perfil")
            };
        }

        public List<Usuario> Listar(int status)
        {
            var lista = new List<Usuario>();
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, "sp_usuario_listar", ("p_status", status));
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(Mapear(reader));
            return lista;
        }

        public Usuario? Obter(int id)
        {
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, "sp_usuario_obter", ("p_id", id));
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Mapear(reader) : null;
        }

        public void Criar(Usuario u)
        {
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, "sp_usuario_criar",
                ("p_nome", u.Nome.Trim()), ("p_login", u.Login.Trim()),
                ("p_senha", Seguranca.GerarHash(u.Senha ?? "")), ("p_perfil", u.Perfil));
            cmd.ExecuteNonQuery();
        }

        // Senha vazia = mantem a atual
        public void Editar(int id, Usuario u)
        {
            string? hash = string.IsNullOrEmpty(u.Senha) ? null : Seguranca.GerarHash(u.Senha);

            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, "sp_usuario_editar",
                ("p_id", id), ("p_nome", u.Nome.Trim()), ("p_login", u.Login.Trim()),
                ("p_perfil", u.Perfil), ("p_senha", hash));
            cmd.ExecuteNonQuery();
        }

        public void AlterarSenha(int id, string senhaAtual, string novaSenha)
        {
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, "sp_usuario_alterar_senha",
                ("p_id", id), ("p_senha_atual", Seguranca.GerarHash(senhaAtual)), ("p_senha_nova", Seguranca.GerarHash(novaSenha)));
            cmd.ExecuteNonQuery();
        }

        public void Excluir(int id) => Executar("sp_usuario_excluir", id);
        public void Reativar(int id) => Executar("sp_usuario_reativar", id);

        private void Executar(string procedure, int id)
        {
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, procedure, ("p_id", id));
            cmd.ExecuteNonQuery();
        }

        private static Usuario Mapear(MySqlDataReader r)
        {
            return new Usuario
            {
                Id = r.Int("id"),
                Nome = r.Str("nome"),
                Login = r.Str("login"),
                Perfil = r.Str("perfil"),
                DataCadastro = r.Data("data_cadastro"),
                Status = r.Int("status")
            };
        }
    }
}
