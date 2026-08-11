document.addEventListener('DOMContentLoaded', () => {
  Guards.requireRole(CONSTANTS.ROLES.EMPLOYER);
  Navigation.renderShell(CONSTANTS.ROLES.EMPLOYER, 'contact-requests');

  loadEmpContactRequests();
});

async function loadEmpContactRequests() {
  const tbody = document.getElementById('empContactRequestsTableBody');
  tbody.innerHTML = Loader.renderTableSkeleton(5, 4);

  try {
    const list = await Api.get(CONSTANTS.API_ENDPOINTS.CONTACT_REQUESTS.EMPLOYER_LIST);

    if (!list || list.length === 0) {
      tbody.innerHTML = `
        <tr>
          <td colspan="5" class="text-center py-6 text-muted">
            No contact requests sent yet. Change candidate application status to 'Selected' to send a contact request.
          </td>
        </tr>
      `;
      return;
    }

    tbody.innerHTML = list.map(req => `
      <tr>
        <td class="font-semibold text-main">${UI.escapeHtml(req.vacancyTitle)}</td>
        <td>${UI.icons.user} ${UI.escapeHtml(req.jobSeekerName || 'Candidate')}</td>
        <td>${UI.formatDate(req.createdAt)}</td>
        <td>${UI.getStatusBadge(req.status)}</td>
        <td style="text-align: right;">
          ${req.status === 'Accepted' ? `
            <button class="btn btn-primary btn-sm" onclick="viewUnlockedContactDetails(${req.id})">${UI.icons.lock} View Contact Info</button>
          ` : `<span class="text-xs text-muted">Pending candidate response</span>`}
        </td>
      </tr>
    `).join('');
  } catch (err) {
    tbody.innerHTML = `<tr><td colspan="5" class="text-danger text-center py-4">Failed to load contact requests log.</td></tr>`;
  }
}

async function viewUnlockedContactDetails(requestId) {
  try {
    const details = await Api.get(CONSTANTS.API_ENDPOINTS.CONTACT_REQUESTS.CONTACT_DETAILS(requestId));

    Modal.confirm({
      title: 'Candidate Contact Details',
      isHtml: true,
      htmlMessage: `
        <div class="flex flex-col gap-3 pt-2" style="font-size: var(--font-size-base);">
          <div class="p-3 border rounded flex items-center justify-between">
            <div>
              <div class="text-xs text-muted font-semibold mb-1">CANDIDATE NAME</div>
              <div class="font-semibold text-main">${UI.escapeHtml(details.jobSeekerName || 'Candidate')}</div>
            </div>
          </div>
          <div class="p-3 border rounded flex items-center justify-between">
            <div>
              <div class="text-xs text-muted font-semibold mb-1">EMAIL ADDRESS</div>
              <a href="mailto:${UI.escapeHtml(details.email)}" class="link font-semibold">${UI.escapeHtml(details.email)}</a>
            </div>
            <button class="btn btn-secondary btn-sm" onclick="navigator.clipboard.writeText('${UI.escapeHtml(details.email)}'); Toast.success('Email copied to clipboard!');">Copy Email</button>
          </div>
          <div class="p-3 border rounded">
            <div class="text-xs text-muted font-semibold mb-1">PHONE NUMBER</div>
            <a href="tel:${UI.escapeHtml(details.phone)}" class="link font-semibold">${UI.escapeHtml(details.phone || 'Not provided')}</a>
          </div>
        </div>
      `,
      confirmText: 'Close',
      cancelText: '',
      onConfirm: () => {}
    });
  } catch (err) {
    Toast.error(err.message || 'Failed to fetch candidate contact details.');
  }
}
