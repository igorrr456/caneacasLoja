using System.ComponentModel.DataAnnotations;

namespace projetoLojaCanecas.Models
{
    // Usuario do painel administrativo
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o nome.")]
        [StringLength(100)]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o login.")]
        [StringLength(50)]
        [Display(Name = "Login")]
        public string Login { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecione o perfil.")]
        [Display(Name = "Perfil")]
        public string Perfil { get; set; } = Perfis.Gerente;

        // Obrigatoria so no cadastro; na edicao, vazia = mantem a senha atual
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string? Senha { get; set; }

        public DateTime DataCadastro { get; set; }
        public int Status { get; set; } = 1;
    }
}
