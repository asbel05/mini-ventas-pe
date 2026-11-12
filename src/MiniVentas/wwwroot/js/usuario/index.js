document.addEventListener('DOMContentLoaded', function () {
    const modal = document.getElementById('modalCambiarEstado');
    if (!modal) return;

    modal.addEventListener('show.bs.modal', function (event) {
        const boton = event.relatedTarget;
        const id = boton.getAttribute('data-id');
        const nombre = boton.getAttribute('data-nombre');
        const activo = boton.getAttribute('data-activo') === 'true';

        document.getElementById('inputEstadoId').value = id;
        document.getElementById('inputEstadoActivo').value = activo ? 'true' : 'false';

        const mensaje = document.getElementById('modalMensajeEstado');
        const btnConfirmar = document.getElementById('btnConfirmarEstado');

        if (activo) {
            mensaje.textContent = `¿Desea activar el acceso del usuario "${nombre}"?`;
            btnConfirmar.textContent = 'Sí, activar';
            btnConfirmar.className = 'btn btn-success';
        } else {
            mensaje.textContent = `¿Desea desactivar el acceso del usuario "${nombre}"? No podrá iniciar sesión.`;
            btnConfirmar.textContent = 'Sí, desactivar';
            btnConfirmar.className = 'btn btn-danger';
        }
    });
});