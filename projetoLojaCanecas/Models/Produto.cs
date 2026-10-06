using System.ComponentModel.DataAnnotations;

namespace projetoLojaCanecas.Models
{
    public class Produto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o nome da caneca.")]
        [StringLength(100)]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        // ---- Chave estrangeira (vai para o banco) ----
        [Range(1, int.MaxValue, ErrorMessage = "Selecione a categoria.")]
        [Display(Name = "Categoria")]
        public int CategoriaId { get; set; }

        // ---- Nome vindo do JOIN (so para exibir) ----
        public string? CategoriaNome { get; set; }

        [StringLength(500)]
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        [Range(50, 2000, ErrorMessage = "Informe a capacidade em ml (entre 50 e 2000).")]
        [Display(Name = "Capacidade (ml)")]
        public int CapacidadeMl { get; set; } = 325;

        [StringLength(50)]
        [Display(Name = "Material")]
        public string? Material { get; set; }

        [StringLength(30)]
        [Display(Name = "Cor")]
        public string? Cor { get; set; }

        [Range(0.01, 9999.99, ErrorMessage = "Informe um preço entre 0,01 e 9.999,99.")]
        [Display(Name = "Preço (R$)")]
        public decimal Preco { get; set; }

        [Range(0, 100000, ErrorMessage = "Estoque inválido.")]
        [Display(Name = "Estoque")]
        public int Estoque { get; set; }

        [Display(Name = "Destaque na página inicial")]
        public bool Destaque { get; set; }

        // Nome do arquivo gravado em wwwroot/img/produtos (vem do banco)
        public string? Imagem { get; set; }

        // Foto enviada no formulario (nao vai para o banco; o arquivo e salvo em disco)
        [Display(Name = "Foto da caneca")]
        public IFormFile? ImagemArquivo { get; set; }

        public DateTime DataCadastro { get; set; }

        public int Status { get; set; } = 1;

        public string ImagemUrl => string.IsNullOrEmpty(Imagem) ? "/img/sem-foto.svg" : "/img/produtos/" + Imagem;
        public bool Disponivel => Estoque > 0;
    }
}
