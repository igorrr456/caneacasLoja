using MySql.Data.MySqlClient;

namespace projetoLojaCanecas.Data
{
    public class DataBase
    {
        // Ajuste usuario/senha conforme o SEU MySQL
        private readonly string connectionString = "server=localhost;port=3305;database=loja_canecas_db;user=root;password=12345678;";

        public MySqlConnection GetConnection()
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            conn.Open();
            return conn;
        }

        // Monta o comando de uma stored procedure ja com os parametros.
        // Valor null vira NULL no banco.
        //   db.Comando(conn, "sp_produto_obter", ("p_id", id))
        public MySqlCommand Comando(MySqlConnection conn, string procedure, params (string nome, object? valor)[] parametros)
        {
            var cmd = new MySqlCommand(procedure, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            foreach (var (nome, valor) in parametros)
                cmd.Parameters.AddWithValue(nome, valor ?? DBNull.Value);
            return cmd;
        }
    }
}
