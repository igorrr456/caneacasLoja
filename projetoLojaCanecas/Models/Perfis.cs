namespace projetoLojaCanecas.Models
{
    // Perfis do painel administrativo
    //   Administrador: tudo, inclusive cadastrar usuarios do painel
    //   Gerente      : produtos, categorias, pedidos, clientes e dashboard
    public static class Perfis
    {
        public const string Administrador = "Administrador";
        public const string Gerente = "Gerente";

        public static readonly string[] Todos = { Administrador, Gerente };
    }
}
