document.addEventListener('DOMContentLoaded', function () {
    const sidebar = document.getElementById('sidebarNav');
    const overlay = document.getElementById('sidebarOverlay');
    const btnHamburger = document.getElementById('btnHamburger');

    // Desplegar y contraer submenús
    document.querySelectorAll('.submenu-toggle').forEach(function (toggle) {
        toggle.addEventListener('click', function (e) {
            e.preventDefault();
            const parent = this.closest('.has-submenu');
            parent.classList.toggle('open');
        });
    });

    // Control del menú hamburguesa (móvil y tablet)
    if (btnHamburger && sidebar && overlay) {
        function toggleSidebar() {
            sidebar.classList.toggle('sidebar-open');
            overlay.classList.toggle('sidebar-overlay-visible');
            btnHamburger.classList.toggle('hamburger-active');
        }

        function closeSidebar() {
            sidebar.classList.remove('sidebar-open');
            overlay.classList.remove('sidebar-overlay-visible');
            btnHamburger.classList.remove('hamburger-active');
        }

        btnHamburger.addEventListener('click', toggleSidebar);
        overlay.addEventListener('click', closeSidebar);

        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape' && sidebar.classList.contains('sidebar-open')) {
                closeSidebar();
            }
        });
    }
});