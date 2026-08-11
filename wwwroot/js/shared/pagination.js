/**
 * Pagination Controls Component
 */
const Pagination = {
  render({ currentPage, totalPages, onPageChangeContainerId, onPageChangeCallback }) {
    const container = document.getElementById(onPageChangeContainerId);
    if (!container || totalPages <= 1) {
      if (container) container.innerHTML = '';
      return;
    }

    let buttonsHtml = '';
    
    // Prev
    buttonsHtml += `<button class="btn btn-secondary btn-sm" ${currentPage <= 1 ? 'disabled' : ''} id="pagPrev">Previous</button>`;

    // Page indicator
    buttonsHtml += `<span style="font-size: var(--font-size-sm); color: var(--text-muted); padding: 0 8px;">Page ${currentPage} of ${totalPages}</span>`;

    // Next
    buttonsHtml += `<button class="btn btn-secondary btn-sm" ${currentPage >= totalPages ? 'disabled' : ''} id="pagNext">Next</button>`;

    container.innerHTML = `<div class="flex items-center justify-end gap-2 mt-4">${buttonsHtml}</div>`;

    const prevBtn = container.querySelector('#pagPrev');
    const nextBtn = container.querySelector('#pagNext');

    if (prevBtn && currentPage > 1) {
      prevBtn.onclick = () => onPageChangeCallback(currentPage - 1);
    }

    if (nextBtn && currentPage < totalPages) {
      nextBtn.onclick = () => onPageChangeCallback(currentPage + 1);
    }
  }
};
