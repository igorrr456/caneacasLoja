using System.Security.Cryptography;
using System.Text;

namespace projetoLojaCanecas.Data
{
    public static class Seguranca
    {
        // Gera o hash SHA-256 da senha (64 caracteres hexadecimais minusculos).
        // E o mesmo resultado da funcao SHA2('senha', 256) do MySQL.
        public static string GerarHash(string senha)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(senha));
            return Convert.ToHexString(bytes).ToLower();
        }
    }
}
