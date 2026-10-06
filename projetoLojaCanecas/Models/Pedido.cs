namespace projetoLojaCanecas.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string ClienteNome { get; set; } = string.Empty;
        public string ClienteEmail { get; set; } = string.Empty;
        public DateTime DataPedido { get; set; }

        // Pendente | Pago | Enviado | Entregue | Cancelado
        public string Situacao { get; set; } = "Pendente";
        public string FormaPagamento { get; set; } = string.Empty;
        public string EnderecoEntrega { get; set; } = string.Empty;
        public decimal ValorProdutos { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorTotal { get; set; }
        public int QtdItens { get; set; }

        // exclusao logica
        public int Status { get; set; } = 1;

        public List<PedidoItem> Itens { get; set; } = new List<PedidoItem>();

        // Proximo passo do andamento (null = pedido encerrado)
        public string? ProximaSituacao => Situacao switch
        {
            "Pendente" => "Pago",
            "Pago" => "Enviado",
            "Enviado" => "Entregue",
            _ => null
        };

        public bool PodeCancelar => Situacao == "Pendente" || Situacao == "Pago";
    }

    public class PedidoItem
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public int ProdutoId { get; set; }
        public string ProdutoNome { get; set; } = string.Empty;
        public string? Imagem { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }

        public decimal Subtotal => Quantidade * PrecoUnitario;
        public string ImagemUrl => string.IsNullOrEmpty(Imagem) ? "/img/sem-foto.svg" : "/img/produtos/" + Imagem;
    }
}
