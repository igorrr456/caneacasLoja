using System.ComponentModel.DataAnnotations;

namespace projetoLojaCanecas.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe seu nome.")]
        [StringLength(100)]
        [Display(Name = "Nome completo")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe seu CPF.")]
        [StringLength(14)]
        [Display(Name = "CPF")]
        public string Cpf { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe seu e-mail.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        [StringLength(100)]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        [Display(Name = "Telefone")]
        public string? Telefone { get; set; }

        // ---- Endereco de entrega ----
        [Required(ErrorMessage = "Informe o CEP.")]
        [StringLength(9)]
        [Display(Name = "CEP")]
        public string Cep { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o endereço.")]
        [StringLength(150)]
        [Display(Name = "Endereço")]
        public string Endereco { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o número.")]
        [StringLength(10)]
        [Display(Name = "Número")]
        public string Numero { get; set; } = string.Empty;

        [StringLength(60)]
        [Display(Name = "Complemento")]
        public string? Complemento { get; set; }

        [Required(ErrorMessage = "Informe o bairro.")]
        [StringLength(60)]
        [Display(Name = "Bairro")]
        public string Bairro { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a cidade.")]
        [StringLength(60)]
        [Display(Name = "Cidade")]
        public string Cidade { get; set; } = string.Empty;


        [Required(ErrorMessage = "Informe o estado.")]
        [RegularExpression(@"^[A-Za-z]{2}$", ErrorMessage = "Selecione o estado.")]
        [Display(Name = "UF")]
        public string Uf { get; set; } = string.Empty;

        public DateTime DataCadastro { get; set; }
        public int Status { get; set; } = 1;

        // ---- Usados so na listagem do painel ----
        public int TotalPedidos { get; set; }
        public decimal TotalComprado { get; set; }

        // Endereco em uma linha (vira o "endereco_entrega" do pedido)
        public string EnderecoCompleto =>
            $"{Endereco}, {Numero}{(string.IsNullOrWhiteSpace(Complemento) ? "" : " - " + Complemento)} - {Bairro} - {Cidade}/{Uf} - CEP {Cep}";
    }

    // Cadastro novo = dados do cliente + senha
    public class CadastroClienteViewModel : Cliente
    {
        [Required(ErrorMessage = "Crie uma senha.")]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Senha { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirme a senha.")]
        [Compare("Senha", ErrorMessage = "As senhas não conferem.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar senha")]
        public string ConfirmarSenha { get; set; } = string.Empty;

        // Para onde voltar depois de cadastrar (ex.: /Checkout)
        public string? ReturnUrl { get; set; }
    }
}
