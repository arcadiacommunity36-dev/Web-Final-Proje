// ========================================================================
// GameQuest — Site JavaScript
// ========================================================================

(function () {
    'use strict';

    // ---------- Navbar scroll effect ----------
    const navbar = document.getElementById('mainNavbar');
    if (navbar) {
        const onScroll = () => {
            if (window.scrollY > 30) {
                navbar.classList.add('scrolled');
            } else {
                navbar.classList.remove('scrolled');
            }
        };
        window.addEventListener('scroll', onScroll, { passive: true });
        onScroll();
    }

    // ---------- Fade-in animation on load ----------
    const animItems = document.querySelectorAll('.gq-animate-in');
    if (animItems.length) {
        const observer = new IntersectionObserver(
            (entries) => {
                entries.forEach((entry) => {
                    if (entry.isIntersecting) {
                        entry.target.style.animationPlayState = 'running';
                        observer.unobserve(entry.target);
                    }
                });
            },
            { threshold: 0.1 }
        );
        animItems.forEach((el) => {
            el.style.animationPlayState = 'paused';
            observer.observe(el);
        });
    }

    // ---------- Active nav link highlight ----------
    const currentPath = window.location.pathname.toLowerCase();
    document.querySelectorAll('.navbar-nav .nav-link').forEach(link => {
        const href = link.getAttribute('href');
        if (href && href !== '/') {
            if (currentPath.startsWith(href.toLowerCase())) {
                link.classList.add('active');
            }
        } else if (href === '/' && currentPath === '/') {
            link.classList.add('active');
        }
    });

    // ---------- Smooth alert dismiss ----------
    document.querySelectorAll('.alert .btn-close').forEach(btn => {
        btn.addEventListener('click', function () {
            const alert = this.closest('.alert');
            if (alert) {
                alert.style.transition = 'opacity 0.3s, transform 0.3s';
                alert.style.opacity = '0';
                alert.style.transform = 'translateY(-10px)';
                setTimeout(() => alert.remove(), 300);
            }
        });
    });

})();
