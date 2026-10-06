namespace projetoLojaCanecas.Autentication
{
    // Nomes das chaves gravadas na Session (evita erros de digitacao)
    public static class SessionKeys
    {
        // Cliente da loja
        public const string ClienteId = "ClienteId";
        public const string ClienteNome = "ClienteNome";

        // Usuario do painel administrativo
        public const string AdminId = "AdminId";
        public const string AdminNome = "AdminNome";
        public const string AdminLogin = "AdminLogin";
        public const string AdminPerfil = "AdminPerfil";

        // Carrinho de compras (lista de produto/quantidade em JSON)
        public const string Carrinho = "Carrinho";
    }
}
