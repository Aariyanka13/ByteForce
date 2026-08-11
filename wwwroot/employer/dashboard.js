document.addEventListener('DOMContentLoaded', async () => {
  Guards.requireRole(CONSTANTS.ROLES.EMPLOYER);
  Navigation.renderShell(CONSTANTS.ROLES.EMPLOYER, 'dashboard');

  try {
    const [profile, vacancies, requests] = await Promise.all([
      Api.get(CONSTANTS.API_ENDPOINTS.EMPLOYER.PROFILE),
      Api.get(CONSTANTS.API_ENDPOINTS.VACANCIES.BASE),
      Api.get(CONSTANTS.API_ENDPOINTS.CONTACT_REQUESTS.EMPLOYER_LIST)
    ]);

    if (profile && profile.companyName) {
      document.getElementById('welcomeCompanyHeader').textContent = `${profile.companyName} Portal`;
    }

    const openVacancies = (vacancies || []).filter(v => v.isOpen);
    document.getElementById('activeVacanciesCount').textContent = openVacancies.length;

    const totalReqs = requests || [];
    document.getElementById('sentRequestsCount').textContent = totalReqs.length;
    const acceptedCount = totalReqs.filter(r => r.status === 'Accepted').length;
    document.getElementById('acceptedConnectionsCount').textContent = acceptedCount;

    // Render recent vacancies
    const recentBox = document.getElementById('recentVacanciesList');
    if (!vacancies || vacancies.length === 0) {
      recentBox.innerHTML = `
        <div class="empty-state py-4">
          <div class="empty-title text-base">No Vacancies Posted Yet</div>
          <div class="empty-text text-sm mb-3">Create your first vacancy to start receiving matched candidates.</div>
          <a href="/employer/vacancy-create.html" class="btn btn-primary btn-sm">Post Vacancy</a>
        </div>
      `;
    } else {
      const recent = vacancies.slice(0, 4);
      recentBox.innerHTML = recent.map(v => `
        <div class="p-3 border rounded flex items-center justify-between" style="font-size: var(--font-size-sm);">
          <div>
            <div class="font-semibold text-main">${UI.escapeHtml(v.title)}</div>
            <div class="text-xs text-muted">${UI.icons.location} ${UI.escapeHtml(v.location || 'Remote')} • Posted ${UI.formatDate(v.createdAt)}</div>
          </div>
          <div class="flex items-center gap-2">
            ${v.isOpen ? '<span class="badge badge-selected">Open</span>' : '<span class="badge badge-rejected">Closed</span>'}
            <a href="/employer/applications.html?vacancyId=${v.id}" class="btn btn-secondary btn-sm">Applicants</a>
          </div>
        </div>
      `).join('');
    }
  } catch (err) {
    Toast.error('Failed to load employer metrics');
  }
});
