using MySql.Data.MySqlClient;

namespace projetoLojaCanecas.Data
{
    // Atalhos para ler colunas do MySqlDataReader pelo nome
    public static class ReaderExtensions
    {
        public static int Int(this MySqlDataReader r, string coluna) => Convert.ToInt32(r[coluna]);
        public static decimal Dec(this MySqlDataReader r, string coluna) => Convert.ToDecimal(r[coluna]);
        public static DateTime Data(this MySqlDataReader r, string coluna) => Convert.ToDateTime(r[coluna]);

        // TINYINT(1) -> bool
        public static bool Bool(this MySqlDataReader r, string coluna) => Convert.ToInt32(r[coluna]) == 1;

        public static string Str(this MySqlDataReader r, string coluna)
            => r.IsDBNull(r.GetOrdinal(coluna)) ? "" : Convert.ToString(r[coluna]) ?? "";

        public static string? StrOuNulo(this MySqlDataReader r, string coluna)
            => r.IsDBNull(r.GetOrdinal(coluna)) ? null : Convert.ToString(r[coluna]);
    }
}
