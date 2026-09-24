(function () {
    const container = document.getElementById('toastContainer');
    if (!container) return;

    const AUTO_CLOSE_MS = 3500;

    function closeToast(toast) {
        toast.classList.add('toast-hiding');
        toast.addEventListener('animationend', () => toast.remove(), { once: true });
    }

    container.querySelectorAll('[data-toast]').forEach(toast => {
        // Botón de cerrar
        toast.querySelector('.toast-close')?.addEventListener('click', () => closeToast(toast));

        // Auto-cierre
        setTimeout(() => closeToast(toast), AUTO_CLOSE_MS);
    });
})();