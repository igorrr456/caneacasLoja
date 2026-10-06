using System.ComponentModel.DataAnnotations;

namespace projetoLojaCanecas.Models
{
    // Login do painel administrativo (usuario + senha)
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Informe o login.")]
        public string Login { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a senha.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; } = string.Empty;
    }

    // Login do cliente da loja (e-mail + senha)
    public class ClienteLoginViewModel
    {
        [Required(ErrorMessage = "Informe seu e-mail.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a senha.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }
    }

    public class AlterarSenhaViewModel
    {
        [Required(ErrorMessage = "Informe a senha atual.")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha atual")]
        public string SenhaAtual { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a nova senha.")]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nova senha")]
        public string NovaSenha { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirme a nova senha.")]
        [Compare("NovaSenha", ErrorMessage = "As senhas não conferem.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar nova senha")]
        public string ConfirmarSenha { get; set; } = string.Empty;
    }
}
