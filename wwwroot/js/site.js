/* ═══════════════════════════════════════════
   GRAND AZURE HOTEL — SITE JS
   ═══════════════════════════════════════════ */

document.addEventListener('DOMContentLoaded', function () {

    /* ── Dark Mode Toggle ───────────────── */
    const toggle = document.getElementById('darkModeToggle');
    const html = document.documentElement;

    // Restore saved preference
    if (localStorage.getItem('theme') === 'dark') {
        html.setAttribute('data-theme', 'dark');
        if (toggle) toggle.innerHTML = '<i class="fa-solid fa-sun"></i>';
    }

    if (toggle) {
        toggle.addEventListener('click', function () {
            const isDark = html.getAttribute('data-theme') === 'dark';
            if (isDark) {
                html.removeAttribute('data-theme');
                localStorage.setItem('theme', 'light');
                toggle.innerHTML = '<i class="fa-solid fa-moon"></i>';
            } else {
                html.setAttribute('data-theme', 'dark');
                localStorage.setItem('theme', 'dark');
                toggle.innerHTML = '<i class="fa-solid fa-sun"></i>';
            }
        });
    }

    /* ── Sidebar Toggle (Mobile) ────────── */
    const sidebarToggle = document.getElementById('sidebarToggle');
    const sidebar = document.getElementById('sidebar');
    const overlay = document.getElementById('sidebarOverlay');

    function closeSidebar() {
        if (sidebar) sidebar.classList.remove('open');
        if (overlay) overlay.classList.remove('active');
    }

    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', function () {
            sidebar.classList.toggle('open');
            overlay.classList.toggle('active');
        });
    }

    if (overlay) {
        overlay.addEventListener('click', closeSidebar);
    }

    /* ── Auto-dismiss Alerts ────────────── */
    document.querySelectorAll('.alert[data-auto-dismiss]').forEach(function (el) {
        var delay = parseInt(el.getAttribute('data-auto-dismiss')) || 5000;
        setTimeout(function () {
            el.style.transition = 'opacity 300ms ease, transform 300ms ease';
            el.style.opacity = '0';
            el.style.transform = 'translateY(-8px)';
            setTimeout(function () { el.remove(); }, 300);
        }, delay);
    });

    /* ── Toast Notification System ──────── */
    window.showToast = function (type, title, message) {
        var container = document.getElementById('toastContainer');
        if (!container) return;

        var toast = document.createElement('div');
        toast.className = 'toast-item toast-' + type;
        toast.innerHTML =
            '<div class="toast-title">' + title + '</div>' +
            (message ? '<div class="toast-body-text">' + message + '</div>' : '') +
            '<button class="toast-close" onclick="this.parentElement.classList.add(\'toast-exit\');setTimeout(function(){this.parentElement.remove()}.bind(this),200)">&times;</button>' +
            '<div class="toast-progress"></div>';

        container.appendChild(toast);

        // Auto-dismiss after 4 seconds
        setTimeout(function () {
            if (toast.parentElement) {
                toast.classList.add('toast-exit');
                setTimeout(function () {
                    if (toast.parentElement) toast.remove();
                }, 200);
            }
        }, 4000);
    };

    /* ── Credential Hint Cards (Login) ──── */
    document.querySelectorAll('.credential-hint').forEach(function (card) {
        card.addEventListener('click', function () {
            var username = this.getAttribute('data-username');
            var password = this.getAttribute('data-password');
            var usertype = this.getAttribute('data-usertype');

            var usernameField = document.getElementById('Username');
            var passwordField = document.getElementById('Password');

            if (usernameField) usernameField.value = username;
            if (passwordField) passwordField.value = password;

            if (usertype) {
                var radio = document.getElementById('type' + usertype);
                if (radio) radio.checked = true;
            }
        });
    });
});
