/**
 * Modal Manager for Action Confirmations & Dialogs
 */
const Modal = {
  confirm({ title = 'Confirm Action', message = '', htmlMessage = '', isHtml = false, confirmText = 'Confirm', cancelText = 'Cancel', onConfirm = null, onCancel = null }) {
    const overlay = document.createElement('div');
    overlay.className = 'modal-overlay';
    const isHtmlContent = isHtml || !!htmlMessage || (typeof message === 'string' && /^\s*<[a-z][\s\S]*>/i.test(message));
    const bodyContent = isHtmlContent ? (htmlMessage || message) : UI.escapeHtml(message);

    overlay.innerHTML = `
      <div class="modal">
        <div class="modal-header">
          <div class="modal-title">${UI.escapeHtml(title)}</div>
          <button class="modal-close-btn" id="modalCloseBtn">&times;</button>
        </div>
        <div class="modal-body" style="color: var(--text-muted); font-size: var(--font-size-sm);">
          ${bodyContent}
        </div>
        <div class="modal-footer">
          ${cancelText ? `<button class="btn btn-secondary" id="modalCancelBtn">${UI.escapeHtml(cancelText)}</button>` : ''}
          ${confirmText ? `<button class="btn btn-primary" id="modalConfirmBtn">${UI.escapeHtml(confirmText)}</button>` : ''}
        </div>
      </div>
    `;

    document.body.appendChild(overlay);
    setTimeout(() => overlay.classList.add('show'), 10);

    const close = () => {
      overlay.classList.remove('show');
      setTimeout(() => overlay.remove(), 200);
    };

    overlay.querySelector('#modalCloseBtn').onclick = close;
    const cancelBtn = overlay.querySelector('#modalCancelBtn');
    if (cancelBtn) {
      cancelBtn.onclick = () => {
        close();
        if (typeof onCancel === 'function') onCancel();
      };
    }
    const confirmBtn = overlay.querySelector('#modalConfirmBtn');
    if (confirmBtn) {
      confirmBtn.onclick = () => {
        close();
        if (typeof onConfirm === 'function') onConfirm();
      };
    }

    overlay.addEventListener('click', (e) => {
      if (e.target === overlay) close();
    });
  }
};
