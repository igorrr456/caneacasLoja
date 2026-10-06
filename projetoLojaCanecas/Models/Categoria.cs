using System.ComponentModel.DataAnnotations;

namespace projetoLojaCanecas.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o nome da categoria.")]
        [StringLength(60)]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(200)]
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        public DateTime DataCadastro { get; set; }

        public int TotalProdutos { get; set; }

        public int Status { get; set; } = 1;
    }
}
