let vacancyId = null;

document.addEventListener('DOMContentLoaded', async () => {
  Guards.requireRole(CONSTANTS.ROLES.EMPLOYER);
  Navigation.renderShell(CONSTANTS.ROLES.EMPLOYER, 'vacancies');

  const params = new URLSearchParams(window.location.search);
  vacancyId = params.get('vacancyId');

  if (!vacancyId) {
    window.location.href = CONSTANTS.ROUTES.EMPLOYER.VACANCIES;
    return;
  }

  await loadApplicants();
});

async function loadApplicants() {
  const tbody = document.getElementById('applicantsTableBody');
  tbody.innerHTML = Loader.renderTableSkeleton(5, 4);

  try {
    const list = await Api.get(CONSTANTS.API_ENDPOINTS.VACANCIES.APPLICANTS(vacancyId));

    if (!list || list.length === 0) {
      tbody.innerHTML = `
        <tr>
          <td colspan="5" class="text-center py-6 text-muted">
            No candidates have applied for this vacancy yet.
          </td>
        </tr>
      `;
      return;
    }

    // Sort applicants by match score descending
    list.sort((a, b) => {
      const scoreA = a.matchScore !== undefined ? a.matchScore : (a.matchingScore || 0);
      const scoreB = b.matchScore !== undefined ? b.matchScore : (b.matchingScore || 0);
      return scoreB - scoreA;
    });

    tbody.innerHTML = list.map(app => {
      const candidateName = app.candidateName || app.candidateFullName || 'Candidate';
      const score = app.matchScore !== undefined ? app.matchScore : app.matchingScore;
      return `
        <tr>
          <td class="font-semibold text-main">
            ${UI.icons.user} ${UI.escapeHtml(candidateName)}
          </td>
          <td>${UI.getMatchScoreChip(score)}</td>
          <td>${UI.formatDate(app.appliedAt)}</td>
          <td>${UI.getStatusBadge(app.status)}</td>
          <td style="text-align: right;">
            <div class="flex items-center justify-end gap-2">
              <select class="form-control form-control-sm" style="width: 140px; padding: 4px 8px;" onchange="changeStatus(${app.applicationId}, this.value)">
                <option value="" disabled selected>Change Status...</option>
                <option value="UnderReview">Under Review</option>
                <option value="Shortlisted">Shortlisted</option>
                <option value="Selected">Selected</option>
                <option value="Rejected">Rejected</option>
              </select>

              ${app.status === 'Selected' ? `
                <button class="btn btn-primary btn-sm" onclick="sendContactRequest(${app.applicationId})">Send Contact Request</button>
              ` : ''}
            </div>
          </td>
        </tr>
      `;
    }).join('');
  } catch (err) {
    tbody.innerHTML = `<tr><td colspan="5" class="text-danger text-center py-4">Failed to load applicants.</td></tr>`;
  }
}

async function changeStatus(applicationId, newStatus) {
  try {
    await Api.patch(CONSTANTS.API_ENDPOINTS.APPLICATIONS.UPDATE_STATUS(applicationId), { status: newStatus });
    Toast.success(`Status updated to ${newStatus}!`);
    await loadApplicants();
  } catch (err) {
    Toast.error(err.message || 'Failed to update status');
  }
}

async function sendContactRequest(applicationId) {
  Modal.confirm({
    title: 'Request Candidate Contact Details',
    message: 'Send an official contact request to this candidate? Once the candidate accepts your request, their direct email and phone number will become accessible.',
    confirmText: 'Send Contact Request',
    onConfirm: async () => {
      try {
        await Api.post(CONSTANTS.API_ENDPOINTS.CONTACT_REQUESTS.BASE, { jobApplicationId: applicationId });
        Toast.success('Contact request sent to candidate!');
      } catch (err) {
        Toast.error(err.message || 'Failed to send contact request');
      }
    }
  });
}
