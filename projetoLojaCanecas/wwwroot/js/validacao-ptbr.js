// jQuery Validation espera numeros com PONTO (49.90). No Brasil digitamos VIRGULA (49,90).
// Este ajuste faz a validacao do navegador aceitar o formato brasileiro.
if (window.jQuery && jQuery.validator) {
    jQuery.validator.methods.number = function (value, element) {
        return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:\.\d{3})+)(?:,\d+)?$/.test(value);
    };

    jQuery.validator.methods.range = function (value, element, param) {
        var numero = parseFloat(String(value).replace(/\./g, '').replace(',', '.'));
        return this.optional(element) || (numero >= param[0] && numero <= param[1]);
    };
}
