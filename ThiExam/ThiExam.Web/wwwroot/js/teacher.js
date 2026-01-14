/* ============================================
   TEACHER MODULE JAVASCRIPT
   ThiExam - Reusable components and utilities
   ============================================ */

const TeacherModule = (function () {
    'use strict';

    // ============================================
    // SIDEBAR MANAGEMENT
    // ============================================
    const Sidebar = {
        sidebar: null,
        overlay: null,
        toggleBtn: null,

        init() {
            this.sidebar = document.querySelector('.teacher-sidebar');
            this.overlay = document.querySelector('.sidebar-overlay');
            this.toggleBtn = document.querySelector('.menu-toggle-btn');

            if (this.toggleBtn) {
                this.toggleBtn.addEventListener('click', () => this.toggle());
            }

            if (this.overlay) {
                this.overlay.addEventListener('click', () => this.close());
            }

            // Close on escape key
            document.addEventListener('keydown', (e) => {
                if (e.key === 'Escape') this.close();
            });
        },

        toggle() {
            if (this.sidebar) {
                this.sidebar.classList.toggle('open');
                this.overlay?.classList.toggle('visible');
                document.body.classList.toggle('sidebar-open');
            }
        },

        close() {
            if (this.sidebar) {
                this.sidebar.classList.remove('open');
                this.overlay?.classList.remove('visible');
                document.body.classList.remove('sidebar-open');
            }
        }
    };

    // ============================================
    // ALERT MANAGEMENT
    // ============================================
    const Alerts = {
        autoHideDelay: 5000,

        init() {
            document.querySelectorAll('.alert').forEach(alert => {
                this.setupAlert(alert);
            });
        },

        setupAlert(alert) {
            const closeBtn = alert.querySelector('.alert-close');
            if (closeBtn) {
                closeBtn.addEventListener('click', () => this.dismiss(alert));
            }

            // Auto-hide after delay
            if (alert.dataset.autoHide !== 'false') {
                setTimeout(() => this.dismiss(alert), this.autoHideDelay);
            }
        },

        dismiss(alert) {
            alert.style.transition = 'opacity 0.3s, transform 0.3s';
            alert.style.opacity = '0';
            alert.style.transform = 'translateY(-0.5rem)';
            setTimeout(() => alert.remove(), 300);
        },

        show(type, title, message, container = '.teacher-content') {
            const alertHtml = `
                <div class="alert alert-${type}" role="alert">
                    <div class="alert-icon">
                        <i data-lucide="${type === 'success' ? 'check-circle' : 'alert-circle'}"></i>
                    </div>
                    <div class="alert-content">
                        <p class="alert-title">${title}</p>
                        <p class="alert-message">${message}</p>
                    </div>
                    <button type="button" class="alert-close" aria-label="Đóng">
                        <i data-lucide="x"></i>
                    </button>
                </div>
            `;

            const containerEl = document.querySelector(container);
            if (containerEl) {
                containerEl.insertAdjacentHTML('afterbegin', alertHtml);
                const newAlert = containerEl.querySelector('.alert');
                this.setupAlert(newAlert);
                
                // Re-init lucide icons
                if (typeof lucide !== 'undefined') {
                    lucide.createIcons();
                }
            }
        }
    };

    // ============================================
    // FORM UTILITIES
    // ============================================
    const Forms = {
        init() {
            this.setupLoadingButtons();
        },

        setupLoadingButtons() {
            document.querySelectorAll('form').forEach(form => {
                form.addEventListener('submit', (e) => {
                    // If using jQuery validation, check validity
                    if (typeof $ !== 'undefined' && typeof $.fn.valid === 'function') {
                        if (!$(form).valid()) return;
                    }

                    const submitBtn = form.querySelector('[type="submit"]');
                    if (submitBtn) {
                        this.setButtonLoading(submitBtn, true);
                    }
                });
            });
        },

        setButtonLoading(btn, loading) {
            if (loading) {
                btn.classList.add('btn-loading');
                btn.disabled = true;
            } else {
                btn.classList.remove('btn-loading');
                btn.disabled = false;
            }
        }
    };

    // ============================================
    // IMAGE PREVIEW
    // ============================================
    const ImagePreview = {
        init() {
            document.querySelectorAll('[data-image-preview]').forEach(btn => {
                const inputSelector = btn.dataset.imagePreview;
                const previewSelector = btn.dataset.previewTarget;
                
                btn.addEventListener('click', () => {
                    this.showPreview(inputSelector, previewSelector);
                });
            });
        },

        showPreview(inputSelector, previewSelector) {
            const input = document.querySelector(inputSelector);
            const preview = document.querySelector(previewSelector);
            
            if (!input || !preview) return;

            const url = input.value.trim();
            if (!url) {
                preview.classList.remove('visible');
                return;
            }

            const img = preview.querySelector('img');
            if (img) {
                img.src = url;
                img.onload = () => preview.classList.add('visible');
                img.onerror = () => {
                    Alerts.show('error', 'Lỗi', 'Không thể tải hình ảnh. Vui lòng kiểm tra URL.');
                    preview.classList.remove('visible');
                };
            }
        }
    };

    // ============================================
    // PUBLIC API
    // ============================================
    return {
        init() {
            // Initialize all modules
            Sidebar.init();
            Alerts.init();
            Forms.init();
            ImagePreview.init();

            // Initialize Lucide icons if available
            if (typeof lucide !== 'undefined') {
                lucide.createIcons();
            }

            console.log('TeacherModule initialized');
        },

        Sidebar,
        Alerts,
        Forms,
        ImagePreview
    };
})();

// Auto-initialize when DOM is ready
document.addEventListener('DOMContentLoaded', () => TeacherModule.init());
