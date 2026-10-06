using System.ComponentModel.DataAnnotations;

namespace projetoLojaCanecas.Models
{
    // O que fica guardado na sessao
    public class CarrinhoItem
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
    }

    // Uma linha do carrinho para exibir (produto vindo do banco + quantidade)
    public class CarrinhoLinha
    {
        public Produto Produto { get; set; } = new Produto();
        public int Quantidade { get; set; }
        public decimal Subtotal => Produto.Preco * Quantidade;
    }

    public class CarrinhoViewModel
    {
        public List<CarrinhoLinha> Linhas { get; set; } = new List<CarrinhoLinha>();

        public int TotalItens => Linhas.Sum(l => l.Quantidade);
        public decimal Subtotal => Linhas.Sum(l => l.Subtotal);
        public decimal Frete => RegrasLoja.CalcularFrete(Subtotal);
        public decimal Total => Subtotal + Frete;

        // Quanto falta para ganhar frete gratis (0 = ja ganhou)
        public decimal FaltaParaFreteGratis => Subtotal >= RegrasLoja.FreteGratisAPartirDe
            ? 0m : RegrasLoja.FreteGratisAPartirDe - Subtotal;
    }

    public class CheckoutViewModel
    {
        public CarrinhoViewModel Carrinho { get; set; } = new CarrinhoViewModel();

        [Required(ErrorMessage = "Informe o endereço de entrega.")]
        [StringLength(300)]
        [Display(Name = "Endereço de entrega")]
        public string EnderecoEntrega { get; set; } = string.Empty;

        [Required(ErrorMessage = "Escolha a forma de pagamento.")]
        [Display(Name = "Forma de pagamento")]
        public string FormaPagamento { get; set; } = "Pix";
    }
}

namespace projetoLojaCanecas.Models
{
    // Tela inicial da loja: vitrine com filtros
    public class VitrineViewModel
    {
        public List<Produto> Produtos { get; set; } = new List<Produto>();
        public List<Produto> Destaques { get; set; } = new List<Produto>();
        public List<Categoria> Categorias { get; set; } = new List<Categoria>();
        public string? Busca { get; set; }
        public int CategoriaId { get; set; }
        public string Ordem { get; set; } = "";

        // Mostra os destaques so na pagina inicial "limpa" (sem filtro)
        public bool SemFiltro => string.IsNullOrWhiteSpace(Busca) && CategoriaId == 0;
    }
}
