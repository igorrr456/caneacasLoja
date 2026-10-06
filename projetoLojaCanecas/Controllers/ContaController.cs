using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using projetoLojaCanecas.Autentication;
using projetoLojaCanecas.Data;
using projetoLojaCanecas.Models;

namespace projetoLojaCanecas.Controllers
{
    // Login, cadastro e dados do CLIENTE da loja
    public class ContaController : Controller
    {
        private readonly ClienteRepositorio clientes = new ClienteRepositorio();

        // GET: /Conta/Login
        public IActionResult Login(string? returnUrl = null)
        {
            if (HttpContext.Session.GetInt32(SessionKeys.ClienteId) != null)
                return RedirectToAction("Index", "Home");

            return View(new ClienteLoginViewModel { ReturnUrl = returnUrl });
        }

        // POST: /Conta/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(ClienteLoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var cliente = clientes.Login(model.Email, model.Senha);
            if (cliente == null)
            {
                ViewBag.Erro = "E-mail ou senha inválidos.";
                return View(model);
            }

            EntrarNaSessao(cliente.Value.Id, cliente.Value.Nome);
            return VoltarOuInicio(model.ReturnUrl);
        }

        // GET: /Conta/Cadastrar
        public IActionResult Cadastrar(string? returnUrl = null)
        {
            if (HttpContext.Session.GetInt32(SessionKeys.ClienteId) != null)
                return RedirectToAction("Index", "Home");

            return View(new CadastroClienteViewModel { ReturnUrl = returnUrl });
        }

        // POST: /Conta/Cadastrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cadastrar(CadastroClienteViewModel model)
        {
            if (!Validacao.CpfValido(model.Cpf))
                ModelState.AddModelError(nameof(model.Cpf), "CPF inválido.");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                int id = clientes.Criar(model);
                EntrarNaSessao(id, model.Nome.Trim());   // ja deixa o cliente logado
            }
            catch (MySqlException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }

            TempData["Mensagem"] = "Cadastro realizado com sucesso! Bem-vindo(a) à Loja de Canecas.";
            return VoltarOuInicio(model.ReturnUrl);
        }

        // GET: /Conta/Sair  (o carrinho continua guardado)
        public IActionResult Sair()
        {
            HttpContext.Session.Remove(SessionKeys.ClienteId);
            HttpContext.Session.Remove(SessionKeys.ClienteNome);
            return RedirectToAction("Index", "Home");
        }

        // GET: /Conta/MinhaConta
        [ClienteAuthorize]
        public IActionResult MinhaConta()
        {
            var cliente = clientes.Obter(HttpContext.Session.GetInt32(SessionKeys.ClienteId)!.Value);
            if (cliente == null)
                return RedirectToAction("Sair");

            return View(cliente);
        }

        // POST: /Conta/MinhaConta
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClienteAuthorize]
        public IActionResult MinhaConta(Cliente model)
        {
            if (!Validacao.CpfValido(model.Cpf))
                ModelState.AddModelError(nameof(model.Cpf), "CPF inválido.");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                clientes.Editar(HttpContext.Session.GetInt32(SessionKeys.ClienteId)!.Value, model);
                HttpContext.Session.SetString(SessionKeys.ClienteNome, model.Nome.Trim());
            }
            catch (MySqlException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }

            TempData["Mensagem"] = "Seus dados foram atualizados.";
            return RedirectToAction("MinhaConta");
        }

        // GET: /Conta/AlterarSenha
        [ClienteAuthorize]
        public IActionResult AlterarSenha()
        {
            return View();
        }

        // POST: /Conta/AlterarSenha
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClienteAuthorize]
        public IActionResult AlterarSenha(AlterarSenhaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                clientes.AlterarSenha(HttpContext.Session.GetInt32(SessionKeys.ClienteId)!.Value, model.SenhaAtual, model.NovaSenha);
            }
            catch (MySqlException ex)
            {
                ViewBag.Erro = ex.Message;
                return View(model);
            }

            TempData["Mensagem"] = "Senha alterada com sucesso!";
            return RedirectToAction("MinhaConta");
        }

        // ------------------------------------------------------------------
        private void EntrarNaSessao(int id, string nome)
        {
            HttpContext.Session.SetInt32(SessionKeys.ClienteId, id);
            HttpContext.Session.SetString(SessionKeys.ClienteNome, nome);
        }

        private IActionResult VoltarOuInicio(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}
