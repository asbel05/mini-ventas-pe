document.addEventListener('DOMContentLoaded', function () {
    const selectTipo = document.getElementById('selectTipoDocumento');
    const inputDocumento = document.getElementById('inputNumeroDocumento');

    if (!selectTipo || !inputDocumento) return;

    function aplicarReglasDocumento() {
        const tipo = selectTipo.value;

        if (tipo === '1') {
            inputDocumento.maxLength = 8;
            inputDocumento.value = inputDocumento.value.replace(/\D/g, '').slice(0, 8);
        } else if (tipo === '6') {
            inputDocumento.maxLength = 11;
            inputDocumento.value = inputDocumento.value.replace(/\D/g, '').slice(0, 11);
        } else {
            inputDocumento.maxLength = 12;
            inputDocumento.value = inputDocumento.value.replace(/[^A-Za-z0-9]/g, '').slice(0, 12);
        }
    }

    selectTipo.addEventListener('change', function () {
        aplicarReglasDocumento();
        inputDocumento.focus();
    });

    inputDocumento.addEventListener('input', aplicarReglasDocumento);

    aplicarReglasDocumento();
});