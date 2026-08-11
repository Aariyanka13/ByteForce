document.addEventListener('DOMContentLoaded', () => {
  Guards.requireRole(CONSTANTS.ROLES.JOB_SEEKER);
  Navigation.renderShell(CONSTANTS.ROLES.JOB_SEEKER, 'contact-requests');

  loadContactRequests();
});

async function loadContactRequests() {
  const tbody = document.getElementById('contactRequestsTableBody');
  tbody.innerHTML = Loader.renderTableSkeleton(5, 3);

  try {
    const requests = await Api.get(CONSTANTS.API_ENDPOINTS.CONTACT_REQUESTS.JOBSEEKER_LIST);

    if (!requests || requests.length === 0) {
      tbody.innerHTML = `
        <tr>
          <td colspan="5" class="text-center py-6 text-muted">
            No contact requests received yet.
          </td>
        </tr>
      `;
      return;
    }

    tbody.innerHTML = requests.map(req => `
      <tr>
        <td class="font-semibold text-main">${UI.escapeHtml(req.vacancyTitle)}</td>
        <td>${UI.icons.building} ${UI.escapeHtml(req.companyName)}</td>
        <td>${UI.formatDate(req.createdAt)}</td>
        <td>${UI.getStatusBadge(req.status)}</td>
        <td style="text-align: right;">
          ${req.status === 'Pending' ? `
            <button class="btn btn-primary btn-sm" onclick="respondRequest(${req.id}, 'Accepted')">Accept</button>
            <button class="btn btn-secondary btn-sm" onclick="respondRequest(${req.id}, 'Declined')">Decline</button>
          ` : `<span class="text-xs text-muted">Responded ${UI.formatDate(req.respondedAt)}</span>`}
        </td>
      </tr>
    `).join('');
  } catch (err) {
    tbody.innerHTML = `<tr><td colspan="5" class="text-danger text-center py-4">Failed to load contact requests.</td></tr>`;
  }
}

async function respondRequest(id, status) {
  const message = status === 'Accepted' 
    ? 'Are you sure you want to accept this contact request? The employer will be granted permission to view your email address and phone number.'
    : 'Are you sure you want to decline this contact request?';

  Modal.confirm({
    title: `${status} Contact Request`,
    message,
    confirmText: status,
    onConfirm: async () => {
      try {
        await Api.put(CONSTANTS.API_ENDPOINTS.CONTACT_REQUESTS.RESPOND(id), { status });
        Toast.success(`Contact request ${status.toLowerCase()}!`);
        await loadContactRequests();
      } catch (err) {
        Toast.error(err.message || 'Failed to respond to request');
      }
    }
  });
}
