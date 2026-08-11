let vacancyId = null;

document.addEventListener('DOMContentLoaded', async () => {
  Guards.requireRole(CONSTANTS.ROLES.EMPLOYER);
  Navigation.renderShell(CONSTANTS.ROLES.EMPLOYER, 'vacancies');

  const params = new URLSearchParams(window.location.search);
  vacancyId = params.get('id');

  if (!vacancyId) {
    window.location.href = CONSTANTS.ROUTES.EMPLOYER.VACANCIES;
    return;
  }

  await loadVacancySummary();
});

async function loadVacancySummary() {
  const container = document.getElementById('vacancyDetailsContainer');

  try {
    const vacancy = await Api.get(`${CONSTANTS.API_ENDPOINTS.VACANCIES.BASE}/${vacancyId}`);

    const skillsHtml = vacancy.requiredSkills && vacancy.requiredSkills.length > 0
      ? vacancy.requiredSkills.map(s => `<span class="badge badge-shortlisted">${UI.escapeHtml(s.name)}</span>`).join(' ')
      : '<span class="text-muted text-sm">No specific skills listed</span>';

    container.innerHTML = `
      <div class="card max-w-[800px]">
        <div class="flex items-center justify-between mb-4">
          <div>
            <h1 class="h2 mb-1">${UI.escapeHtml(vacancy.title)}</h1>
            <div class="text-sm text-muted">${UI.icons.location} ${UI.escapeHtml(vacancy.location || 'Remote')} • Posted ${UI.formatDate(vacancy.createdAt)}</div>
          </div>
          ${vacancy.isOpen ? '<span class="badge badge-selected" style="font-size: var(--font-size-sm); padding: 6px 12px;">Open for Applications</span>' : '<span class="badge badge-rejected" style="font-size: var(--font-size-sm); padding: 6px 12px;">Closed</span>'}
        </div>

        <div class="mb-6 pt-4 border-top">
          <h3 class="h4 mb-2">Job Description</h3>
          <div class="text-muted" style="white-space: pre-line; line-height: 1.6;">${UI.escapeHtml(vacancy.description)}</div>
        </div>

        <div class="mb-6">
          <h3 class="h4 mb-2">Required Skills</h3>
          <div class="flex flex-wrap gap-2">${skillsHtml}</div>
        </div>

        <div class="grid grid-cols-2 gap-4 pt-4 border-top text-sm mb-6">
          <div>
            <span class="font-semibold text-main">Required Experience:</span>
            <span class="text-muted">${vacancy.requiredExperienceYears || 0}+ Years</span>
          </div>
          <div>
            <span class="font-semibold text-main">Education Level:</span>
            <span class="text-muted">${UI.escapeHtml(vacancy.requiredEducationLevel || 'No Requirement')}</span>
          </div>
        </div>

        <div class="flex items-center gap-3 pt-4 border-top">
          <a href="/employer/applications.html?vacancyId=${vacancy.id}" class="btn btn-primary btn-lg">View Matched Applicants</a>
          <a href="/employer/vacancy-edit.html?id=${vacancy.id}" class="btn btn-secondary btn-lg">Edit Vacancy</a>
        </div>
      </div>
    `;
  } catch (err) {
    container.innerHTML = `<div class="card text-danger text-center py-6">Failed to load vacancy summary. ${UI.escapeHtml(err.message)}</div>`;
  }
}
