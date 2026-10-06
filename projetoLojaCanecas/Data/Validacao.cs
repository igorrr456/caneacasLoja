using System.Text.RegularExpressions;

namespace projetoLojaCanecas.Data
{
    // Pequenas funcoes de validacao e formatacao usadas nos cadastros
    public static class Validacao
    {
        public static string SomenteDigitos(string? texto)
        {
            return Regex.Replace(texto ?? "", @"\D", "");
        }

        // Confere os dois digitos verificadores do CPF
        public static bool CpfValido(string? cpf)
        {
            string d = SomenteDigitos(cpf);
            if (d.Length != 11 || d.Distinct().Count() == 1)
                return false;

            for (int tamanho = 9; tamanho <= 10; tamanho++)
            {
                int soma = 0;
                for (int i = 0; i < tamanho; i++)
                    soma += (d[i] - '0') * (tamanho + 1 - i);

                int resto = (soma * 10) % 11;
                if (resto == 10) resto = 0;
                if (resto != d[tamanho] - '0')
                    return false;
            }
            return true;
        }

        // 12345678901 -> 123.456.789-01
        public static string FormatarCpf(string? cpf)
        {
            string d = SomenteDigitos(cpf);
            return d.Length == 11 ? $"{d[..3]}.{d.Substring(3, 3)}.{d.Substring(6, 3)}-{d.Substring(9, 2)}" : (cpf ?? "");
        }

        // 01310100 -> 01310-100
        public static string FormatarCep(string? cep)
        {
            string d = SomenteDigitos(cep);
            return d.Length == 8 ? $"{d[..5]}-{d.Substring(5)}" : (cep ?? "");
        }
    }
}
