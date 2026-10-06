using System.Text.Json;
using projetoLojaCanecas.Autentication;
using projetoLojaCanecas.Data;
using projetoLojaCanecas.Models;

namespace projetoLojaCanecas.Helpers
{
    // O carrinho fica na SESSAO (so produto + quantidade). Os precos sao sempre
    // lidos do banco na hora de mostrar/finalizar, entao nunca ficam desatualizados.
    public static class CarrinhoHelper
    {
        public static List<CarrinhoItem> Obter(ISession session)
        {
            string? json = session.GetString(SessionKeys.Carrinho);
            if (string.IsNullOrEmpty(json))
                return new List<CarrinhoItem>();

            return JsonSerializer.Deserialize<List<CarrinhoItem>>(json) ?? new List<CarrinhoItem>();
        }

        public static void Salvar(ISession session, List<CarrinhoItem> itens)
        {
            session.SetString(SessionKeys.Carrinho, JsonSerializer.Serialize(itens));
        }

        public static int TotalItens(ISession session)
        {
            return Obter(session).Sum(i => i.Quantidade);
        }

        // Monta o carrinho para exibir, usando os dados ATUAIS do banco (preco, estoque, produto excluido).
        // Itens indisponiveis saem do carrinho e quantidades acima do estoque sao reduzidas.
        // alterado = true quando o carrinho precisou ser ajustado (para avisar o cliente).
        public static CarrinhoViewModel Montar(ISession session, ProdutoRepositorio repo, out bool alterado)
        {
            var itens = Obter(session);
            var produtos = repo.ListarPorIds(itens.Select(i => i.ProdutoId)).ToDictionary(p => p.Id);

            var carrinho = new CarrinhoViewModel();
            var ajustados = new List<CarrinhoItem>();
            alterado = false;

            foreach (var item in itens)
            {
                if (!produtos.TryGetValue(item.ProdutoId, out var produto) || produto.Estoque <= 0)
                {
                    alterado = true;
                    continue;
                }

                int qtd = Math.Min(item.Quantidade, produto.Estoque);
                if (qtd != item.Quantidade)
                    alterado = true;

                ajustados.Add(new CarrinhoItem { ProdutoId = produto.Id, Quantidade = qtd });
                carrinho.Linhas.Add(new CarrinhoLinha { Produto = produto, Quantidade = qtd });
            }

            if (alterado)
                Salvar(session, ajustados);

            return carrinho;
        }

        public static void Limpar(ISession session)
        {
            session.Remove(SessionKeys.Carrinho);
        }
    }
}
