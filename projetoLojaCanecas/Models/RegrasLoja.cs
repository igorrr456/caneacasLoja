namespace projetoLojaCanecas.Models
{
    // Regras comerciais da loja num lugar so.
    // ATENCAO: o frete tambem e calculado na procedure sp_pedido_finalizar (mesma regra).
    public static class RegrasLoja
    {
        public const decimal FreteGratisAPartirDe = 150m;
        public const decimal ValorFrete = 15.90m;
        public const int EstoqueBaixo = 5;

        public static decimal CalcularFrete(decimal subtotal)
        {
            return subtotal >= FreteGratisAPartirDe || subtotal <= 0 ? 0m : ValorFrete;
        }

        public static readonly string[] FormasPagamento = { "Pix", "Cartão de crédito", "Boleto" };

        // Ordem do andamento de um pedido
        public static readonly string[] Situacoes = { "Pendente", "Pago", "Enviado", "Entregue", "Cancelado" };

        // Cor do "badge" do Bootstrap para cada situacao
        public static string CorSituacao(string situacao) => situacao switch
        {
            "Pendente" => "warning text-dark",
            "Pago" => "info text-dark",
            "Enviado" => "primary",
            "Entregue" => "success",
            "Cancelado" => "danger",
            _ => "secondary"
        };

        public static readonly string[] Ufs =
        {
            "AC","AL","AP","AM","BA","CE","DF","ES","GO","MA","MT","MS","MG","PA",
            "PB","PR","PE","PI","RJ","RN","RS","RO","RR","SC","SP","SE","TO"
        };
    }
}
