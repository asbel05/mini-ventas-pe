document.addEventListener('DOMContentLoaded', function () {
    const formulariosDesactivar = document.querySelectorAll('.form-desactivar');

    formulariosDesactivar.forEach(function (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();

            Swal.fire({
                title: '¿Desactivar producto?',
                text: 'El producto ya no estará disponible en el punto de venta.',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#dc3545',
                cancelButtonColor: '#6c757d',
                confirmButtonText: 'Sí, desactivar',
                cancelButtonText: 'Cancelar',
                reverseButtons: true
            }).then(function (result) {
                if (result.isConfirmed) {
                    HTMLFormElement.prototype.submit.call(form);
                }
            });
        });
    });
});