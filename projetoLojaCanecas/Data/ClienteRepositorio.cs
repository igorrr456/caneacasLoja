using MySql.Data.MySqlClient;
using projetoLojaCanecas.Models;

namespace projetoLojaCanecas.Data
{
    public class ClienteRepositorio
    {
        private readonly DataBase db = new DataBase();

        // Devolve (id, nome) se e-mail e senha conferem e o cliente esta ativo; senao null
        public (int Id, string Nome)? Login(string email, string senha)
        {
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, "sp_cliente_login",
                ("p_email", email.Trim().ToLower()), ("p_senha", Seguranca.GerarHash(senha)));
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? (reader.Int("id"), reader.Str("nome")) : null;
        }

        // Cadastra e devolve o id do novo cliente
        public int Criar(CadastroClienteViewModel c)
        {
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, "sp_cliente_criar",
                ("p_nome", c.Nome.Trim()), ("p_cpf", Validacao.FormatarCpf(c.Cpf)),
                ("p_email", c.Email.Trim().ToLower()), ("p_senha", Seguranca.GerarHash(c.Senha)),
                ("p_telefone", c.Telefone), ("p_cep", Validacao.FormatarCep(c.Cep)),
                ("p_endereco", c.Endereco), ("p_numero", c.Numero), ("p_complemento", c.Complemento),
                ("p_bairro", c.Bairro), ("p_cidade", c.Cidade), ("p_uf", c.Uf.ToUpper()));
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public Cliente? Obter(int id)
        {
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, "sp_cliente_obter", ("p_id", id));
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Mapear(reader, false) : null;
        }

        public void Editar(int id, Cliente c)
        {
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, "sp_cliente_editar",
                ("p_id", id), ("p_nome", c.Nome.Trim()), ("p_cpf", Validacao.FormatarCpf(c.Cpf)),
                ("p_email", c.Email.Trim().ToLower()), ("p_telefone", c.Telefone),
                ("p_cep", Validacao.FormatarCep(c.Cep)), ("p_endereco", c.Endereco), ("p_numero", c.Numero),
                ("p_complemento", c.Complemento), ("p_bairro", c.Bairro), ("p_cidade", c.Cidade),
                ("p_uf", c.Uf.ToUpper()));
            cmd.ExecuteNonQuery();
        }

        public void AlterarSenha(int id, string senhaAtual, string novaSenha)
        {
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, "sp_cliente_alterar_senha",
                ("p_id", id), ("p_senha_atual", Seguranca.GerarHash(senhaAtual)), ("p_senha_nova", Seguranca.GerarHash(novaSenha)));
            cmd.ExecuteNonQuery();
        }

        // Listagem do painel (com total de pedidos e valor comprado)
        public List<Cliente> Listar(string? busca, int status)
        {
            var lista = new List<Cliente>();
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, "sp_cliente_listar", ("p_busca", busca ?? ""), ("p_status", status));
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(Mapear(reader, true));
            return lista;
        }

        public void Excluir(int id) => Executar("sp_cliente_excluir", id);
        public void Reativar(int id) => Executar("sp_cliente_reativar", id);

        private void Executar(string procedure, int id)
        {
            using var conn = db.GetConnection();
            using var cmd = db.Comando(conn, procedure, ("p_id", id));
            cmd.ExecuteNonQuery();
        }

        // resumo = true -> resultado da listagem (menos colunas, mais totais)
        private static Cliente Mapear(MySqlDataReader r, bool resumo)
        {
            var c = new Cliente
            {
                Id = r.Int("id"),
                Nome = r.Str("nome"),
                Cpf = r.Str("cpf"),
                Email = r.Str("email"),
                Telefone = r.StrOuNulo("telefone"),
                Cidade = r.Str("cidade"),
                Uf = r.Str("uf"),
                DataCadastro = r.Data("data_cadastro"),
                Status = r.Int("status")
            };

            if (resumo)
            {
                c.TotalPedidos = r.Int("total_pedidos");
                c.TotalComprado = r.Dec("total_comprado");
            }
            else
            {
                c.Cep = r.Str("cep");
                c.Endereco = r.Str("endereco");
                c.Numero = r.Str("numero");
                c.Complemento = r.StrOuNulo("complemento");
                c.Bairro = r.Str("bairro");
            }
            return c;
        }
    }
}
