// =====================================================================
//  Loja de Canecas - scripts simples (sem frameworks alem do jQuery)
// =====================================================================

// ---- Seletor de quantidade: botoes - e + ----------------------------
document.addEventListener('click', function (e) {
    var btn = e.target.closest('[data-qty-step]');
    if (!btn) return;

    var box = btn.closest('.qty-stepper');
    var input = box.querySelector('input');
    var passo = parseInt(btn.getAttribute('data-qty-step'), 10);
    var min = parseInt(input.min || '1', 10);
    var max = parseInt(input.max || '99', 10);
    var valor = (parseInt(input.value, 10) || min) + passo;

    input.value = Math.max(min, Math.min(max, valor));
});

// ---- Mascaras (CPF, CEP, telefone) e busca de CEP -----------------
function soDigitos(v) { return (v || '').replace(/\D/g, ''); }

document.addEventListener('input', function (e) {
    var el = e.target;
    var tipo = el.getAttribute && el.getAttribute('data-mask');
    if (!tipo) return;

    var d = soDigitos(el.value);
    if (tipo === 'cpf') {
        d = d.substring(0, 11);
        el.value = d.replace(/(\d{3})(\d)/, '$1.$2').replace(/(\d{3})(\d)/, '$1.$2').replace(/(\d{3})(\d{1,2})$/, '$1-$2');
    } else if (tipo === 'cep') {
        d = d.substring(0, 8);
        el.value = d.replace(/(\d{5})(\d)/, '$1-$2');
    } else if (tipo === 'tel') {
        d = d.substring(0, 11);
        if (d.length > 10) el.value = d.replace(/(\d{2})(\d{5})(\d{1,4})/, '($1) $2-$3');
        else if (d.length > 6) el.value = d.replace(/(\d{2})(\d{4})(\d{1,4})/, '($1) $2-$3');
        else if (d.length > 2) el.value = d.replace(/(\d{2})(\d+)/, '($1) $2');
        else el.value = d;
    }
});

// Preenche endereco automaticamente pelo CEP (servico gratuito ViaCEP).
// Se nao houver internet, simplesmente nao faz nada e o cliente digita.
document.addEventListener('focusout', function (e) {
    var el = e.target;
    if (!el.matches || !el.matches('[data-mask="cep"]')) return;

    var cep = soDigitos(el.value);
    if (cep.length !== 8) return;

    fetch('https://viacep.com.br/ws/' + cep + '/json/')
        .then(function (r) { return r.json(); })
        .then(function (d) {
            if (d.erro) return;
            var set = function (nome, valor) {
                var campo = document.querySelector('[name="' + nome + '"]');
                if (campo && valor) campo.value = valor;
            };
            set('Endereco', d.logradouro);
            set('Bairro', d.bairro);
            set('Cidade', d.localidade);
            set('Uf', d.uf);
        })
        .catch(function () { /* sem internet: ignora */ });
});

// ---- Pre-visualizacao da foto escolhida no painel -------------------
document.addEventListener('change', function (e) {
    var el = e.target;
    if (!el.matches || !el.matches('[data-preview]')) return;

    var img = document.querySelector(el.getAttribute('data-preview'));
    if (img && el.files && el.files[0]) img.src = URL.createObjectURL(el.files[0]);
});
